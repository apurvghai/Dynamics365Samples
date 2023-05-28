using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    /// <summary>
    /// Dataverse Bulk Importer class
    /// </summary>
    public class DataverseBulkImporter
    {
        /// <summary>
        /// Internal Method to Invoke Dataverse processing
        /// </summary>
        /// <param name="logger">ILogger</param>
        /// <param name="userIndex">Specify the User Index to Pick The Application User</param>
        /// <param entityName="isElasticTable">Set True to Demostrate hyperscale demo; else SQL Standard Table</param>
        /// <param entityName="recordsToInsert">Records to Insert</param>
        /// <param entityName="batchSize">Batch Size</param>
        /// <param entityName="dop">Degree of Parallelism </param>
        /// <param entityName="sessionTrackingId">Session ID for tracking. Can be shared with Microsoft Support for Troubleshooting</param>
        /// <param entityName="maxRetryCount">Retry Count for Dataverse</param>
        /// <returns>Task</returns>
        public async Task ExecuteAsync(
            ILogger logger,
            int userIndex,
            bool isElasticTable,
            int recordsToInsert,
            int batchSize,
            int dop,
            string sessionTrackingId,
            int maxRetryCount)
        {
            try
            {
                DataverseClient dataverseClient = new DataverseClient();
                List<Entity> records = null;
                string entityName = null;
                if (isElasticTable)
                {
                    entityName = ElasticTableEntitySchema.EntityName;
                    records = dataverseClient.GenerateElasticTablesSampleRecords(recordsToInsert);
                }
                else
                {
                    entityName = StandardTableEntitySchema.EntityName;
                    records = dataverseClient.GenerateStandardTablesSampleRecords(recordsToInsert);
                }

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                logger.LogInformation($"Started-Watch {sessionTrackingId} - StartedProcessing: {recordsToInsert} at {DateTime.Now} ");

                using (ServiceClient client = await dataverseClient.GetClientAsync(logger, userIndex))
                {
                    client.EnableAffinityCookie = false;
                    client.SessionTrackingId = Guid.Parse(sessionTrackingId);

                    this.CreateMultipleInParallel(client, entityName, records, logger, dop, batchSize, maxRetryCount);

                    logger.LogInformation($"{recordsToInsert} record(s) created in {stopWatch.Elapsed.TotalSeconds} seconds with SessionTrackingId {sessionTrackingId}");
                }

                stopWatch.Stop();

                logger.LogInformation($"Ended-Watch {sessionTrackingId} - CompletedProcessing: {recordsToInsert} in {stopWatch.Elapsed.Hours} hours {stopWatch.Elapsed.Minutes} mins. Full Clock: {stopWatch.Elapsed.Duration()}");

                await Task.Yield();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"{nameof(ExecuteAsync)} method failed");
                throw;
            }
        }

        /// <summary>
        /// Create Record using "Create Multiple" Organization Request 
        /// </summary>
        /// <param entityName="serviceClient">Service Client (Dataverse SDK)</param>
        /// <param entityName="entityName">Entity Name</param>
        /// <param entityName="entities">List of Entities to be Created in a Batch</param>
        /// <param entityName="logger">ILogger Instance</param>
        /// <param entityName="dop">Custom DOP</param>
        /// <param entityName="batchSize">Batch Size</param>
        /// <param entityName="maxRetryCount">Max Retry Count</param>
        private void CreateMultipleInParallel(
            ServiceClient serviceClient,
            string entityName,
            List<Entity> entities,
            ILogger logger,
            int dop,
            int batchSize,
            int maxRetryCount)
        {
            List<OrganizationRequest> createMultipleRequests = new List<OrganizationRequest>();

            List<List<Entity>> entityBatches = entities.Split(batchSize);
            foreach (List<Entity> entityBatch in entityBatches)
            {
                EntityCollection localEntityCollection = new EntityCollection();
                localEntityCollection.EntityName = entityName;
                localEntityCollection.Entities.AddRange(entityBatch);

                OrganizationRequest createMultipleRequest = new OrganizationRequest(Constants.OrganizationRequestCreateMultiple);

                //Pass Targets -> EntityCollection. Do not Change the Parameter Name. This is required
                createMultipleRequest.Parameters[Constants.Targets] = localEntityCollection;

                //Turn Off Custom Plugin Execution. Default is False
                createMultipleRequest.Parameters[Constants.BypassCustomPluginExecution] = true; 

                createMultipleRequests.Add(createMultipleRequest);
            }

            if (serviceClient.RetryPauseTime == default(TimeSpan))
            {
                serviceClient.RetryPauseTime = new TimeSpan(0, 0, 5);
            }

            if (serviceClient.MaxRetryCount == default(int))
            {
                serviceClient.MaxRetryCount = maxRetryCount;
            }

            logger.LogInformation($"MaxDop returned by Dataverse: {serviceClient.RecommendedDegreesOfParallelism}");
            ParallelOptions parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = dop == 0 ? serviceClient.RecommendedDegreesOfParallelism : dop,
            };

            Parallel.ForEach(createMultipleRequests, parallelOptions,
               () =>
               {
                   //Disable Sticky Session
                   serviceClient.EnableAffinityCookie = false;
                   return serviceClient.Clone();

               }, (batch, loopState, index, threadSafeClient) =>
               {
                   bool isProcessed = false;
                   int retryCount = 0;

                   do
                   {
                       try
                       {
                           OrganizationResponse response = (OrganizationResponse)threadSafeClient.Execute(batch);
                           isProcessed = true;
                       }
                       catch (Exception)
                       {
                           retryCount++;
                           Task.Delay(15); //Hard-Code Failures
                       }
                   }

                   while (!isProcessed && retryCount <= threadSafeClient.MaxRetryCount);
                   return threadSafeClient;
               },
               (threadSafeLocalInstance) =>
               {
                   //Dispose the cloned threadSafeClient instance
                   if (threadSafeLocalInstance != null)
                   {
                       threadSafeLocalInstance.Dispose();
                   }
               });
        }
    }
}
