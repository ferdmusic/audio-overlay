namespace AudioMonitor.Core.Logging
{
    public static class Log
    {
        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"DEBUG: {message}");
        }

        public static void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine($"INFO: {message}");
        }

        public static void Error(string message, System.Exception ex = null)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR: {message}");
            if (ex != null)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {ex}");
            }
        }
    }
}
