using CommitFetcher.GitCLI;
using CommitFetcher.Interfaces;
using CommitParser.GitCLI;
using CommitParser.Interfaces;
using CommitViewer.Logger;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using log4net;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.log4net;
using CommandLine;

namespace CommitViewer.Console
{
    class Program
    {

        /// <summary>
        /// Setting up the IoC container 
        /// </summary>
        /// <returns>The IoC container instance</returns>
        public static IUnityContainer SetupUnityContainer()
        {
            IUnityContainer container = new UnityContainer();

            // adding ILog (from lo4net) provider
            container.AddNewExtension<Log4NetExtension>();

            // adding fetching and parsing related singletons
            container.RegisterType<ICommitViewerLog, Log4NetWrapper>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitFetcher<CommitDTO>, GitCLICommitFetcher>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitParser, CommitParser.GitCLI.CommitParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitIdParser, CommitIdParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitAuthorParser, CommitAuthorParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitDateParser, CommitDateParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitMessageParser, CommitMessageParser>(new ContainerControlledLifetimeManager());

            return container;
        }

        public static void ConfigureLogging()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        static void Main(string[] args)
        {
            ConfigureLogging();

            var container = SetupUnityContainer();

            var fetcher = container.Resolve<ICommitFetcher<CommitDTO>>();

            Parser.Default.ParseArguments<ConsoleOptions>(args)
                .WithParsed(o =>
                {
                    var commits = fetcher.GetCommits(o.Url, o.Skip, o.Top).Result;

                    foreach (var commit in commits)
                    {
                        System.Console.WriteLine($"{commit.Id} {commit.Message}");
                    }
                }); 

            System.Console.ReadLine();
        }
    }
}
