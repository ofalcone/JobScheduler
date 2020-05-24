using System;

namespace JobScheduler.Infrastructure
{
    public class IdentityException : Exception
    {
        public string Code { get; }

        public string Description { get; }

        public IdentityException(string code, string description)
            : base($"Identity Error, Code: {code}, Description: {description}")
        { }
    }
}
