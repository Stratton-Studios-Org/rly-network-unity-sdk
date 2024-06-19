using System.Collections;
using System.Collections.Generic;

namespace RallyProtocol.Core
{

    /// <summary>
    /// Configuration for how the wallet key should be stored. This includes whether to save to cloud and whether to reject if saving to cloud fails.
    /// </summary>
    /// <remarks>
    /// Please note that when moving from <see cref="SaveToCloud"/> = false to true, the wallet will be moved to cross device sync. This can overwrite a device only wallet your user might have on a different device.
    /// You should ensure you properly communicate to end users that moving to cloud storage will could cause issues if they currently have different wallets on different devices.
    ///
    /// There are several other gotchas to keep in mind when it comes to saveToCloud.
    /// 1. Keys are stored using the OS provided cross device backup mechanism. This mechanism is controlled by user and app preferences and can be disabled by the user or app developer.
    /// 2. On Android, the backup mechanism is Blockstore, which requires user to be logged in to play account and have a device pincode or password set.
    /// 3. On iOS, the backup mechanism is iCloud keychain, which requires user to be logged in to iCloud and have iCloud backup enabled.
    /// </remarks>
    [System.Serializable]
    public class KeyStorageConfig
    {

        /// <summary>
        /// Gets or sets whether to save the mnemonic in a way that is eligible for device OS cloud storage. If set to false, the mnemonic will only be stored on device.
        /// </summary>
        public virtual bool? SaveToCloud { get; set; }

        /// <summary>
        /// Gets or sets whether to raise an error if saving to cloud fails. If set to false, the mnemonic will silently fall back to local on device only storage.
        /// </summary>
        public virtual bool? RejectOnCloudSaveFailure { get; set; }

    }

}