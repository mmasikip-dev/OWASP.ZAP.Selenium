using System;
using System.Collections.Generic;
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
        [Test]
        public void OWASPZAPSample()
        {
            //ZAP VARIABLES
            var zapAddress = "127.0.0.1";
            var zapPort = 8080;
            var zapAPIKey = "qkt80fuj60usrvleljq0le468b";

            //SET TO HIDE CONSOLE
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            //SET OWASP ZAP PROXY
            var proxy = new Proxy
            {
                HttpProxy = $"{zapAddress}:{zapPort}"
            };

            //SET CHROME OPTIONS AND PROXY
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments(new string[] { "--no-sandbox", "--ignore-certificate-errors", "test-type", "acceptSslCerts", "--start-maximized" });
            chromeOptions.AcceptInsecureCertificates = true;
            chromeOptions.Proxy = proxy;

            //INITIALIZE DRIVER
            var driver = new ChromeDriver(chromeDriverService, chromeOptions);

            //NAVIGATE TO TARGET URL
            driver.Navigate().GoToUrl("https://online.asb.co.nz/apply/join/asb");

            //INITIALIZE ZAP API
            var clientApi = new ClientApi(zapAddress, zapPort, zapAPIKey);

            WaitForPassiveScanToComplete(clientApi);

            //QUIT DRIVER
            driver.Quit();
        }

        private static void WaitForPassiveScanToComplete(ClientApi api)
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