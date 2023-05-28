using System;
using System.Configuration;
using System.Diagnostics;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    public static class Environment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigurationSetting(string key)
        {
            if (Debugger.IsAttached)
            {
                return System.Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.User);
            }

            return System.Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }
    }
}
