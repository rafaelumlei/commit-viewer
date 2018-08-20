using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitFetcher.Interfaces.Exceptions
{
    public class CommitFetchingOperationException : Exception
    {

        public CommitFetchingOperationException(string message) : base(message)
        {
        }

        public CommitFetchingOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
