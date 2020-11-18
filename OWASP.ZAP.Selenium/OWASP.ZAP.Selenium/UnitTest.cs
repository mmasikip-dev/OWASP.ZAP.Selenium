using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        const int ZAP_PORT = 8081;
        const string ZAP_API_KEY = "5ste6eglkhh83lvu263jqju2o3";
        public static ClientApi ZapApi;
        private IWebDriver _driver;
        private Process Process;

        [SetUp]
        public void BeforeTestRun()
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Start_ZAP_Headless.bat");

            //START ZAP HEADLESS
            Process = new Process();
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.StartInfo.FileName = path;
            Process.StartInfo.Arguments = $"{ZAP_ADDRESS} {ZAP_PORT}";
            Process.Start();

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
            EnablePassiveScan();
            WaitForPassiveScanToComplete();
        }

        [TearDown]
        public void AfterTestRun()
        {
            WriteZapHtmlReport();
            ZapApi.Dispose();
            ZapApi.core.shutdown();
            _driver.Quit();
        }

        private void WriteZapHtmlReport()
        {
            var reportFileName = $"C:\\Temp\\report-{DateTime.Now:dd-MMM-yyyy-hh-mm-ss}.html";
            File.WriteAllBytes(reportFileName, ZapApi.core.htmlreport());
        }

        private void EnablePassiveScan()
        {
            //ENABLE PASSIVE SCANNER
            ZapApi.pscan.enableAllScanners();
        }

        private void WaitForPassiveScanToComplete()
        {
            Console.WriteLine("--- Waiting for passive scan to complete... --- ");

            try
            {
                //GET ALL RECORDS TO SCAN
                var response = (ApiResponseElement)ZapApi.pscan.recordsToScan();

                //ITERATE UNTIL WE HAVE NONE
                while (!response.Value.Equals("0"))
                {
                    response = (ApiResponseElement)ZapApi.pscan.recordsToScan();
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