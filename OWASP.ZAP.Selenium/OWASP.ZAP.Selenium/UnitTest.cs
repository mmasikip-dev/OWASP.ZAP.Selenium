using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OWASPZAPDotNetAPI;

namespace OWASP.ZAP.Selenium
{
    [TestFixture]
    public class UnitTest
    {
        const string ZAP_ADDRESS = "127.0.0.1";
        const int ZAP_PORT = 8080;
        const string ZAP_API_KEY = "qkt80fuj60usrvleljq0le468b";
        public static ClientApi ZapApi;
        private IWebDriver _driver;

        [SetUp]
        public void BeforeTestRun()
        {
            //INITIALIZE ZAP API
            ZapApi = new ClientApi(ZAP_ADDRESS, ZAP_PORT, ZAP_API_KEY);

            //SET TO HIDE CONSOLE
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            //SET OWASP ZAP PROXY
            var proxy = new Proxy
            {
                Kind = ProxyKind.Manual,
                IsAutoDetect = false,
                HttpProxy = $"{ZAP_ADDRESS}:{ZAP_PORT}",
                SslProxy = $"{ZAP_ADDRESS}:{ZAP_PORT}"
            };

            //SET CHROME OPTIONS AND PROXY
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new string[] { "--no-sandbox", "--ignore-certificate-errors", "test-type", "acceptSslCerts", "--start-maximized" });
            chromeOptions.AcceptInsecureCertificates = true;
            chromeOptions.Proxy = proxy;

            //INITIALIZE DRIVER
            _driver = new ChromeDriver(chromeDriverService, chromeOptions);
        }

        [Test]
        public void OWASPZAPSample()
        {
            //NAVIGATE TO TARGET URL
            _driver.Navigate().GoToUrl("https://online.asb.co.nz/apply/join/asb");

            //WaitForPassiveScanToComplete(ZapApi);
        }

        [TearDown]
        public void AfterTestRun()
        {
            WriteZapHtmlReport("C:\\Temp\\report.html", ZapApi.core.htmlreport());
            ZapApi.Dispose();
            _driver.Quit();
        }

        private void WriteZapHtmlReport(string path, byte[] bytes)
        {
            File.WriteAllBytes(path, bytes);
        }

        private void WaitForPassiveScanToComplete(ClientApi api)
        {
            Console.WriteLine("--- Waiting for passive scan to complete --- ");

            try
            {
                api.pscan.enableAllScanners(); // enable passive scanner.

                // getting a response
                var response = api.pscan.recordsToScan();


                //iterating till we get response as "0".
                while (!response.ToString().Equals("0"))
                {
                    response = api.pscan.recordsToScan();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("--- Passive scan completed! ---");
        }
    }
}