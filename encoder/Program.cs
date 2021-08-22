using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace encoder
{
    class Program
    {

        private static string CurrentDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private const string DefaultCommandLine = "-i \"{0}\" -c:v hevc_nvenc -preset medium -rc:v vbr_hq -cq:v 23 -b:v 0 -c:a copy -map 0 \"{1}\"";

        static void Main(string[] args)
        {
            //default working directory
            string workingDirectory = CurrentDirectory;

            //use -i if available
            if(args.Contains("-i"))
            {
                var idx = Array.IndexOf(args, "-i") + 1;
                workingDirectory = args[idx].Trim('\'', '"');
            }

            //setup ffmpeg command line
            string commandLine = DefaultCommandLine;
            var customPath = Path.Combine(CurrentDirectory, "custom.txt");
            if (File.Exists(customPath))
            {
                commandLine = File.ReadAllText(customPath).Trim();
            }

            //create recoded folder if it does not exist
            var targetPath = Path.Combine(workingDirectory, "recoded");

            //use -o if available
            if (args.Contains("-o"))
            {
                var idx = Array.IndexOf(args, "-o") + 1;
                targetPath = args[idx].Trim('\'', '"');
            }

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            Console.WriteLine("Working directory: " + workingDirectory);
            Console.WriteLine("Target directory: " + targetPath);

            Thread.Sleep(5000);

            //transcode all the things
            foreach(var file in Directory.EnumerateFiles(workingDirectory))
            {
                string extension = Path.GetExtension(file);
                if (!(string.Equals(extension, ".mp4", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".mov", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".mkv", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".wmv", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".mpg", StringComparison.OrdinalIgnoreCase)))
                    continue;

                string fileName = Path.GetFileNameWithoutExtension(file);
                string targetFileName = fileName + ".mp4";

                if (File.Exists(Path.Combine(targetPath, targetFileName)))
                    continue;

                Console.WriteLine("Transcoding " + file);

                Process p = new Process();
                p.StartInfo.Arguments = string.Format(commandLine, Path.GetFileName(file), Path.Combine(targetPath, targetFileName));
                p.StartInfo.WorkingDirectory = CurrentDirectory;
                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.CreateNoWindow = false;

                p.Start();
                p.WaitForExit();

                Console.WriteLine("Transcoded " + targetFileName);
            }

            Console.WriteLine();

        }
    }
}
