using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace NetCoreSeleniumWebCrawler.Selenium.Utils
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateWebDriver(Browser browser, string pathDriver, bool headless)
        {
            IWebDriver webDriver = null;

            switch (browser)
            {
                case Browser.Firefox:
                    var optionsFF = new FirefoxOptions();

                    if (headless)
                        optionsFF.AddArgument("--headless");

                    webDriver = new FirefoxDriver(pathDriver, optionsFF);

                    break;

                case Browser.Chrome:
                    var options = new ChromeOptions();

                    if (headless)
                        options.AddArgument("--headless");

                    webDriver = new ChromeDriver(pathDriver, options);

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(browser), browser, null);
            }

            return webDriver;
        }
    }
}
