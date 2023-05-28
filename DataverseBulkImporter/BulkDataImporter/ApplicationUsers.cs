using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using System;
using System.Threading.Tasks;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    public class ApplicationUsers
    {
        private string _vaultUrl = Environment.GetConfigurationSetting("KeyVaultUrl");
        private SecretClient secretClient;

        public ApplicationUsers()
        {
            this.secretClient = new SecretClient(new System.Uri(_vaultUrl), new DefaultAzureCredential());
        }

        public async Task<ApplicationUser> GetUserAsync(int index)
        {
            switch (index)
            {
                case 1:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserOne")).Value);

                case 2:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserTwo")).Value);

                case 3:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserThree")).Value);

                case 4:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserFour")).Value);

                case 5:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserFive"))?.Value);

                case 6:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserSix"))?.Value);

                case 7:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserSeven"))?.Value);

                case 8:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserEight"))?.Value);

                case 9:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>a",
                        (await this.GetSecretValueAsync("ApplicationUserNine"))?.Value);

                case 10:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserTen"))?.Value);

                case 11:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserEleven"))?.Value);

                case 12:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserTwelve"))?.Value);

                case 13:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserThirteen"))?.Value);

                case 14:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserFourteen"))?.Value);

                case 15:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserFifteen"))?.Value);

                case 16:
                    return new ApplicationUser(
                        "<Your Application ID / Azure App ID>",
                        (await this.GetSecretValueAsync("ApplicationUserSixteen"))?.Value);
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Retrieve Secret
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task<KeyVaultSecret> GetSecretValueAsync(string key)
        {
            return (await this.secretClient.GetSecretAsync(key)).Value;
        }
    }

    public class ApplicationUser
    {
        public ApplicationUser(string id, string clientSecret)
        {
            this.Id = id;
            this.ClientSecret = clientSecret;
        }

        public string Id { get; }

        public string ClientSecret { get; }
    }
}
