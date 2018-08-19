using CommitFetcher.Interfaces;
using CommitViewer.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitCLICommitFetcher
{
    public class GitCLICommitFetcher : ICommitFetcher<CommitDTO>
    {

        public GitCLICommitFetcher()
        {

        }

        public IEnumerable<CommitDTO> GetCommits()
        {
            throw new NotImplementedException();
        }
    }
}
