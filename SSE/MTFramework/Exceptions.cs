using System;

namespace MTFramework.Utilities
{
    public class MTFException : Exception
    {
        public MTFException(string message = "")
            : base(message)
        { }
    }
}