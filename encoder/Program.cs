using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder
{
    class Program
    {

        private static string CurrentDirectory => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private const string DefaultCommandLine = "-c:v h264_cuvid -i \"{0}\" -c:v hevc_nvenc -profile:v main -pixel_format yuv420p -preset medium -rc:v vbr_hq -cq:v 19 -b:v 10000k -maxrate:v 16000k -c:a copy -map 0 \"{1}\"";

        static void Main(string[] args)
        {
            //setup ffmpeg command line
            string commandLine = DefaultCommandLine;
            var customPath = Path.Combine(CurrentDirectory, "custom.txt");
            if (File.Exists(customPath))
            {
                commandLine = File.ReadAllText(customPath).Trim();
            }

            //create recoded folder if it does not exist
            var targetPath = Path.Combine(CurrentDirectory, "recoded");
            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            //transcode all the things
            foreach(var file in Directory.EnumerateFiles(CurrentDirectory))
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
