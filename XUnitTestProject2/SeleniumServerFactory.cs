using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using System.Diagnostics;
using System.Linq;
using TestServerPOC;

namespace XUnitTestProject2
{
    public class SeleniumServerFactory<TStartup> : WebApplicationFactory<Startup> where TStartup : class
    {
        public string RootUrl { get; set; } // Save this use by tests
        public TestServer TestServer { get; private set; }
        public IWebHost HostWeb { get; set; }

        private readonly Process ProcessSelenium;

        public SeleniumServerFactory()
        {
            ProcessSelenium = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "selenium-standalone",
                    Arguments = "start",
                    UseShellExecute = true
                }
            };
            ProcessSelenium.Start();
        }

        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            HostWeb = builder.UseEnvironment("Testing").Build();
            HostWeb.Start();
            RootUrl = HostWeb.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault(); //Last is https://localhost:5001!

            //Fake Server we won't use...this is lame. Should be cleaner, or a utility class
            return new TestServer(new WebHostBuilder().UseStartup<MockStartup>());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                HostWeb.Dispose();
                ProcessSelenium.CloseMainWindow();
            }
        }
    }

    public class MockStartup
    {
        public void ConfigureServices()
        {
        }

        public void Configure()
        {
        }
    }
}