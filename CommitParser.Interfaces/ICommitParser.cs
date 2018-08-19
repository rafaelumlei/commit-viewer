using CommitViewer.Model.DTOs;
using System.Collections.Generic;

namespace CommitParser.Interfaces
{
    public interface ICommitParser
    {
        IEnumerable<CommitDTO> Parse(string gitLog);
    }
}
