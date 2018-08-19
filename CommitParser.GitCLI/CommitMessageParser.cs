using CommitParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitParser.GitCLI
{
    public class CommitMessageParser : ICommitMessageParser
    {

        public string Parse(IEnumerable<string> lines)
        {
            // removing trailing 4 spaces before joining
            return string.Join("\n", lines?.Select(l => l.Remove(0, 4)) ?? new string[] { });
        }
    }
}
