using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol
{

    [System.Serializable]
    public class RelayException : RallyException
    {

        public const string DefaultMessage = "Unable to perform action, transaction relay error";

        public RelayException() : base(DefaultMessage) { }
        public RelayException(System.Exception inner) : base(DefaultMessage, inner) { }
        public RelayException(string message) : base(message) { }
        public RelayException(string message, System.Exception inner) : base(message, inner) { }
        protected RelayException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}