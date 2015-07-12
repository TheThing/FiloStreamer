using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiloStreamer.FFmpeg
{
    public class FFmpegProcess
    {
        Process _process;

        public event DataReceivedEventHandler OutputDataReceived = delegate { };

        public FFmpegProcess()
        {
            _process = new Process();
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_OutputDataReceived;
            _process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            _process.EnableRaisingEvents = true;
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
                return;
            for (int i = 0; i < e.Data.Length; i += 120)
                Console.WriteLine("[ffmpeg] " + e.Data.Substring(i, Math.Min(120, e.Data.Length - i)));
            OutputDataReceived(this, e);
        }

        public void Run(string arguments)
        {
            _process.StartInfo.Arguments = arguments;
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
        }

        public async Task RunAsync(string arguments)
        {
            var task = new TaskCompletionSource<bool>();

            Run(arguments);
            _process.Exited += (a, b) => { task.SetResult(true); };
            _process.WaitForExit();

            await task.Task;
        }
    }
}
