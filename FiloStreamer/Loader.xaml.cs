using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FiloStreamer.Decklink;
using FiloStreamer.Logger;

namespace FiloStreamer
{
    /// <summary>
    /// Interaction logic for Loader.xaml
    /// </summary>
    public partial class Loader : Window
    {
        private readonly BackgroundWorker _worker;
        private bool _expanded;

        public Loader()
        {
            _worker = new BackgroundWorker();
            LogWriter.CurrentWriter = new LogWriter { Dispatcher = Dispatcher };
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            _expanded = false;

            Console.SetOut(LogWriter.CurrentWriter);
            LogWriter.CurrentWriter.List.CollectionChanged += List_CollectionChanged;

            itemsList.ItemsSource = LogWriter.CurrentWriter.List;

            _worker.WorkerReportsProgress = true;
            _worker.DoWork += _worker_DoWork;
            _worker.RunWorkerCompleted += _worker_RunWorkerCompleted;
            _worker.ProgressChanged += _worker_ProgressChanged;
            _worker.RunWorkerAsync();
        }

        private void List_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            string line = e.NewItems[e.NewItems.Count - 1] as string;
            textboxLastLogLine.Text = line;
        }

        private void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            loadingControl.ContentText = e.UserState.ToString();
        }

        private async void _worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _worker.ReportProgress(0, "1/3\nInitializing");
            await Manager.Init();
            _worker.ReportProgress(0, "2/3\nDevices");
            await Manager.LoadDevices();
            _worker.ReportProgress(0, "3/3\nModes");
            await Manager.LoadDeviceModes();
        }

        private void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void Expand(bool expand)
        {
            _expanded = expand;
            if (_expanded)
            {
                this.ResizeMode = ResizeMode.CanResizeWithGrip;
                scroller.Visibility = Visibility.Visible;
                this.Height = 150 + 100;
            }
            else
            {
                this.ResizeMode = ResizeMode.NoResize;
                scroller.Visibility = Visibility.Collapsed;
                this.Height = 150;
            }
        }

        private void ButtonExpand_Click(object sender, RoutedEventArgs e)
        {
            Expand(!_expanded);
        }
    }
}
