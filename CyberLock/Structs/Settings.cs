using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberLock.Structs
{
    class Settings
    {
        public object KeyCode { get; set; }
        public bool Ctrl { get; set; }
        public bool Shift { get; set; }

        public void SetKeyCode(string key)
        {
            KeysConverter kc = new KeysConverter();
            var Object = kc.ConvertFromString(key);
            KeyCode = Object;
        }
    }
}
