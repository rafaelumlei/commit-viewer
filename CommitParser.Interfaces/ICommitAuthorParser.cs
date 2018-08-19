using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitParser.Interfaces
{
    public interface ICommitAuthorParser
    {
        (string name, string email) Parse(string line);
    }
}
