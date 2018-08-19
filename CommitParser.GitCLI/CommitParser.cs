using CommitParser.Interfaces;
using System.Collections.Generic;
using CommitViewer.Model.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;
using CommitViewer.Logger.Interfaces;

namespace CommitParser.GitCLI
{
    public class CommitParser : ICommitParser
    {
        private readonly string newLineSeparator;

        private ICommitViewerLog logger;

        private readonly ICommitIdParser commitIdParser;

        private readonly ICommitAuthorParser commitAuthorParser;

        private readonly ICommitDateParser commitDateParser;

        private readonly ICommitMessageParser commitMessageParser;

        public CommitParser(ICommitIdParser commitIdParser,
            ICommitAuthorParser commitAuthorParser,
            ICommitDateParser commitDateParser,
            ICommitMessageParser commitMessageParser,
            ICommitViewerLog commitViewerLogger,
            string newLineSeparator = "\n")
        {
            this.newLineSeparator = newLineSeparator;
            this.commitIdParser = commitIdParser;
            this.commitAuthorParser = commitAuthorParser;
            this.commitDateParser = commitDateParser;
            this.commitMessageParser = commitMessageParser;
            this.logger = commitViewerLogger;
        }

        private CommitDTO Parse(IEnumerable<string> commitLines)
        {
            try
            {
                if (commitLines?.Count() > 3)
                {
                    var commit = new CommitDTO()
                    {
                        Id = this.commitIdParser.Parse(commitLines.ElementAt(0))
                    };

                    var authorInfo = this.commitAuthorParser.Parse(commitLines.ElementAt(1));

                    commit.AuthorName = authorInfo.name;
                    commit.AuthorEmail = authorInfo.email;
                    commit.Date = commitDateParser.Parse(commitLines.ElementAt(2));
                    commit.Message = commitMessageParser.Parse(commitLines.Skip(3));

                    return commit;
                }
                else
                {
                    logger.Error("Insufficient information for a commit: \n" + string.Join("\n", commitLines));
                    return null;
                }
            }
            catch (Exception exp)
            {
                logger.Error("Commit parsing exception while parsing: \n" + string.Join("\n", commitLines), exp);
                return null;
            }
        }

        public IEnumerable<CommitDTO> Parse(string gitLog)
        {
            List<CommitDTO> commits = new List<CommitDTO>();

            if (!string.IsNullOrWhiteSpace(gitLog))
            {
                string[] lines = gitLog.Split(new string[] { this.newLineSeparator }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < lines.Length;)
                {
                    // taking commit lines: commit id, author, date, multi-line message 
                    var commitLines = lines.Skip(i).TakeWhile((l, index) => index < 3 || l.StartsWith("    "));
                    var parsedCommit = this.Parse(commitLines);

                    if (parsedCommit != null)
                    {
                        commits.Add(parsedCommit);
                    }

                    i += commitLines.Count();
                }
            }

            return commits;
        }
    }
}
