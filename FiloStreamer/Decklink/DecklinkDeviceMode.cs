using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiloStreamer.Decklink
{
    public class DecklinkDeviceMode
    {
        private string _index;
        private string _description;

        public string Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }

            set
            {
                _description = value;
            }
        }
    }
}
