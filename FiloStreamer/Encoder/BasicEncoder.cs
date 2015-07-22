using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using FiloStreamer.FFmpeg;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FiloStreamer.Encoder
{
    abstract class BasicEncoder : INotifyPropertyChanged
    {
        protected string _fps;
        protected string _time;
        protected string _bitrate;
        protected int _minFps;
        protected FFmpegProcess _process;
        protected Dispatcher _dispatcher;
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public BasicEncoder(Dispatcher dispatcher, string name)
        {
            _dispatcher = dispatcher;
            _process = new FFmpegProcess(name);
            _process.OutputDataReceived += _process_OutputDataReceived;
            _process.ProcessExited += _process_ProcessExited;

            this.FPS = "-";
            this.Time = "-";
            this.Bitrate = "-";
        }

        protected void Run(string arguments)
        {
            Console.WriteLine("Running FFmpeg with following arguments:");
            Console.WriteLine(arguments);
            _process.Run(arguments);
            _minFps = 0;
        }

        protected virtual void _process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            string temp = e.Data;
            if (_minFps <= 0)
            {
                int streamIndex = e.Data.IndexOf("Stream");
                int fpsIndex = e.Data.IndexOf(" fps");
                if (streamIndex > 0 && fpsIndex > 0 && fpsIndex > streamIndex)
                {
                    string findFps = e.Data.Remove(fpsIndex);
                    findFps = findFps.Remove(0, findFps.LastIndexOf(',') + 2);
                    int.TryParse(findFps, out _minFps);
                    RunPropertyChanged("StatusOk");
                }
            }
            var splitted = temp.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitted.Length >= 10 && splitted.Length <= 16 && splitted[2] == "fps" && splitted[8] == "time" && splitted[10] == "bitrate")
            {
                this.FPS = splitted[3];
                this.Time = splitted[9];
                this.Bitrate = splitted[11].Replace("kpbs", " kbps");
            }
            else
            {
                this.FPS = "-";
                this.Time = "-";
                this.Bitrate = "-";
            }
        }

        protected virtual void _process_ProcessExited(object sender, EventArgs e)
        {
            this.FPS = "-";
            this.Time = "-";
            this.Bitrate = "-";
            _minFps = 0;
        }

        public bool StatusOk
        {
            get {
                int fps = 0;
                if (int.TryParse(this._fps, out fps))
                    return fps >= _minFps;
                return true;
            }
        }

        public string FPS
        {
            get { return _fps; }
            set
            {
                _fps = value;
                RunPropertyChanged("FPS");
                RunPropertyChanged("StatusOk");
            }
        }

        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                RunPropertyChanged("Time");
            }
        }

        public string Bitrate
        {
            get { return _bitrate; }
            set
            {
                _bitrate = value;
                RunPropertyChanged("Bitrate");
            }
        }

        protected void RunPropertyChanged(string name)
        {
            _dispatcher.Invoke(() =>
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            });
        }
    }
}
