using CommitFetcher.Interfaces;
using CommitFetcher.Interfaces.Exceptions;
using CommitParser.Interfaces;
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

        private ICommitParser commitParser;

        public GitCLICommitFetcher(ICommitParser commitParser,
            string gitCloneCommand = "",
            string gitPullCommand = ""
            )
        {
            if (!string.IsNullOrEmpty(gitCloneCommand))
            {
                this.gitClone = gitCloneCommand;
            }

            if (!string.IsNullOrEmpty(gitPullCommand))
            {
                this.gitPull = gitPullCommand;
            }

            this.commitParser = commitParser;
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
            string path = System.Environment.GetEnvironmentVariable("path");

            // creating process and reading STDOUT
            Process proc = new Process();
            proc.StartInfo = procInfo;
            proc.Start();
            return await proc.StandardOutput.ReadToEndAsync();
        }

        public async Task<IEnumerable<CommitDTO>> GetCommits(string url, int skipToken = 0, int top = 10)
        {
            try
            {
                // if the project was already cloned let's use pull... it's faster 
                bool gitPull = true;
                string projectPath = this.GetProjectPath(url);

                if (!Directory.Exists(projectPath))
                {
                    Directory.CreateDirectory(projectPath);
                    gitPull = false;
                }

                // formating command with input parameters
                string command = string.Format(gitPull ? this.gitPull : this.gitClone,
                    projectPath,
                    url,
                    skipToken + top,
                    skipToken,
                    top);

                
                string gitLog = await this.ExecuteShellCommand(command);

                // TODO: remove this HACK... the STDOUT must not contain the command that was executed
                gitLog = gitLog.Remove(0, gitLog.IndexOf("commit "));

                return string.IsNullOrWhiteSpace(gitLog) ? new List<CommitDTO>() : this.commitParser.Parse(gitLog);
            }
            catch (Exception exp)
            {
                throw new CommitFetchingOperationAborted("Unexpected error while getting and parsing commits. Check inner exception.", exp);
            }
        }
    }
}
