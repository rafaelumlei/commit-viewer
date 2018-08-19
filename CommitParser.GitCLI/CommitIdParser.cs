using CommitParser.Interfaces;
using CommitParser.Interfaces.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommitParser.GitCLI
{
    class CommitIdParser : ICommitIdParser
    {

        private static readonly Regex commitIdRegex = new Regex(@"^commit (<commitId>[^ ])+");

        public string Parse(string line)
        {
            Match m = commitIdRegex.Match(line);

            if (m.Success && !string.IsNullOrWhiteSpace(m.Groups["commitId"]?.Value))
                return m.Groups["commitId"]?.Value;
            else
                throw new InvalidTokenException(nameof(CommitIdParser), $"Invalid commitId input {line ?? "null"}");
        }
    }
}
