using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    [System.Serializable]
    public class TransferMethodNotSupportedException : RallyException
    {

        public const string DefaultMessage = "This token does not have a transfer method supported by this sdk";

        public TransferMethodNotSupportedException() : base(DefaultMessage) { }
        public TransferMethodNotSupportedException(string message) : base(message) { }
        public TransferMethodNotSupportedException(string message, System.Exception inner) : base(message, inner) { }
        protected TransferMethodNotSupportedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}