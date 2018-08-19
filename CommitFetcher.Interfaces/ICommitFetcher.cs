using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitFetcher.Interfaces
{
    public interface ICommitFetcher<T>
    {
        Task<IEnumerable<T>> GetCommits(string url, int skipToken = 0, int top = 10);
    }
}
