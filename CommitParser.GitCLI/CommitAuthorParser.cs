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
    public class CommitAuthorParser : ICommitAuthorParser
    {
        private static readonly Regex commitAuthorRegex = new Regex(@"^Author: (<name>.*) \<(<email>.*)\>$");

        public (string, string) Parse(string line)
        {
            Match m = commitAuthorRegex.Match(line);

            if (m.Success &&
                !string.IsNullOrWhiteSpace(m.Groups["name"]?.Value) &&
                !string.IsNullOrWhiteSpace(m.Groups["email"]?.Value))
                return (m.Groups["name"]?.Value, m.Groups["email"]?.Value);
            else
                throw new InvalidTokenException(nameof(CommitIdParser), $"Invalid commit author input {line ?? "null"}");
        }
    }
}
