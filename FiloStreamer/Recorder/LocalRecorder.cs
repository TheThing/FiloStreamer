using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FiloStreamer.Properties;
using FiloStreamer.Encoder;
using System.Windows.Threading;

namespace FiloStreamer.Recorder
{
    class LocalRecorder : BasicEncoder
    {
        private bool _isRecording;
        private string _file;

        public LocalRecorder(Dispatcher dispatcher)
            : base(dispatcher, "Rec")
        {
            _isRecording = false;
        }

        public void Run()
        {
            string video = string.Format("-vcodec copy");
            string audio = string.Format("-acodec copy");
            string network = string.Format("-i udp://{0}", Settings.Default.networkLocal);
            var time = DateTime.Now;
            File = Path.Combine(Properties.Settings.Default.settingsRecordingFolder,
                                       string.Format("{0}-{1:00}-{2:00}_{3:00}-{4:00}-{5:00}.mp4",
                                                     time.Year,
                                                     time.Month,
                                                     time.Day,
                                                     time.Hour,
                                                     time.Minute,
                                                     time.Second));
            string arguments = string.Format("-y -threads 4 {0} -map 0 {1} {2} \"{3}\"", network, video, audio, File);
            this.IsRecording = true;
            base.Run(arguments);
        }

        public void Stop()
        {
            _process.Stop();
            File = "";
        }

        protected override void _process_ProcessExited(object sender, EventArgs e)
        {
            base._process_ProcessExited(sender, e);
            this.IsRecording = false;
        }

        public string File
        {
            get { return _file; }
            set
            {
                _file = value;
                RunPropertyChanged("File");
            }
        }

        public bool IsRecording
        {
            get { return _isRecording; }
            set
            {
                _isRecording = value;
                RunPropertyChanged("IsRecording");
            }
        }
    }
}
