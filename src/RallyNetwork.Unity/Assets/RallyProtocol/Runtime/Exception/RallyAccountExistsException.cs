using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace RallyProtocol
{

    [System.Serializable]
    public class RallyAccountExistsException : RallyException
    {

        public const string DefaultMessage = "Account already exists. Use overwrite flag to overwrite";

        public RallyAccountExistsException() : base(DefaultMessage) { }
        public RallyAccountExistsException(string message) : base(message) { }
        public RallyAccountExistsException(string message, System.Exception inner) : base(message, inner) { }
        protected RallyAccountExistsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    }

}