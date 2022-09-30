using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Exceptions
{
    public class CustomExceptions : Exception
    {
        public CustomExceptions()
        {
        }

        public CustomExceptions(string message) : base(message)
        {
        }

        public CustomExceptions(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CustomExceptions(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
