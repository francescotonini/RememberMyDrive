/*
 This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org/>
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace RememberMyKey
{
    class Program
    {
        private static Thread trayIconThread; //In order to work properly, place the tray icon on a separate thread.

        //Adding some stuff for hide the Console window
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private static Int32 showWindow = 0;

        private static void Main()
        {
            //Create an event which will be fired when user shutdown or logoff/console closing
            SystemEvents.SessionEnding += SystemEvents_SessionEnding;

            //Start tray icon thread
            trayIconThread = new Thread(AddToSystemTray);
            trayIconThread.Start();

            //Hide the Console window. It can be opened via the Tray Icon
            ShowWindow(ThisConsole, showWindow);
        }

        /// <summary>
        /// Fired when session shuts down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (USBDevice.GetAllRemovableDevices().Length != 0)
            {
                Thread preShutdownCheck = new Thread(PreShutdownCheck);
                preShutdownCheck.Start();

                MessageBox.Show("Ehi! You forgot your drives on the PC.", "REMEMBER YOUR DRIVE!!!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Check if drives are connected when messagebox "Ehi! You forgot..." is displayed
        /// </summary>
        private static void PreShutdownCheck()
        {
            // Active wait
            do
            {
                Thread.Sleep(500);
            } while (USBDevice.GetAllRemovableDevices().Length != 0);

            // All keys disconnected... Time to shutdown!
            Application.Exit();
            Environment.Exit(1);
        }

        /// <summary>
        /// Add the program to the system tray
        /// </summary>
        private static void AddToSystemTray()
        {
            NotifyIcon trayIcon = new NotifyIcon();
            
            //Assign icon file to tray icon + contextmenustrip + name
            trayIcon.Text = "Remember my drive";
            trayIcon.Icon = RememberMyKey.Properties.Resources.icon;
            trayIcon.ContextMenuStrip = new ContextMenuStrip();

            //Info button
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem());
            trayIcon.ContextMenuStrip.Items[0].Text = "Info";
            trayIcon.ContextMenuStrip.Items[0].Click += TrayIconInfoButton_Click;

            //Exit button
            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem());
            trayIcon.ContextMenuStrip.Items[1].Text = "Exit";
            trayIcon.ContextMenuStrip.Items[1].Click += TrayIconExitButton_Click;

            //Add event click
            trayIcon.MouseClick += trayIcon_MouseClick;

            //Make it visible
            trayIcon.Visible = true;
            Application.Run();
        }

        /// <summary>
        /// Info button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TrayIconInfoButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Remember my drive! Brought to you by Francesco Tonini :)", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Exit button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void TrayIconExitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(1);
        }

        /// <summary>
        /// Mouse click on tray 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) 
            {
                USBDevice[] devices = USBDevice.GetAllRemovableDevices();
                string toShow = string.Format("You have {0} removable drives connected", devices.Length);
                foreach (USBDevice device in devices)
                {
                    toShow += string.Format("\n{0} -> {1}", device.DriveLabel, device.Name);
                }
                MessageBox.Show(toShow, "Remember my drive", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
