using System;
using System.Collections.Generic;
using System.Text;

namespace Enexure.MicroBus
{
    public class InvalidDuplicateRegistrationsException : Exception
    {
        public InvalidDuplicateRegistrationsException(IReadOnlyCollection<Type> messageTypes)
            : base(GetExceptionMessage(messageTypes))
        {
        }

        private static string GetExceptionMessage(IReadOnlyCollection<Type> messageTypes)
        {
            var builder = new StringBuilder();
            builder.AppendLine("The following types cannot have duplicate registrations");
            builder.AppendLine();

            var i = 1;
            foreach (var messageType in messageTypes)
            {
                builder.AppendFormat("{0}) {1}{2}", i++, messageType, Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}