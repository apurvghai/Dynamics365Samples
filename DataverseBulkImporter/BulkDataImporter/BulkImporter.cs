using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    public class BulkImporter
    {
        private readonly ILogger _logger;

        public BulkImporter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BulkImporter>();
        }

        [Function("AzFuncBulkOperationOrchestrator")]
        public async Task Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            _logger.LogInformation("AzFuncBulkOperationOrchestrator was triggered via HTTP");

            this.Initialize();

            DataverseBulkImporter dataverseBulkImporter = new DataverseBulkImporter();

            NameValueCollection queryParameters = HttpUtility.ParseQueryString(req.Url.Query);

            string recordsToInsertParameter = queryParameters["records"];
            int recordsToInsert = !string.IsNullOrWhiteSpace(recordsToInsertParameter) ? Convert.ToInt32(recordsToInsertParameter) : 1000;

            string batchSizeParameter = queryParameters["batch"];
            int batchSize = !string.IsNullOrWhiteSpace(batchSizeParameter) ? Convert.ToInt32(batchSizeParameter) : 100;

            string dopParameter = queryParameters["dop"];
            int dop = !string.IsNullOrWhiteSpace(dopParameter) ? Convert.ToInt32(dopParameter) : 20;

            string correlationIdParameter = queryParameters["correlationId"];
            string correlationId = correlationIdParameter ?? executionContext.InvocationId;

            string userIndexParameter = queryParameters["userIndex"];
            int userIndex = !string.IsNullOrWhiteSpace(userIndexParameter) ? Convert.ToInt32(userIndexParameter) : 0;

            string isElasticTableParameter = queryParameters["elastic"];
            bool isElasticTable = !string.IsNullOrWhiteSpace(isElasticTableParameter) ? Convert.ToBoolean(isElasticTableParameter) : false;

            _logger.LogInformation($"Records to Insert: {recordsToInsert}");
            _logger.LogInformation($"Batch Size: {batchSize}");
            _logger.LogInformation($"DOP: {dop}");
            _logger.LogInformation($"Correlation Id: {correlationId}");
            _logger.LogInformation($"User Index: {userIndex}");

            await dataverseBulkImporter.ExecuteAsync(
                logger: _logger,
                userIndex: userIndex,
                isElasticTable: isElasticTable,
                recordsToInsert: recordsToInsert,
                batchSize: batchSize,
                dop: dop,
                sessionTrackingId: correlationId,
                maxRetryCount: 40);
        }

        private void Initialize()
        {
            //Change max connections from .NET to a remote service default: 2
            ServicePointManager.DefaultConnectionLimit = 65000;

            //Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4
            ThreadPool.SetMinThreads(100, 100);

            //Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server
            ServicePointManager.Expect100Continue = false;

            //Can decrease overall transmission overhead but can cause delay in data packet arrival
            ServicePointManager.UseNagleAlgorithm = false;

            //Updated for faster handshake
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }
    }
}
