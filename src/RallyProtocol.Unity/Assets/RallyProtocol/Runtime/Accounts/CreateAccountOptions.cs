using System.Collections;
using System.Collections.Generic;

using RallyProtocol.Core;

namespace RallyProtocol.Accounts
{

    [System.Serializable]
    public class CreateAccountOptions
    {

        #region Fields

        /// <summary>
        /// Whether to overwrite the existing account if there are any.
        /// </summary>
        public virtual bool? Overwrite { get; set; }

        /// <summary>
        /// The key storage options
        /// </summary>
        public virtual KeyStorageConfig StorageOptions { get; set; }

        #endregion

    }

}