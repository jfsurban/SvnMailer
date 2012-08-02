using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;

namespace SvnMailer {
    class Options : CommandLineOptionsBase {
        public Options() {
            Port = 25;
        }

        [Option("v", "server", Required = true, HelpText = "SMTP server.")]
        public String Server { get; set; }

        [Option("p", "port", HelpText = "SMTP port.")]
        public int Port { get; set; }

        [Option("n", "svn", Required = true, HelpText = "Subversion path.")]
        public String SvnPath { get; set; }

        [Option("a", "path", Required = true, HelpText = "SVN Repository path.")]
        public String RepositoryPath { get; set; }

        [Option("e", "revision", Required = true, HelpText = "SVN revision.")]
        public int Revision { get; set; }

        [Option("f", "from", Required = true, HelpText = "From email.")]
        public String From { get; set; }

        [OptionList("t", "to", Separator = ',', Required = true, HelpText = "Email recipients (comma separated).")]
        public IList<String> To { get; set; }

        [HelpOption("h", "help")]
        public string GetUsage() {
            return HelpText.AutoBuild(this,
              (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }
    }
}
