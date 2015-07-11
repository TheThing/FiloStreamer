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

namespace FiloStreamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            vlcControl.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
        }

        void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vlclib"));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var file = new FileInfo(@"K:\A_Digital_Media_Primer_For_Geeks-720p.webm");
            vlcControl.MediaPlayer.Play(file);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vlcControl.MediaPlayer.Stop();
            //vlcControl.Dispose();
        }
    }
}
