using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    public class DataverseClient
    {
        private const string DataverseInstance = "DataverseInstance";

        private readonly ApplicationUsers applicationUsers;

        public DataverseClient()
        {
            applicationUsers = new ApplicationUsers();
        }

        /// <summary>
        /// Get Service Client
        /// </summary>
        /// <param name="logger">Logger Instance</param>
        /// <param name="userIndex">Index of the application user</param>
        /// <returns>Service client instance</returns>
        public async Task<ServiceClient> GetClientAsync(ILogger logger, int userIndex)
        {
            ApplicationUser applicationUser = await this.applicationUsers.GetUserAsync(userIndex);

            string clientId = applicationUser.Id;
            string clientSecret = applicationUser.ClientSecret;

            return new ServiceClient(
                new Uri(Environment.GetConfigurationSetting(DataverseClient.DataverseInstance)),
                clientId,
                clientSecret,
                false,
                logger);
        }

        /// <summary>
        /// Generates sample entity records for Standard tables
        /// </summary>
        /// <param name="count">Number of records to create</param>
        /// <returns>List of sample entity records</returns>
        public List<Entity> GenerateStandardTablesSampleRecords(int count)
        {
            List<Entity> records = new List<Entity>();
            for (int i = 0; i < count; i++)
            {
                string recordName = $"Test-Record_{DateTime.Now.Date.ToString("u").Replace(@"/\", "_").Replace(":", "_").Replace(" ", "")}";

                Entity sampleEntityRecord = new Entity(StandardTableEntitySchema.EntityName);
                sampleEntityRecord[StandardTableEntitySchema.Name] = recordName;
                sampleEntityRecord[StandardTableEntitySchema.PhoneNumber] = "000-000-1100";
                sampleEntityRecord[StandardTableEntitySchema.PhoneNumberOne] = "000-000-1100";
                sampleEntityRecord[StandardTableEntitySchema.CityOne] = $"mum-{recordName}";
                records.Add(sampleEntityRecord);
            }

            return records;
        }

        /// <summary>
        /// Generates sample entity records for Elastic tables
        /// </summary>
        /// <param name="count">Number of records to create</param>
        /// <returns>List of sample entity records</returns>
        public List<Entity> GenerateElasticTablesSampleRecords(int count)
        {
            List<Entity> records = new List<Entity>();
            for (int i = 0; i < count; i++)
            {
                string recordName = $"Test-Record_{DateTime.Now.Date.ToString("u").Replace(@"/\", "_").Replace(":", "_").Replace(" ", "")}";

                Entity sampleEntityRecord = new Entity(ElasticTableEntitySchema.EntityName);
                sampleEntityRecord[ElasticTableEntitySchema.Name] = recordName;
                sampleEntityRecord[ElasticTableEntitySchema.PhoneNumber] = "000-000-1100";
                sampleEntityRecord[ElasticTableEntitySchema.PhoneNumberOne] = "000-000-1100";
                sampleEntityRecord[ElasticTableEntitySchema.CityOne] = $"mum-{recordName}";
                sampleEntityRecord[ElasticTableEntitySchema.TTLInSeconds] = 3600; //Time to Live
                records.Add(sampleEntityRecord);
            }

            return records;
        }
    }
}
