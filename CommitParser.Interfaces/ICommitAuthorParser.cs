using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitParser.Interfaces
{
    public interface ICommitAuthorParser
    {
        (string, string) Parse(string line);
    }
}
