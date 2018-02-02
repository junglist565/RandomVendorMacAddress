using System;
using System.Diagnostics;

namespace RandomVendorMacAddress
{
    public class LogManager
    {
        private const String ERROR_LOG_FORMAT = "Exception: {0}\nMessage: {1}\n\n{2}";

        public static void LogException(Exception ex)
        {
            String logMessage = String.Format(ERROR_LOG_FORMAT, ex.ToString(), ex.Message, ex.StackTrace);

            Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
        }
    }
}
