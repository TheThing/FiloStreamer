using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using FiloStreamer.Streamer;
using FiloStreamer.Recorder;
using FiloStreamer.Logger;
using System.Collections.Specialized;

namespace FiloStreamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LocalStreamer _streamer;
        private LocalRecorder _recorder;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            LogWriter.CurrentWriter.List.CollectionChanged += List_CollectionChanged;
            _streamer = new LocalStreamer(this.Dispatcher);
            _recorder = new LocalRecorder(this.Dispatcher);
            _streamer.Stopped += _streamer_Stopped;
            this.DataContext = _streamer;
            groupBoxRecord.DataContext = _recorder;
            listDevices.ItemsSource = Decklink.Manager.Devices;
        }

        private void _streamer_Stopped(object sender, EventArgs e)
        {
            vlcPlayer.Stop();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _streamer.Stop();
            _recorder.Stop();
            Properties.Settings.Default.Save();
        }

        private void List_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            textboxLastLogLine.Text = LogWriter.CurrentWriter.LastLine;
        }

        private void buttonLive_Click(object sender, RoutedEventArgs e)
        {
            if (!_streamer.IsLive)
            {
                Properties.Settings.Default.Save();
                Logger.LogWriter.CurrentWriter.List.Clear();
                _streamer.Run();
                Dispatcher.Invoke(async() =>
                {
                    try
                    {
                        string path = string.Format("udp://@{0}", Properties.Settings.Default.networkLocal);
                        await vlcPlayer.Stop();
                        vlcPlayer.LoadMedia(new Uri(path));
                        vlcPlayer.Play();
                    }
                    catch (Exception error)
                    {
                        Console.WriteLine(error);
                    }
                });
            }
            else
            {
                _streamer.Stop();
            }
        }

        private void ButtonLog_Click(object sender, RoutedEventArgs e)
        {
            new LogWindow().Show();
        }

        private void buttonRecord_Click(object sender, RoutedEventArgs e)
        {
            if (!_recorder.IsRecording)
            {
                _recorder.Run();
            }
            else
            {
                _recorder.Stop();
            }
        }
        
        private void vlcPlayer_VideoSourceChanged(object sender, EventArgs e)
        {
            //vlcPlayer.Volume = 0;
            //vlcPlayer.IsMute = true;
        }
    }
}
