using CommitParser.Interfaces;
using CommitParser.Interfaces.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommitParser.GitCLI
{
    public class CommitDateParser : ICommitDateParser
    {

        private static readonly Regex commitDateRegex = new Regex(@"^Date:[ ]+(?<date>.*)$");

        public CommitDateParser()
        {
        }

        public DateTimeOffset Parse(string line)
        {
            Match m = commitDateRegex.Match(line);

            if (m.Success &&
                !string.IsNullOrWhiteSpace(m.Groups["date"]?.Value) &&
                DateTimeOffset.TryParse(m.Groups["date"]?.Value, out DateTimeOffset parsedDate))
                return parsedDate;
            else
                throw new InvalidTokenException(nameof(CommitDateParser), $"Invalid commit date input {line ?? "null"}");
        }
    }
}
