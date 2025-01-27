using System;

namespace Haden.NxtSharp.Exceptions
{
    /// <summary>
    /// The class which handles assertion failed exceptions.
    /// </summary>
    public class AssertionFailedException : ApplicationException
    {
        /// <summary>
        /// The _title
        /// </summary>
        readonly string _title;
        /// <summary>
        /// Gets the title.
        /// </summary>
        public string Title { get { return _title; } }
        /// <summary>
        /// Initializes a new instance of the <see cref="AssertionFailedException" /> class with the message and title.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="title">The title.</param>
        public AssertionFailedException(string message, string title)
            : base(message)
        {
            _title = title;
        }
    }
}
