using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Haden.NxtSharp.Exceptions;

namespace Haden.NxtSharp.Utilties
{
    /// <summary>
    /// The class containing operational functions.
    /// </summary>
    public class Functions
    {
        static readonly string[] HexChars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

        private Functions() { }

        #region Filename manipulation

        /// <summary>
        /// Strips the path from a filename
        /// 
        /// i.e. 'D:\util\zip.exe' becomes 'zip.exe'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StripDirectory(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index != -1)
            {
                return path.Substring(index + 1);
            }
            return path;
        }
        /// <summary>
        /// Strips the filename from a path
        /// 
        /// i.e. 'D:\util\zip.exe' becomes 'D:\util'
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string StripFilename(string path)
        {
            int index = path.LastIndexOf('\\');
            if (index != -1 && index != path.Length - 1)
            {
                return path.Substring(0, index);
            }
            return path;
        }
        /// <summary>
        /// Gets the extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static string GetExtension(string path)
        {
            int lastForwardSlashIdx = path.LastIndexOf('\\');
            int lastDotIdx = path.LastIndexOf('.');
            if (lastForwardSlashIdx > lastDotIdx)
            {
                return "";
            }
            return path.Substring(lastDotIdx + 1);
        }

        #endregion

        #region String functions

        /// <summary>
        /// Pads a number with zeroes
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string PadZero(int value, int length)
        {
            return value.ToString(CultureInfo.InvariantCulture).PadLeft(length, '0');
        }
        /// <summary>
        /// Creates a string containing a list of strings
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static string CreateList(List<string> items)
        {
            return CreateList(items, "");
        }
        /// <summary>
        /// Creates a string containing a list of strings
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public static string CreateList(List<string> items, string defaultValue)
        {
            if (items == null || items.Count == 0)
            {
                return defaultValue;
            }
            string result = "";
            int count = 0;
            foreach (string item in items)
            {
                if (count > 0)
                {
                    if (count == items.Count - 1)
                    {
                        result += " en ";
                    }
                    else
                    {
                        result += ", ";
                    }
                }
                result += item;
                count++;
            }
            return result;
        }
        /// <summary>
        /// Creates a string containing a list of strings
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="comma">The comma.</param>
        /// <param name="and">The and.</param>
        /// <returns></returns>
        public static string CreateList(List<object> items, string comma, string and)
        {
            StringBuilder result = new StringBuilder();
            int count = 0;
            foreach (object obj in items)
            {
                if (count > 0)
                {
                    result.Append(count == items.Count - 1 ? and : comma);
                }
                result.Append(obj);
                count++;
            }
            return result.ToString();
        }
        /// <summary>
        /// Returns true if the whitespace trimmed, lowercase
        /// versions of both strings are equal
        /// </summary>
        /// <param name="s1"></param>
        /// <param name="s2"></param>
        /// <returns></returns>
        public static bool KindOfEquals(string s1, string s2)
        {
            return s1.Trim().ToLower() == s2.Trim().ToLower();
        }
        /// <summary>
        /// Returns the string value of a byte array as hexidecimal.
        /// </summary>
        /// <param name="hex">The byte array in hex.</param>
        /// <returns></returns>
        public static string Hex(byte[] hex)
        {
            var builder = new StringBuilder();
            for (var i = 0; i < hex.Length; i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" ");
                }
                builder.Append(Hex(hex[i]));
            }
            return builder.ToString();
        }
        /// <summary>
        /// Returns ASCII string value as hexidecimal.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static string Hex(string s)
        {
            return Hex(Encoding.ASCII.GetBytes(s));
        }
        /// <summary>
        /// Returns the string value of a byte as hexidecimal.
        /// </summary>
        /// <param name="b">The byte in hex.</param>
        /// <returns></returns>
        public static string Hex(byte b)
        {
            var hi = b / 16;
            var lo = b % 16;
            return HexChars[hi] + HexChars[lo];
        }
        

        #endregion

        #region Date/Time functions

        /// <summary>
        /// Returns a timestamp in ISO format (YYYYMMDDHHMMSSMMMM)
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string IsoTimeStamp(DateTime time)
        {
            return time.Year + PadZero(time.Month, 2) + PadZero(time.Day, 2) + PadZero(time.Hour, 2) + PadZero(time.Minute, 2) + PadZero(time.Second, 2) + PadZero(time.Millisecond, 4);
        }
        /// <summary>
        /// Returns the total number of milliseconds until the specified time is reached.
        /// 
        /// If the specified time is in the past, the resulting number will be negative
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double MilliSecondsTo(DateTime time)
        {
            TimeSpan span = time.Subtract(DateTime.Now);
            return span.TotalMilliseconds;
        }
        /// <summary>
        /// Retrieves the number of milliseconds that elapsed 
        /// since the computer started
        /// </summary>
        /// <returns></returns>
        public static long MilliSeconds()
        {
            if (_hiresTimerFrequency == 0)
            {
                QueryPerformanceFrequency(out _hiresTimerFrequency);
            }

            long result;
            QueryPerformanceCounter(out result);
            result *= 1000;
            result /= _hiresTimerFrequency;
            return result;
        }
        private static long _hiresTimerFrequency;
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        #endregion

        #region Random

        /// <summary>
        /// Contains a random object for general use
        /// </summary>
        /// <value></value>
        public static Random Random
        {
            get
            {
                return RandomObject;
            }
        }
        private static readonly Random RandomObject = new Random();

        #endregion

        #region Math

        /// <summary>
        /// Mods the specified n.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns></returns>
        public static int Mod(int n, int modulo)
        {
            if (n < 0)
            {
                int a = Math.Abs(n / modulo);
                n += (a + 1) * modulo;
            }
            return n % modulo;
        }
        /// <summary>
        /// Mods the specified n.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <param name="modulo">The modulo.</param>
        /// <returns></returns>
        public static double Mod(double n, double modulo)
        {
            if (n < 0)
            {
                int a = (int)Math.Abs(n / modulo);
                n += (a + 1) * modulo;
            }
            return n % modulo;
        }
        /// <summary>
        /// Distances the specified points.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
        /// <summary>
        /// Clamps to the specified sensor to send instructions.
        /// </summary>
        /// <param name="n">The sensor.</param>
        /// <param name="min">The sensor's minimum value range.</param>
        /// <param name="max">The sensor's maximum value range.</param>
        /// <returns></returns>
        public static float Clamp(float n, float min, float max)
        {
            return Math.Max(Math.Min(n, max), min);
        }

        #endregion

        #region Color

        /// <summary>
        /// Converts a (h,s,v) color to a RGB color
        /// </summary>
        /// <param name="hue">The hue of the color [0-360]</param>
        /// <param name="saturation">Saturation [0.0-1.0]</param>
        /// <param name="value">Value [0.0-1.0]</param>
        /// <returns></returns>
        public static Color RgbFromHsv(double hue, double saturation, double value)
        {
            hue = Mod(hue, 360.0);
            int quadrant = (int)(hue / 60.0);
            double f = (hue / 60.0) - quadrant;
            double p = value * (1.0 - saturation);
            double q = value * (1.0 - f * saturation);
            double r = value * (1.0 - (1.0 - f) * saturation);
            switch (quadrant)
            {
                case 0:
                    return Color.FromArgb((int)(value * 255.0), (int)(r * 255.0), (int)(p * 255.0));
                case 1:
                    return Color.FromArgb((int)(q * 255.0), (int)(value * 255.0), (int)(p * 255.0));
                case 2:
                    return Color.FromArgb((int)(p * 255.0), (int)(value * 255.0), (int)(r * 255.0));
                case 3:
                    return Color.FromArgb((int)(p * 255.0), (int)(q * 255.0), (int)(value * 255.0));
                case 4:
                    return Color.FromArgb((int)(r * 255.0), (int)(p * 255.0), (int)(value * 255.0));
                case 5:
                    return Color.FromArgb((int)(value * 255.0), (int)(p * 255.0), (int)(q * 255.0));
                default:
                    throw new Exception("This should not happen!");
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// This function populates the string with values from the datarow
        /// 
        /// 'Adres: {address}' -> 'Adres: Noorderstraat 5'
        /// </summary>
        /// <param name="str"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static string Populate(string str, DataRow row)
        {
            foreach (DataColumn column in row.Table.Columns)
            {
                str = str.Replace("{" + column.ColumnName + "}", row[column].ToString());
            }
            return str;
        }

        #endregion

        #region Debugging

        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, "A condition was not satisfied.");
        }
        /// <summary>
        /// Asserts the specified condition.
        /// </summary>
        /// <param name="condition">if set to <c>true</c> [condition].</param>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        /// <exception cref="AssertionFailedException"></exception>
        public static void Assert(bool condition, string message, string title)
        {
            if (!condition)
            {
                throw new AssertionFailedException(message, title);
            }
        }

        #endregion

        #region Conversions

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static int GetInt32(byte[] buffer, int start)
        {
            int result = 0;
            result += buffer[start];
            result += 256 * buffer[start + 1];
            result += 65536 * buffer[start + 2];
            result += 16777216 * buffer[start + 3];
            return result;
        }
        /// <summary>
        /// Gets the uint16.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static ushort GetUInt16(byte[] buffer, int start)
        {
            ushort result = 0;
            result += buffer[start];
            result += (ushort)(256 * buffer[start + 1]);
            return result;
        }
        /// <summary>
        /// Gets the uint32.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static uint GetUInt32(byte[] buffer, int start)
        {
            uint result = 0;
            result += buffer[start];
            result += (uint)(256 * buffer[start + 1]);
            result += (uint)(65536 * buffer[start + 2]);
            result += (uint)(16777216 * buffer[start + 3]);
            return result;
        }
        /// <summary>
        /// Gets the int16.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <returns></returns>
        public static short GetInt16(byte[] buffer, int start)
        {
            short result = 0;
            result += buffer[start];
            result += (short)(256 * buffer[start + 1]);
            return result;
        }
        /// <summary>
        /// Sets the int32.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="value">The value.</param>
        public static void SetInt32(byte[] buffer, int start, int value)
        {
            for (int i = 0; i < 4; i++)
            {
                buffer[start + i] = (byte)(value & 0xff);
                value /= 256;
            }
        }
        /// <summary>
        /// Sets the uint32.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="value">The value.</param>
        public static void SetUInt32(byte[] buffer, int start, uint value)
        {
            for (int i = 0; i < 4; i++)
            {
                buffer[start + i] = (byte)(value & 0xff);
                value /= 256;
            }
        }
        /// <summary>
        /// Sets the int16.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="start">The start.</param>
        /// <param name="value">The value.</param>
        public static void SetInt16(byte[] buffer, int start, Int16 value)
        {
            for (int i = 0; i < 2; i++)
            {
                buffer[start + i] = (byte)(value & 0xff);
                value /= 256;
            }
        }

        #endregion
    }
}
