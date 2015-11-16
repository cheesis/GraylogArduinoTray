using System;
using System.Diagnostics;


namespace GraylogArduinoTray
{
    class Logger
    {

        public static void log(string msg)
        {
            if (Properties.Settings.Default.EnableLogging)
                Trace.TraceInformation(msg);
        }

        public static void error(string msg)
        {
            if (Properties.Settings.Default.EnableLogging)
                Trace.TraceError(msg);
        }

        public static void warning(string msg)
        {
            if (Properties.Settings.Default.EnableLogging)
                Trace.TraceWarning(msg);
        }
    }
}