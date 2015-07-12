using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FiloStreamer.Logger
{
    public class LogWriter : System.IO.TextWriter
    {
        public static LogWriter CurrentWriter;

        private string _buffer;
        private ObservableCollection<string> _list;
        private Dispatcher _dispatcher;

        public LogWriter()
        {
            _buffer = "";
            _list = new ObservableCollection<string>();
        }

        public override void Write(char value)
        {
            _buffer += value;
            Check();
        }

        public override void Write(string value)
        {
            _buffer += value;
            Check();
        }

        private async void Check()
        {
            await Task.Run(() =>
            {
                lock (_list)
                {
                    int index = _buffer.IndexOf('\n');
                    if (index > 0)
                    {
                        _dispatcher.Invoke(() =>
                        {
                            _list.Add(_buffer.Remove(index).Replace("\r", ""));
                            //if (_list.Count > 50)
                            //    _list.RemoveAt(0);
                        });
                        _buffer = _buffer.Remove(0, index + 1);
                    }
                }
            });

        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }

        public Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set { _dispatcher = value; }
        }

        public ObservableCollection<string> List
        {
            get { return _list; }
        }
    }
}
