using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol
{

    [System.Serializable]
    public class PriorDustingException : RallyException
    {

        public const string DefaultMessage = "Account already dusted, will not dust again";

        public PriorDustingException() : base(DefaultMessage) { }
        public PriorDustingException(string message) : base(message) { }
        public PriorDustingException(string message, System.Exception inner) : base(message, inner) { }
        protected PriorDustingException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}