using Colorful;
using Microsoft.Extensions.Configuration;
using NetCoreSeleniumWebCrawler.Selenium.Utils;
using System;
using System.Drawing;
using System.IO;
using Console = Colorful.Console;

namespace NetCoreSeleniumWebCrawler.Crawler
{
    class Program
    {
        static void Main(string[] args)
        {
            var colorInfo = Color.LightBlue;
            var font = FigletFont.Load("fontes\\chunky.flf");
            var figlet = new Figlet(font);

            Console.WriteLine(figlet.ToAscii("Web Crawler"), Color.LawnGreen);
            Console.WriteLine("Google Search Crawler", Color.LawnGreen);
            Console.WriteLine("With Selenium", Color.Yellow);
            Console.WriteLine();

            try
            {
                Console.WriteLine("- Carregando configurações...", colorInfo);
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                var configuration = builder.Build();

                Console.WriteLine("- Carregando driver do Selenium...", colorInfo);
                var rotina = new GoogleSearchCrawler(configuration, Browser.Firefox);

                Console.WriteLine("- Carregando página de pesquisa do Google...", colorInfo);
                rotina.CarregarPagina();

                Console.WriteLine("- Pesquisando pelo termo...", colorInfo);
                rotina.PreencherTermoPesquisa("Juiz de Fora");

                Console.WriteLine("- Submetendo pesquisa...", colorInfo);
                rotina.SubmeterPesquisa();

                Console.WriteLine("- Verificando ocorrências...", colorInfo);
                rotina.VerificarOcorrencias();

                Console.WriteLine("- Encerrando driver do Selenium...", colorInfo);
                rotina.Fechar();
            }
            catch (Exception ex)
            {
                Console.WriteLine("- Ocorreu um erro!", Color.Red);
                Console.WriteLine(ex.Message, Color.Red);
            }
            finally
            {
                Console.WriteLine("- Pressione qualquer tecla para fechar...", colorInfo);
                Console.ReadKey();
            }
        }
    }
}
