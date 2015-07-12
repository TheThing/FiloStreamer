using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiloStreamer.FFmpeg;

namespace FiloStreamer.Decklink
{
    public static class Manager
    {
        public static List<DecklinkDevice> Devices;

        public static async Task Init()
        {
            Console.WriteLine("Creating list of devices.");
            Devices = new List<DecklinkDevice>();
        }

        public static async Task LoadDevices()
        {
            Console.WriteLine("Running FFmpeg");
            var ffmpeg = new FFmpegProcess();

            ffmpeg.OutputDataReceived += Ffmpeg_ProcessDeviceLine;

            Console.WriteLine("");
            await ffmpeg.RunAsync("-f decklink -list_devices 1 -i dummy");
            Console.WriteLine("");

            for (int i = 0; i < Devices.Count; i++)
                Console.WriteLine("Found Decklink device #{0}: {1}", i + 1, Devices[i].Name);
        }

        private static void Ffmpeg_ProcessDeviceLine(object sender, System.Diagnostics.DataReceivedEventArgs b)
        {
            if (b.Data.Length > 0 && b.Data[0] == '[' && b.Data.IndexOf(']') > 1 && b.Data.IndexOf('\'') > b.Data.IndexOf(']'))
                Devices.Add(new DecklinkDevice { Name = b.Data.Substring(b.Data.IndexOf('\'') + 1, b.Data.LastIndexOf('\'') - b.Data.IndexOf('\'') - 1) });
        }

        public static async Task LoadDeviceModes()
        {
            for (int i = 0; i < Devices.Count; i++)
            {
                Console.WriteLine("Loading supported modes for device #{0} {1}", i + 1, Devices[i].Name);

                var ffmpeg = new FFmpegProcess();
                ffmpeg.OutputDataReceived += (a, b) => { Ffmpeg_ProcessDeviceModeLine(Devices[i], b.Data); };

                Console.WriteLine("");
                await ffmpeg.RunAsync(string.Format("-f decklink -list_formats 1 -i \"{0}\"", Devices[i].Name));
                Console.WriteLine("");
            }

            for (int i = 0; i < Devices.Count; i++)
            {
                Console.WriteLine("#{0} {1} supports {2} modes", i + 1, Devices[i].Name, Devices[i].Modes.Count);
            }
        }

        private static void Ffmpeg_ProcessDeviceModeLine(DecklinkDevice device, string line)
        {
            if (line.Length > 0 && line[0] == '[' && line.IndexOf(']') > 1 && line.IndexOf('\t') > line.IndexOf(']'))
            {
                string parsed = line.Substring(line.IndexOf(']') + 1);
                var lines = parsed.Trim().Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length != 2)
                {
                    Console.WriteLine("WARNING: Unable to process following mode: {0}", line);
                    return;
                }
                device.Modes.Add(new DecklinkDeviceMode {
                    Index = lines[0].Trim(),
                    Description = string.Format("{0}@{1}", lines[0].Trim(), lines[1].Trim())
                });
            }
        }
    }
}
