using System;

namespace Haden.NxtSharp.Utilties
{
    /// <summary>
    /// Thread-safe equivalent of System.Random using static methods.
    /// </summary>
    public static class StaticRandom
    {
        static readonly Random Random = new Random();
        static readonly object TheLock = new object();
        /// <summary>
        /// Returns a nonnegative random number. 
        /// </summary>		
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than Int32.MaxValue.</returns>
        public static int Next()
        {
            lock (TheLock)
            {
                return Random.Next();
            }
        }
        /// <summary>
        /// Returns a nonnegative random number less than the specified maximum. 
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; that is, the range of return values includes zero but not maxValue.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If maxValue is less than zero.</exception>
        public static int Next(int max)
        {
            lock (TheLock)
            {
                return Random.Next(max);
            }
        }
        /// <summary>
        /// Returns a random number within a specified range. 
        /// </summary>
        /// <param name="min">The inclusive lower bound of the random number returned. </param>
        /// <param name="max">
        /// The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; that is, the range of return values includes minValue but not maxValue. If minValue equals maxValue, minValue is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">If minValue is greater than maxValue.</exception>
        public static int Next(int min, int max)
        {
            lock (TheLock)
            {
                return Random.Next(min, max);
            }
        }
        /// <summary>
        /// Returns a random number between 0.0 and 1.0.
        /// </summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        public static double NextDouble()
        {
            lock (TheLock)
            {
                return Random.NextDouble();
            }
        }
        /// <summary>
        /// Fills the elements of a specified array of bytes with random numbers.
        /// </summary>
        /// <param name="buffer">An array of bytes containing random numbers.</param>
        /// <exception cref="ArgumentNullException">If buffer is null.</exception>
        public static void NextBytes(byte[] buffer)
        {
            lock (TheLock)
            {
                Random.NextBytes(buffer);
            }
        }
    }
}
