using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitViewer.Model.DTOs
{
    public class CommitDTO
    {
        public string Id { get; set; }

        public string AuthorName { get; set; }

        public string AuthorEmail { get; set; }

        public DateTimeOffset Date { get; set; }

        public string Message { get; set; }
    }
}
