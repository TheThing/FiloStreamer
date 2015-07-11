using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Vlc.DotNet.Core.Interops;
    
namespace FiloStreamer.Controls
{
    public class VlcControl : HwndHost
    {
        public Vlc.DotNet.Forms.VlcControl MediaPlayer { get; private set; }

        public VlcControl()
        {
            MediaPlayer = new Vlc.DotNet.Forms.VlcControl();
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Win32Interops.SetParent(MediaPlayer.Handle, hwndParent.Handle);
            return new HandleRef(this, MediaPlayer.Handle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (MediaPlayer == null)
                return;
            Win32Interops.SetParent(IntPtr.Zero, hwnd.Handle);
            try
            {
                //MediaPlayer.Dispose();
            }
            catch (System.AccessViolationException) { }
            finally
            {
                MediaPlayer = null;
            }
        }
    }
}