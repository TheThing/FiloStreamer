using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiloStreamer.Decklink
{
    public class DecklinkDevice
    {
        private string _name;
        private List<DecklinkDeviceMode> _modes;

        public DecklinkDevice()
        {
            _modes = new List<DecklinkDeviceMode>();
        }

        public List<DecklinkDeviceMode> Modes
        {
            get { return _modes; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
