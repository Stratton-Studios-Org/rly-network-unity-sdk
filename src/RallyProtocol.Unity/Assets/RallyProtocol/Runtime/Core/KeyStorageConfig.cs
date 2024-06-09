using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Core
{

    [System.Serializable]
    public class KeyStorageConfig
    {

        /// <summary>
        /// Gets or sets whether to save the key to the cloud.
        /// </summary>
        public virtual bool? SaveToCloud { get; set; }

        /// <summary>
        /// Gets or sets whether to reject the storage operation if cloud save fails.
        /// </summary>
        public virtual bool? RejectOnCloudSaveFailure { get; set; }

    }

}