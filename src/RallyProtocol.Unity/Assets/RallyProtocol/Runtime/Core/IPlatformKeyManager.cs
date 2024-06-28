using System.Threading.Tasks;

namespace RallyProtocol.Core
{

    /// <summary>
    /// A native key manager interface, implement this on each platform to add support for key storage and generation.
    /// </summary>
    public interface IPlatformKeyManager
    {

        #region Public Methods

        /// <summary>
        /// Gets the bundle ID.
        /// </summary>
        /// <returns>Returns the bundle ID</returns>
        Task<string> GetBundleId();

        /// <summary>
        /// Getst the saved mnemonic phrase if there are any.
        /// </summary>
        /// <returns>Returns the saved mnemonic phrase, otherwise null</returns>
        Task<string> GetMnemonic();

        /// <summary>
        /// Generates a new mnemonic phrase natively.
        /// </summary>
        /// <returns>Returns a newly generated mnemonic phrase</returns>
        Task<string> GenerateNewMnemonic();

        /// <summary>
        /// Saves the mnemonic phrase locally natively.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to save</param>
        /// <param name="options">The storage options</param>
        /// <returns>Returns true if successful, otherwise false</returns>
        Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig? options = null);

        /// <summary>
        /// Deletes the currently stored mnemonic phrase.
        /// </summary>
        /// <returns>Returns true if deleted, otherwise false</returns>
        Task<bool> DeleteMnemonic();

        /// <summary>
        /// Removes the mnemonic from the cloud storage. This is a destructive operation.
        /// </summary>
        /// <remarks>
        /// This is necessary for the case where dev wants to move user storage from cloud to local only.
        /// </remarks>
        /// <returns>Returns true if deleted, otherwise false</returns>
        Task<bool> DeleteCloudMnemonic();

        /// <summary>
        /// Returns whether the current wallet is stored in a way that is eligible for OS provided cloud backup and cross device sync.
        /// This is not a guarantee that the wallet is backed up to cloud,
        /// as the some user & app level settings determine whether secure keys are backed up to device cloud.
        /// On iOS this is a check whether the wallet will sync if user enables iCloud -> Keychain sync.
        /// On Android this is a check whether the wallet is in google play keystore and will sync if user enables google backup.
        /// TRUE response indicates that the wallet will be backed up to OS cloud if user enables the OS provided cloud backup / cross device sync.
        ///
        /// This method should not be used as a check for wallet existence
        /// as it will return false if there is no wallet or if the wallet does exist but is not backed up to cloud.
        /// </summary>
        /// <returns>Returns true if is eligible, otherwise false</returns>
        Task<bool> IsMnemonicEligibleForCloudSync();

        /// <summary>
        /// Gets the private key from menonic phrase.
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <returns>Returns the private key if successful, otherwise null</returns>
        Task<string> GetPrivateKeyFromMnemonic(string mnemonic);

        #endregion

    }

}