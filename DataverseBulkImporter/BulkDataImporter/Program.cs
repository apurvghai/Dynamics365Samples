using Microsoft.Extensions.Hosting;

namespace Microsoft.Support.Dataverse.Samples.BulkDataImporter
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .Build();

            host.Run();
        }
    }
}
