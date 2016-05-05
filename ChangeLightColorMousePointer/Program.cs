using Q42.HueApi;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeLightColorMousePointer
{
    class Program
    {
        static  void Main(string[] args)
        {
            //set up bridge connection
            IBridgeLocator locator = new HttpBridgeLocator();
            string ip = "192.168.2.53";
            ILocalHueClient client = new LocalHueClient(ip);
            var appKey = "GrAewDZRcvmS4GmJt-jSgmZ7bBOF0VQyEN5WD14b";
            client.Initialize(appKey);
            //create command variable
            var command = new LightCommand();
            //turn off my 2 white lights
            command.TurnOff();
            client.SendCommandAsync(command, new List<string> { "4", "5" });

            while (true)
            {
                Thread.Sleep(500);
                //get cursor position current color
                Color c = Win32.GetPixelColor(Cursor.Position.X, Cursor.Position.Y);
                Console.WriteLine(c.ToString());
                //change lights based on grabbed color
                command.TurnOn().SetColor(c.R,c.G,c.B);
               // client.SendCommandAsync(command);
                client.SendCommandAsync(command, new List<string> { "1","2","3" });

            }
        }
    }
    //this is the get color of xy method
    public sealed class Win32
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        static public System.Drawing.Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                         (int)(pixel & 0x0000FF00) >> 8,
                         (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
    }
}
