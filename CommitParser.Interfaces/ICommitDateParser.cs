using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitParser.Interfaces
{
    public interface ICommitDateParser
    {
        DateTimeOffset Parse(string line);
    }
}
