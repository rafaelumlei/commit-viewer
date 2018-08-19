using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitParser.Interfaces.Exceptions
{
    public class InvalidTokenException : Exception
    {
        private readonly string field = null;

        public InvalidTokenException(string field, string message) : base(message)
        {
            this.field = field;
        }
    }
}
