using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiloStreamer.FFmpeg
{
    public class FFmpegProcess : IDisposable
    {
        private Process _process;
        private string _name;
        private bool _isRunning;
        private TaskCompletionSource<bool> _outputTask;
        private TaskCompletionSource<bool> _errorTask;

        public event DataReceivedEventHandler OutputDataReceived = delegate { };
        public event EventHandler ProcessExited = delegate { };

        public FFmpegProcess()
            : this("ffmpeg")
        {
        }

        public FFmpegProcess(string name)
        {
            _name = name;
            _isRunning = false;
        }

        private void _process_Exited(object sender, EventArgs e)
        {
            _process.Dispose();
            _isRunning = false;
            ProcessExited(this, e);
        }

        private void _process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                _errorTask.TrySetResult(true);
                return;
            }
            for (int i = 0; i < e.Data.Length; i += 120)
                Console.WriteLine("[{0}]> {1}", _name, e.Data.Substring(i, Math.Min(120, e.Data.Length - i)));
            OutputDataReceived(this, e);
        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                _outputTask.TrySetResult(true);
                return;
            }
            for (int i = 0; i < e.Data.Length; i += 120)
                Console.WriteLine("[{0}]| {1}", _name, e.Data.Substring(i, Math.Min(120, e.Data.Length - i)));
            OutputDataReceived(this, e);
        }

        public void Run(string arguments)
        {
            _process = new Process();
            _outputTask = new TaskCompletionSource<bool>();
            _errorTask = new TaskCompletionSource<bool>();
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardOutput = true;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardInput = true;
            _process.StartInfo.CreateNoWindow = true;
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ErrorDataReceived += _process_ErrorDataReceived;
            _process.Exited += _process_Exited;
            _process.StartInfo.FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
            _process.EnableRaisingEvents = true;

            _process.StartInfo.Arguments = arguments;
            _process.Start();
            _isRunning = true;

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
        }

        public async void Stop()
        {
            if (_process == null)
                return;

            if (!_isRunning)
                return;

            var process = _process;
            _isRunning = false;
            process.StandardInput.Write('q');

            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                if (!process.HasExited)
                {
                    process.Kill();
                    process.Close();
                }
            }
            catch (InvalidOperationException) { }
            catch (Exception error)
            {
                Console.WriteLine(error);
            }
            finally
            {
                if (process == _process)
                    _process_Exited(this, new EventArgs());
                else
                    process.Dispose();
            }
        }

        public void WaitForExit()
        {
            _errorTask.Task.Wait();
            _outputTask.Task.Wait();
        }

        public async void RunAsync(string arguments)
        {
            var task = new TaskCompletionSource<bool>();

            Run(arguments);
            _process.Exited += (a, b) => {
                task.TrySetResult(true);
            };
            await _errorTask.Task;
            await _outputTask.Task;
        }

        public void Dispose()
        {
            _process.Kill();
            _process.Close();
            _process.Dispose();
        }
    }
}
