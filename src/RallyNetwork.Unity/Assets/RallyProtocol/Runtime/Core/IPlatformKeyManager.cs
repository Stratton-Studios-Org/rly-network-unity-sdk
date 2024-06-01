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
        /// Checks whether the mnemonic is backed up to the cloud.
        /// </summary>
        /// <returns>Returns true if is backed up, otherwise false</returns>
        Task<bool> IsMnemonicBackedUpToCloud();

        /// <summary>
        /// Gets the private key from menonic phrase.
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase</param>
        /// <returns>Returns the private key if successful, otherwise null</returns>
        Task<string> GetPrivateKeyFromMnemonic(string mnemonic);

        #endregion

    }

}