using CommitParser.Interfaces;
using System.Collections.Generic;
using CommitViewer.Model.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CommitParser.GitCLI
{
    public class GitCLICommitParser : ICommitParser
    {
        private readonly string commitSeparator;

        private readonly ICommitIdParser commitIdParser;

        private readonly ICommitAuthorParser commitAuthorParser;

        private readonly ICommitDateParser commitDateParser;

        private readonly ICommitMessageParser commitMessageParser;

        public GitCLICommitParser(ICommitIdParser commitIdParser,
            ICommitAuthorParser commitAuthorParser,
            ICommitDateParser commitDateParser,
            ICommitMessageParser commitMessageParser,
            string commitSeparator = "\n")
        {
            this.commitSeparator = commitSeparator;
        }

        private CommitDTO Parse(IEnumerable<string> commit)
        {
            try
            {

            }
            catch (Exception exp)
            {
                // TODO: add logging
            }

            return null;
        }

        public IEnumerable<CommitDTO> Parse(string gitLog)
        {
            CommitDTO[] commits = null;

            if (!string.IsNullOrWhiteSpace(gitLog))
            {
                string[] lines = gitLog.Split(new string[] { this.commitSeparator }, StringSplitOptions.RemoveEmptyEntries);

                
                commits = new CommitDTO[lines.Length / 6];

                Parallel.For(0, commits.Length, (c) =>
                {
                    var parsedCommit = this.Parse(lines.Skip(c * 6).Take(6));

                    if (parsedCommit != null)
                    {
                        commits[c] = parsedCommit;
                    }
                    else
                    {
                        // TODO: add logging 
                    }
                });
            }

            return commits ?? new CommitDTO[] { };
        }
    }
}
