using System;
using System.IO;
using System.Text;

namespace Haden.NxtSharp
{
    /// <summary>
    /// The class which performs logging for the library. Originated in MACOS 9.0.4 under CodeWarrior late 2014.
    /// </summary>
    public static class Logging
    {
        /// <summary>
        /// The file path for executing assemblies.
        /// </summary>
        public static string FilePath = Environment.CurrentDirectory;
        /// <summary>
        /// The type of log to write.
        /// </summary>
        [Flags]
        public enum LogType { Information, Error, Statistics, Warning };
        /// <summary>
        /// The classes within the interpreter calling the log.
        /// </summary>
        public enum LogCaller { Brick, Communicator, Computation, Control, Learner, Me, Mood, Motor, Sensor, TestFramework }
        /// <summary>
        /// The last message passed to logging.
        /// </summary>
        public static string LastMessage = "";
        /// <summary>
        /// The delegate for returning the last log message to the calling application.
        /// </summary>
        public delegate void LoggingDelegate();
        /// <summary>
        /// Occurs when [returned to console] is called.
        /// </summary>
        public static event LoggingDelegate ReturnedToConsole;
        /// <summary>
        /// Records an event relevant to the algorithm.
        /// </summary>
        /// <param name="dataPoint">The data point itself.</param>
        /// <param name="eventData">The event data.</param>
        /// <param name="intervalPeriod">The interval period.</param>
        /// <param name="intervalNomen">The interval nomen, e.g., s, m, h.</param>
        public static void RecordEvent(DataPoints.EventDataPoint dataPoint, double eventData, int intervalPeriod, string intervalNomen)
        {
            var stream = new StreamWriter(FilePath + @"\logs\monitor.txt", true);
            switch (dataPoint)
            {
                case DataPoints.EventDataPoint.Compute:
                    stream.WriteLine(DateTime.Now + " - Compute datapoint value is: [" + eventData + "] on an interval of " + intervalPeriod + " " + intervalNomen + ".");
                    break;
                case DataPoints.EventDataPoint.Control:
                    stream.WriteLine(DateTime.Now + " - Control datapoint value is: [" + eventData + "] on an interval of " + intervalPeriod + " " + intervalNomen + ".");
                    break;
                case DataPoints.EventDataPoint.Seek:
                    stream.WriteLine(DateTime.Now + " - Seek datapoint value is: [" + eventData + "] on an interval of " + intervalPeriod + " " + intervalNomen + ".");
                    break;
            }
            stream.Close();
        }
        /// <summary>
        /// Logs a message sent from the calling application to a file.
        /// </summary>
        /// <param name="message">The message to log. Space between the message and log type enumeration provided.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="caller">The class creating the log entry.</param>
        public static void WriteLog(string message, LogType logType, LogCaller caller)
        {
            LastMessage = message;
            StreamWriter stream = new StreamWriter(FilePath + @"\logs\logfile.txt", true);
            switch (logType)
            {
                case LogType.Error:
                    stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + " from " + caller + ".");
                    break;
                case LogType.Warning:
                    stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + " from " + caller + ".");
                    break;
                case LogType.Information:
                    stream.WriteLine(DateTime.Now + " - " + " INFO " + message);
                    break;
                case LogType.Statistics:
                    stream.WriteLine(DateTime.Now + " - " + message + ".");
                    break;
            }
            stream.Close();
            if (!Equals(null, ReturnedToConsole))
            {
                ReturnedToConsole();
            }
        }
        /// <summary>
        /// Writes the log.
        /// </summary>
        /// <param name="message">The message. Space between the message and log type enumeration provided.</param>
        /// <param name="logType">Type of the log.</param>
        /// <param name="caller">The class creating the log entry.</param>
        /// <param name="method">The method creating the log entry.</param>
        public static void WriteLog(string message, LogType logType, LogCaller caller, string method)
        {
            LastMessage = message;
            StreamWriter stream = new StreamWriter(FilePath + @"\logs\logfile.txt", true);
            switch (logType)
            {
                case LogType.Error:
                    stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + " from class " + caller + " using method " + method + ".");
                    break;
                case LogType.Warning:
                    stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + " from class " + caller + " using method " + method + ".");
                    break;
                case LogType.Information:
                    stream.WriteLine(DateTime.Now + " - " + message + " called from the class " + caller + " using method " + method + ".");
                    break;
                case LogType.Statistics:
                    stream.WriteLine(DateTime.Now + " - " + message + ".");
                    break;
            }
            stream.Close();
            if (!Equals(null, ReturnedToConsole))
            {
                ReturnedToConsole();
            }
        }
        public static void WriteLog(string message, LogType logType)
        {
            LastMessage = message;
            StreamWriter stream = new StreamWriter(FilePath + @"\logs\logfile.txt", true);
            switch (logType)
            {
                case LogType.Error:
                    stream.WriteLine(DateTime.Now + " - " + " ERROR " + " - " + message + ".");
                    break;
                case LogType.Warning:
                    stream.WriteLine(DateTime.Now + " - " + " WARNING " + " - " + message + ".");
                    break;
                case LogType.Information:
                    stream.WriteLine(DateTime.Now + " - " + message);
                    break;
            }
            stream.Close();
            if (!Equals(null, ReturnedToConsole))
            {
                ReturnedToConsole();
            }
        }
        /// <summary>
        /// Records a transcript of the conversation.
        /// </summary>
        /// <param name="message">The message to save in transcript format.</param>
        public static void RecordTranscript(string message)
        {
            try
            {
                StreamWriter stream = new StreamWriter(FilePath + @"\logs\transcript.txt", true);
                stream.WriteLine(DateTime.Now + " - " + message);
                stream.Close();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }

        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        public static void SaveLastResult(StringBuilder output)
        {
            try
            {
                StreamWriter stream = new StreamWriter(FilePath + @"\db\analytics.txt", true);
                stream.WriteLine(DateTime.Now + " - " + output);
                stream.Close();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }

        }
        /// <summary>
        /// Saves the last result to support analysis of the algorithm to storage.
        /// </summary>
        /// <param name="output">The output from the conversation.</param>
        public static void SaveLastResultToStorage(StringBuilder output)
        {
            try
            {
                StreamWriter stream = new StreamWriter(FilePath + @"\db\analyticsStorage.txt", true);
                stream.WriteLine("#" + DateTime.Now + ";" + output);
                stream.Close();
            }
            catch (Exception ex)
            {
                WriteLog(ex.Message, LogType.Error, LogCaller.Me);
            }

        }
        /// <summary>
        /// Writes a debug log with object parameters.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public static void Debug(params object[] objects)
        {
            StreamWriter stream = new StreamWriter(FilePath + @"\logs\dump.txt", true);
            foreach (object obj in objects)
            {
                stream.WriteLine(obj);
            }
            stream.WriteLine("--");
            stream.Close();
        }
    }
}
