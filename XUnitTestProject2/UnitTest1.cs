using Microsoft.Extensions.DependencyInjection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using TestServerInMemoryDbPOC.Data;
using TestServerPOC;
using TestServerPOC.Data;
using Xunit;

namespace XUnitTestProject2
{
    public class SeleniumTests : IClassFixture<SeleniumServerFactory<Startup>>, IDisposable
    {
        public SeleniumServerFactory<Startup> Server { get; }
        public IWebDriver Browser { get; }
        public HttpClient Client { get; }

        public SeleniumTests(SeleniumServerFactory<Startup> server)
        {
            Server = server;
            Client = server.CreateClient(); // required for setup

            var opts = new ChromeOptions();
            opts.SetLoggingPreference(LogType.Browser, LogLevel.All);

            var driver = new RemoteWebDriver(opts);
            Browser = driver;
        }

        [Fact]
        public void Run()
        {
            // add new user
            var db = Server.HostWeb.Services.GetService<ApplicationDbContext>();

            db.User.Add(new User()
            {
                Name = "Andrew"
            });

            db.SaveChanges();

            // edit the user from the test (Home/Edit) action
            Browser.Navigate().GoToUrl(Server.RootUrl);

            var editBtn = By.Id("edit");
            Browser.FindElement(editBtn).Click();

            // just wait until ajax is done for now.
            Thread.Sleep(20000);

            db = Server.HostWeb.Services.GetService<ApplicationDbContext>();

            // Name should be "Herrick" as my Home/Edit updates the record
            var data = db.User.First();
            Assert.Equal("Herrick", data.Name);
        }

        public void Dispose()
        {
            Browser.Dispose();
        }
    }
}