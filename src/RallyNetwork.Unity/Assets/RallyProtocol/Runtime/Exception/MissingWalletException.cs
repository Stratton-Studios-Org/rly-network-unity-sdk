using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    [System.Serializable]
    public class MissingWalletException : RallyException
    {

        public const string DefaultMessage = "Unable to perform action, no account on device";

        public MissingWalletException() : base(DefaultMessage) { }
        public MissingWalletException(string message) : base(message) { }
        public MissingWalletException(string message, System.Exception inner) : base(message, inner) { }
        protected MissingWalletException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}