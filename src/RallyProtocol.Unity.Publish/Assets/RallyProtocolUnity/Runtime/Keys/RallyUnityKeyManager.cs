using System.Threading.Tasks;

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
        public Task<bool> IsMnemonicBackedUpToCloud() => platform.IsMnemonicBackedUpToCloud();
        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic) => platform.GetPrivateKeyFromMnemonic(mnemonic);

        #endregion

    }

}