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
        [Option('u', "url", HelpText = "Git repository url.", Required = false)]
        public string Url { get; set; } = "https://github.com/rafaelumlei/tsoa.git";

        [Option('s', "skip", HelpText = "Number of commits to skip.", Required = false)]
        public int Skip { get; set; } = 0;

        [Option('t', "top", HelpText = "Number of commits to get.", Required = false)]
        public int Top { get; set; } = 10;
    }
}
