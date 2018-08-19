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
            container.RegisterType<ICommitFetcher<CommitDTO>, GitCLICommitFetcher>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitParser, CommitParser.GitCLI.CommitParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitIdParser, CommitIdParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitAuthorParser, CommitAuthorParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitDateParser, CommitDateParser>(new ContainerControlledLifetimeManager());
            container.RegisterType<ICommitMessageParser, CommitMessageParser>(new ContainerControlledLifetimeManager());

            return container;
        }

        static void Main(string[] args)
        {
            //Uri uri = new Uri("https://github.com/rafaelumlei/tsoa.git");
            //string path = Regex.Replace(uri.PathAndQuery, ".git", "", RegexOptions.IgnoreCase);
            //System.Console.WriteLine(Path.GetTempPath());
            //string newpath = Path.GetTempPath() + path.Replace('/', '\\');
            //System.Console.WriteLine(newpath);
            //Directory.CreateDirectory(newpath);
            //System.Console.ReadLine();
            //CommitFetcher.GitCLI.CommitFetcher cf = new CommitFetcher.GitCLI.CommitFetcher(new CommitParser.GitCLI.CommitParser())
            //var commitParser = new CommitParser.GitCLI.CommitParser(new CommitIdParser(), new CommitAuthorParser(), new CommitDateParser(), new CommitMessageParser());
            //var commitFetcher = new GitCLICommitFetcher(commitParser);
            //var total = commitFetcher.GetCommits("https://github.com/rafaelumlei/tsoa.git", 0, 10).Result;
            //var total2 = commitFetcher.GetCommits("https://github.com/rafaelumlei/tsoa.git", 10, 10).Result;

            var container = SetupUnityContainer();

            var fetcher = container.Resolve<ICommitFetcher<CommitDTO>>();

            var commits = fetcher.GetCommits("https://github.com/rafaelumlei/tsoa.git", 0, 10).Result;

            foreach (var commit in commits)
            {
                System.Console.WriteLine($"{commit.Id} {commit.Message}");
            }

            System.Console.ReadLine();
        }
    }
}
