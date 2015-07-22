using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiloStreamer.Properties;
using FiloStreamer.Encoder;
using System.Windows.Threading;

namespace FiloStreamer.Streamer
{
    class LocalStreamer : BasicEncoder
    {
        private bool _isLive;

        public event EventHandler Stopped = delegate { };

        public LocalStreamer(Dispatcher dispatcher)
            : base(dispatcher, "Streamer")
        {
            _isLive = false;
        }

        public void Run()
        {
            var decklink = Decklink.Manager.Devices[Settings.Default.device];
            string device = string.Format("-i \"{0}@{1}\"", decklink.Name, decklink.Modes[Settings.Default.deviceMode].Index);
            string video = string.Format("-vcodec libx264 -b:v {0}k -preset {1} {2}",
                                         Settings.Default.videoBitrate,
                                         Settings.Default.videoPreset,
                                         Settings.Default.videoExtra);
            string audio = string.Format("-acodec {0} -b:a {1}k {2}",
                                         Settings.Default.audioCodec,
                                         Settings.Default.audioBitrate,
                                         Settings.Default.audioExtra);
            string network = string.Format("-f mpegts \"udp://{0}\"", Settings.Default.networkLocal);
            string arguments = string.Format("-f decklink {0} -threads 6 {1} {2} {3}", device, video, audio, network);
            this.IsLive = true;
            base.Run(arguments);
        }

        public void Stop()
        {
            _process.Stop();
        }

        protected override void _process_ProcessExited(object sender, EventArgs e)
        {
            base._process_ProcessExited(sender, e);
            this.IsLive = false;
            Stopped(this, new EventArgs());
        }

        public bool IsLive
        {
            get { return _isLive; }
            set
            {
                _isLive = value;
                RunPropertyChanged("IsLive");
            }
        }
    }
}
