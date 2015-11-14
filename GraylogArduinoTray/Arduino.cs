using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Management;

namespace GraylogArduinoTray
{
    class Arduino
    {
        int baudrate;
        SerialPort port;

        public Arduino(int inBaudrate)
        {
            baudrate = inBaudrate;

            // try to connect to Arduino/set port
            connect();

            // PnP stands for plug and play
            string queryStr = "SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            ManagementEventWatcher deviceCreationWatcher = new ManagementEventWatcher(queryStr);
            deviceCreationWatcher.EventArrived += new EventArrivedEventHandler(DeviceChangeEventReceived);
            deviceCreationWatcher.Start();

            queryStr = "SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_PnPEntity'";
            ManagementEventWatcher deviceDeletionWatcher = new ManagementEventWatcher(queryStr);
            deviceDeletionWatcher.EventArrived += new EventArrivedEventHandler(DeviceChangeEventReceived);
            deviceDeletionWatcher.Start();
        }

        private void DeviceChangeEventReceived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject pnp_device = (ManagementBaseObject)e.NewEvent.GetPropertyValue("TargetInstance");
            string caption = (string)pnp_device.GetPropertyValue("Caption");
            if (caption.Contains("Arduino"))
            {
                connect(); // sets port to a valid serial port or null
            }
        }

        public bool sendError(int errors)
        {
            return send("Errors:" + errors.ToString());
        }

        public bool sendText(string msg)
        {
            // here we should split for the different lines on the lcd
            return send(msg);
        }

        private bool send(string data)
        {
            try
            {
                if (port != null)
                {
                    port.Open();
                    port.Write(data);
                    port.Close();
                    return true;
                } else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.error(e.ToString());
                return false;
            }
        }

        public void connect()
        {
            try {
                string deviceID = arduinoPort();
                if (deviceID != "")
                {
                    port = new SerialPort(deviceID, 9600);
                    Logger.log("Connected to Arduino with device ID " + deviceID);
                }
                else
                {
                    port = null;
                }
            }
            catch (Exception e)
            {
                Logger.error(e.ToString());
            }
        }

        private static string arduinoPort()
        {
            SelectQuery selectQuery = new SelectQuery("Win32_SerialPort");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(selectQuery);

            try {
                foreach (ManagementObject port in searcher.Get())
                {

                    if (port.GetPropertyValue("Name").ToString().Contains("Arduino"))
                    {
                        return port.GetPropertyValue("DeviceID").ToString();
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                // probably a disconnect
                return "";
            }
        }


        private static object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName).GetValue(obj, null);
        }
    }
}
