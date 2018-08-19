using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitFetcher.Interfaces.Exceptions
{
    public class CommitFetchingOperationAborted : Exception
    {
        public CommitFetchingOperationAborted(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
