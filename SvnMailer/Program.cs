using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using CommandLine;
using System.Diagnostics;
using System.IO;

namespace SvnMailer {
    class Program {
        static void Main(String[] args) {

            Options options = new Options();
            if (CommandLineParser.Default.ParseArguments(args, options)) {

                Process svnlookInfo = new Process();
                svnlookInfo.StartInfo.FileName = Path.Combine(options.SvnPath, "svnlook.exe");
                svnlookInfo.StartInfo.Arguments = String.Format("info {0} -r {1}", options.RepositoryPath, options.Revision);
                svnlookInfo.StartInfo.UseShellExecute = false;
                svnlookInfo.StartInfo.RedirectStandardOutput = true;
                svnlookInfo.Start();
                String commitInfo = svnlookInfo.StandardOutput.ReadToEnd();
                svnlookInfo.WaitForExit();

                String[] infos = commitInfo.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                String author = infos[0];
                String datestamp = infos[1];
                String message = "";
                if (infos.Length >= 4) {
                    message = infos[3] ?? "";
                }

                Process svnlookChanges = new Process();
                svnlookChanges.StartInfo.FileName = Path.Combine(options.SvnPath, "svnlook.exe");
                svnlookChanges.StartInfo.Arguments = String.Format("changed {0} -r {1}", options.RepositoryPath, options.Revision);
                svnlookChanges.StartInfo.UseShellExecute = false;
                svnlookChanges.StartInfo.RedirectStandardOutput = true;
                svnlookChanges.Start();
                String changed = svnlookChanges.StandardOutput.ReadToEnd();
                svnlookChanges.WaitForExit();

                String subject = String.Format("Commit: {0} Author: {1} Message: {2} ",
                    options.Revision, author, message.Substring(0, Math.Min(80, message.Length)));
                String body = String.Format("Full Message:\n{0}\n\nTimestamp:\n{1}\n\nChanges:\n{2}",
                    message, datestamp, changed);
                // Create a message and set up the recipients.
                MailMessage mail = new MailMessage(
                   options.From,
                   String.Join(",", options.To),
                   subject,
                   body);

                SmtpClient client = new SmtpClient(options.Server, options.Port);

                try {
                    client.Send(mail);
                } catch (Exception ex) {
                    Console.WriteLine("Failed to send mail: {0}", ex.ToString());
                }
            }
        }
    }
}
