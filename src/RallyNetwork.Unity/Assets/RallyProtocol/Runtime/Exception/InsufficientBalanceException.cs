using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol
{

    [System.Serializable]
    public class InsufficientBalanceException : RallyException
    {

        public const string DefaultMessage = "Unable to transfer, insufficient balance";

        public InsufficientBalanceException() : base(DefaultMessage) { }
        public InsufficientBalanceException(string message) : base(message) { }
        public InsufficientBalanceException(string message, System.Exception inner) : base(message, inner) { }
        protected InsufficientBalanceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}