using Microsoft.Extensions.Configuration;
using NetCoreSeleniumWebCrawler.Selenium.Utils;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using Console = Colorful.Console;

namespace NetCoreSeleniumWebCrawler.Crawler
{
    public class GoogleSearchCrawler
    {
        private IConfiguration _configuration;
        private Browser _browser;
        private IWebDriver _driver;
        public string _resultSearchPage = string.Empty;
        private double _timeOutLoadPage = 10;

        public GoogleSearchCrawler(IConfiguration configuration, Browser browser)
        {
            _configuration = configuration;
            _browser = browser;
            _timeOutLoadPage = Convert.ToDouble(_configuration.GetSection("Selenium:TimeOutLoadPage").Value);

            var caminhoDriver = string.Empty;
            var headless = Convert.ToBoolean(_configuration.GetSection("Selenium:Headless").Value);

            switch (browser)
            {
                case Browser.Firefox:
                    caminhoDriver = _configuration.GetSection("Selenium:FirefoxDriverPath").Value;
                    break;

                case Browser.Chrome:
                    caminhoDriver = _configuration.GetSection("Selenium:ChromeDriverPath").Value;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(browser), browser, null);
            }

            _driver = WebDriverFactory.CreateWebDriver(browser, caminhoDriver, headless);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CarregarPagina()
        {
            _driver.LoadPage(TimeSpan.FromSeconds(_timeOutLoadPage), _configuration.GetSection("Selenium:UrlGoogle").Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="valor"></param>
        public void PreencherTermoPesquisa(string valor)
        {
            _driver.SetText(By.Name("q"), valor);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SubmeterPesquisa()
        {
            _driver.Submit(By.Name("q"));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_timeOutLoadPage);

            _resultSearchPage = _driver.Url;
        }

        /// <summary>
        /// 
        /// </summary>
        public void VerificarOcorrencias()
        {
            var searchResultPanel = _driver.FindElement(By.Id("search"));
            var results = searchResultPanel.FindElements(By.XPath("//div[@class='r']/a"));

            var links = new List<string>();

            foreach (var result in results)
                links.Add(result.GetAttribute("href"));

            var totalSites = links.Count;
            var contadorLoop = 0;
            var totalOcorrencias = 0;

            foreach (var link in links)
            {
                contadorLoop++;

                try
                {
                    Console.Write(string.Format("- Verificando site {0} de {1} -> ", contadorLoop, totalSites), Color.LightBlue);

                    _driver.Navigate().GoToUrl(link);
                    _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(_timeOutLoadPage);

                    var bodyTag = _driver.FindElement(By.TagName("body"));

                    if (string.IsNullOrEmpty(bodyTag.Text))
                        Console.WriteLine(" Nenhuma ocorrência, site sem texto.", Color.Yellow);
                    else
                    {
                        var found = Regex.Matches(bodyTag.Text, "Juiz de Fora").Count;

                        totalOcorrencias += found;

                        if (found > 0)
                            Console.WriteLine(string.Format("{0} ocorrência(s)", found), Color.Green);
                        else
                            Console.WriteLine("Nenhuma ocorrência", Color.Yellow);
                    }
                }
                catch (Exception)
                {
                    // Caso ocorra algum erro aqui, devo continuar até o fim do loop.
                    Console.WriteLine("Ocorreu um erro ao buscar ocorrências", Color.Red);
                }
                finally
                {

                }
            }

            Console.WriteLine(string.Format("- Foram encontradas {0} ocorrências em {1} sites.", totalOcorrencias, totalSites), Color.Green);
        }

        /// <summary>
        /// 
        /// </summary>
        public void MostrarResultado()
        {
            var resultsPanel = _driver.FindElement(By.Id("search"));
            var searchResults = resultsPanel.FindElements(By.XPath(".//a"));

            foreach (var result in searchResults)
            {
                Console.WriteLine(result.Text);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Fechar()
        {
            _driver.Quit();
            _driver = null;
        }
    }
}
