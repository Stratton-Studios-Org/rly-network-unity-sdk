using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Accounts
{

    public class CreateAccountOptions
    {

        /// <summary>
        /// Whether to overwrite the existing account if there are any.
        /// </summary>
        public bool? Overwrite;

        /// <summary>
        /// The key storage options
        /// </summary>
        public KeyStorageConfig StorageOptions;

    }

}