using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitViewer.Console
{
    class ConsoleOptions
    {
        [Option("url", HelpText = "Git repository url.", Required = false)]
        public string Url { get; set; } = "https://github.com/rafaelumlei/tsoa.git";

        [Option("page", HelpText = "Commits page to get.", Required = false)]
        public int Page { get; set; } = 0;

        [Option("per-page", HelpText = "Number of commits per page.", Required = false)]
        public int PerPage { get; set; } = 10;
    }
}
