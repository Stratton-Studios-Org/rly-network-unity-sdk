using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol
{

    [System.Serializable]
    public class RallyException : System.Exception
    {
        public RallyException() { }
        public RallyException(string message) : base(message) { }
        public RallyException(string message, System.Exception inner) : base(message, inner) { }
        protected RallyException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}