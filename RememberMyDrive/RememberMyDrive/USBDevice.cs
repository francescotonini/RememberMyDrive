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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RememberMyKey
{
    /// <summary>
    /// Rappresent a single USB Device
    /// </summary>
    public class USBDevice
    {
        private DriveInfo drive;

        /// <summary>
        /// Rappresent the label name of the device (ex. "Luca's USB")
        /// </summary>
        public string Name
        {
            get { return this.drive.VolumeLabel; }
        }

        /// <summary>
        /// Indicates the free space available to user
        /// </summary>
        public long FreeSpace
        {
            get { return this.drive.AvailableFreeSpace; }
        }

        /// <summary>
        /// Indicates the total free space available (not the par value)
        /// </summary>
        public long TotaleSpace
        {
            get { return this.drive.TotalFreeSpace; }
        }

        /// <summary>
        /// Get the drive label (ex. "D:")
        /// </summary>
        public string DriveLabel
        {
            get { return this.drive.Name; }
        }

        /// <summary>
        /// Initialize a USBDevice object
        /// </summary>
        /// <param name="drive">The actual drive (MUST be a Removable)</param>
        public USBDevice(DriveInfo drive)
        {
            if (drive.DriveType == DriveType.Removable && drive.IsReady) this.drive = drive; //the IsReady property is necessary when suddently a USB devices has been disconnected and the drive is unavailable but still recognized
        }

        /// <summary>
        /// Get an array of removable devices
        /// </summary>
        /// <returns>An array of USBDevice with ONLY Removable devices</returns>
        public static USBDevice[] GetAllRemovableDevices()
        {
            List<USBDevice> rDevices = new List<USBDevice>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    rDevices.Add(new USBDevice(drive));
                }
            }
            return rDevices.ToArray();
        }
    }
}
