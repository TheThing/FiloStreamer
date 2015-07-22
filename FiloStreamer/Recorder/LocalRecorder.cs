using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiloStreamer.Properties;
using FiloStreamer.Encoder;
using System.Windows.Threading;

namespace FiloStreamer.Recorder
{
    class LocalRecorder : BasicEncoder
    {
        private bool _isRecording;

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
            string arguments = string.Format("-y -threads 4 {0} -map 0 {1} {2} \"test.mp4\"", network, video, audio);
            this.IsRecording = true;
            base.Run(arguments);
        }

        public void Stop()
        {
            _process.Stop();
        }

        protected override void _process_ProcessExited(object sender, EventArgs e)
        {
            base._process_ProcessExited(sender, e);
            this.IsRecording = false;
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
