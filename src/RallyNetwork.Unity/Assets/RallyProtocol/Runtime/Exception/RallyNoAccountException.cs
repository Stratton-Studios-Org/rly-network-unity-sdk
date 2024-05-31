using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol
{


    [System.Serializable]
    public class RallyNoAccountException : RallyException
    {

        public const string DefaultMessage = "No account";

        public RallyNoAccountException() : base(DefaultMessage) { }
        public RallyNoAccountException(string message) : base(message) { }
        public RallyNoAccountException(string message, System.Exception inner) : base(message, inner) { }
        protected RallyNoAccountException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}