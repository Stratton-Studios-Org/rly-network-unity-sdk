using System.Threading.Tasks;

using RallyProtocol;
using RallyProtocol.Core;

namespace RallyProtocolUnity.Storage
{

    /// <summary>
    /// Handles platform-specific key storage.
    /// </summary>
    public class RallyUnityKeyManager : IPlatformKeyManager
    {

        #region Fields

        protected static RallyUnityKeyManager defaultInstance;

        protected readonly IPlatformKeyManager platform;

        #endregion

        #region Properties

        public static RallyUnityKeyManager Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new();
                }

                return defaultInstance;
            }
        }

        #endregion

        #region Constructors

        public RallyUnityKeyManager()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            platform = new AndroidKeyManager();
#elif UNITY_IOS && !UNITY_EDITOR
            platform = new IOSKeyManager();
#elif UNITY_EDITOR
            platform = new StandaloneKeyManager();
#else
            platform = new UnsupportedPlatformKeyManager();
#endif
        }

        #endregion

        #region Public Methods

        public Task<string> GetBundleId() => platform.GetBundleId();
        public Task<string> GetMnemonic() => platform.GetMnemonic();
        public Task<string> GenerateNewMnemonic() => platform.GenerateNewMnemonic();
        public Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig options = null) => platform.SaveMnemonic(mnemonic, options);
        public Task<bool> DeleteMnemonic() => platform.DeleteMnemonic();
        public Task<bool> DeleteCloudMnemonic() => platform.DeleteCloudMnemonic();
        public Task<bool> IsMnemonicEligibleForCloudSync() => platform.IsMnemonicEligibleForCloudSync();
        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic) => platform.GetPrivateKeyFromMnemonic(mnemonic);

        /// <summary>
        /// Updates the storage settings for an existing wallet.
        /// Accepts a KeyStorageConfig object to specify the storage options for the wallet, same as when creating the wallet
        ///
        /// Throws an error if no wallet is found.
        /// Will reject the promise if the cloud save fails and rejectOnCloudSaveFailure is set to true.
        /// If rejectOnCloudSaveFailure is set to false, cloud save failure will fallback to on device only storage without rejecting the promise.
        ///
        /// Please note that when moving from KeyStorageConfig.saveToCloud = false to true, the wallet will be moved to device cloud
        /// which will replace a non cloud on device wallet your user might have on a different device. You should ensure you properly
        /// communicate to end users that moving to cloud storage will could cause issues if they currently have different wallets on different devices
        /// </summary>
        /// <param name="optinos"></param>
        /// <returns></returns>
        /// <exception cref="MissingWalletException"></exception>
        public async Task UpdateWalletStorage(KeyStorageConfig optinos = null)
        {
            string mnemonic = await GetMnemonic();
            if (string.IsNullOrEmpty(mnemonic))
            {
                throw new MissingWalletException("Unable to update storage settings, no wallet found");
            }

            await SaveMnemonic(mnemonic, optinos);

            if (optinos.SaveToCloud == false)
            {
                await DeleteCloudMnemonic();
            }
        }

        #endregion

    }

}