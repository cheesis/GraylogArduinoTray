using System;
using System.Diagnostics;


namespace GraylogArduinoTray
{
    class Logger
    {

        public static void log(string msg)
        {
            Trace.TraceInformation(msg);
        }

        public static void error(string msg)
        {
            Trace.TraceError(msg);
        }

        public static void warning(string msg)
        {
            Trace.TraceWarning(msg);
        }
    }
}