using System;
using System.Runtime.CompilerServices;
using TLog = Tizen.Log;

namespace Plugin.MediaManager
{
    public static class Log
    {
        public static string Tag { get; set; } = "Plugin.MediaManager";

        public static void Debug(string message,
                                 [CallerFilePath] string file = "",
                                 [CallerMemberName] string func = "",
                                 [CallerLineNumber] int line = 0)
        {
            TLog.Debug(Tag, message, file, func, line);
        }

        public static void Verbose(string message,
                                   [CallerFilePath] string file = "",
                                   [CallerMemberName] string func = "",
                                   [CallerLineNumber] int line = 0)
        {
            TLog.Verbose(Tag, message, file, func, line);
        }

        public static void Info(string message,
                                [CallerFilePath] string file = "",
                                [CallerMemberName] string func = "",
                                [CallerLineNumber] int line = 0)
        {
            TLog.Info(Tag, message, file, func, line);
        }

        public static void Warn(string message,
                                [CallerFilePath] string file = "",
                                [CallerMemberName] string func = "",
                                [CallerLineNumber] int line = 0)
        {
            TLog.Warn(Tag, message, file, func, line);
        }

        public static void Error(string message,
                                [CallerFilePath] string file = "",
                                [CallerMemberName] string func = "",
                                [CallerLineNumber] int line = 0)
        {
            TLog.Error(Tag, message, file, func, line);
        }

        public static void Fatal(string message,
                                [CallerFilePath] string file = "",
                                [CallerMemberName] string func = "",
                                [CallerLineNumber] int line = 0)
        {
            TLog.Fatal(Tag, message, file, func, line);
        }
    }
}