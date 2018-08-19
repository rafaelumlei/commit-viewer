using CommitFetcher.GitCLI;
using CommitParser.GitCLI;
using System.Linq;

namespace CommitViewer.Console
{
    class Program
    {
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
            var commitParser = new CommitParser.GitCLI.CommitParser(new CommitIdParser(), new CommitAuthorParser(), new CommitDateParser(), new CommitMessageParser());
            var commitFetcher = new GitCLICommitFetcher(commitParser);
            var total = commitFetcher.GetCommits("https://github.com/rafaelumlei/tsoa.git", 0, 10).Result;
            var total2 = commitFetcher.GetCommits("https://github.com/rafaelumlei/tsoa.git", 10, 10).Result;
            
            System.Console.ReadLine();
        }
    }
}
