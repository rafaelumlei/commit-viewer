using CommitFetcher.Interfaces;
using CommitFetcher.Interfaces.Exceptions;
using CommitParser.Interfaces;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommitFetcher.GitCLI
{

    public class GitCLICommitFetcher : ICommitFetcher<CommitDTO>
    {
        private readonly string gitClone = "cd {0} > NUL && git.exe clone --quiet {1} --depth={2} . > NUL && git.exe --no-pager log --date=iso --skip={3} --max-count={4}";

        private readonly string gitPull = "cd {0} > NUL && git.exe pull --quiet {1} --depth={2} > NUL && git.exe --no-pager log --date=iso --skip={3} --max-count={4}";

        private readonly ICommitParser commitParser;

        private readonly ICommitViewerLog logger;

        public GitCLICommitFetcher(ICommitParser commitParser,
            ICommitViewerLog commitViewerLog)
        {
            this.commitParser = commitParser;
            this.logger = commitViewerLog;
        }

        private string GetProjectPath(string url)
        {
            Uri uri = new Uri(url);
            string path = Regex.Replace(uri.PathAndQuery, ".git", "", RegexOptions.IgnoreCase);
            return Path.GetTempPath() + path.Replace('/', '\\');
        }

        private async Task<string> ExecuteShellCommand(string cmd)
        {
            // creating temp bat file 
            string tempfilePath = Path.GetTempPath() + Guid.NewGuid().ToString() + ".bat";
            File.WriteAllText(tempfilePath, cmd);

            // executing temp bat file 
            ProcessStartInfo procInfo = new ProcessStartInfo("cmd", $"/C {tempfilePath}");
            procInfo.RedirectStandardOutput = true;
            procInfo.UseShellExecute = false;
            procInfo.CreateNoWindow = true;
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;

            // creating process and reading STDOUT
            logger.Debug($"Starting new process to fetch commits with cmd: {cmd}");
            Process proc = new Process();
            proc.StartInfo = procInfo;
            proc.Start();
            return await proc.StandardOutput.ReadToEndAsync();
        }

        public async Task<IEnumerable<CommitDTO>> GetCommits(string url, int page = 0, int per_page = 10)
        {
            try
            {
                // if the project was already cloned let's use pull... it's faster 
                bool gitPull = true;
                string projectPath = this.GetProjectPath(url);

                if (!Directory.Exists(projectPath))
                {
                    logger.Debug($"Preparing to clone new project {url}");
                    Directory.CreateDirectory(projectPath);
                    gitPull = false;
                }
                else
                {
                    logger.Debug($"The project {url} was already cloned.");
                }

                // formating command with input parameters
                string command = string.Format(gitPull ? this.gitPull : this.gitClone,
                    projectPath,
                    url,
                    page * per_page + per_page,
                    page * per_page,
                    per_page);

                string gitLog = await this.ExecuteShellCommand(command);

                // TODO: remove this HACK... the STDOUT must not contain the command that was executed
                gitLog = gitLog.Remove(0, gitLog.IndexOf("commit "));

                return string.IsNullOrWhiteSpace(gitLog) ? new List<CommitDTO>() : this.commitParser.Parse(gitLog);
            }
            catch (Exception exp)
            {
                logger.Error($"Unexpected error while getting and parsing commits in {nameof(GitCLICommitFetcher)}", exp);
                throw new CommitFetchingOperationException("Unexpected error while getting and parsing commits. Check inner exception.", exp);
            }
        }
    }
}
