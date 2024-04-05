using System.Threading.Tasks;

namespace RallyProtocol
{

    /// <summary>
    /// Handles platform-specific key storage.
    /// </summary>
    public class KeyManager
    {

        protected readonly IPlatformKeyManager platform;

        public KeyManager()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            platform = new RallyProtocolAndroidPlatform();
#elif UNITY_IOS && !UNITY_EDITOR
            platform = new RallyProtocolIOSPlatform();
#else
            platform = new UnsupportedPlatformKeyManager();
#endif
        }

        public Task<string> GetBundleId() => platform.GetBundleId();
        public Task<string> GetMnemonic() => platform.GetMnemonic();
        public Task<string> GenerateNewMnemonic() => platform.GenerateNewMnemonic();
        public Task<bool> SaveMnemonic(string mnemonic, bool saveToCloud, bool rejectOnCloudSaveFailure) => platform.SaveMnemonic(mnemonic, saveToCloud, rejectOnCloudSaveFailure);
        public Task<bool> DeleteMnemonic() => platform.DeleteMnemonic();
        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic) => platform.GetPrivateKeyFromMnemonic(mnemonic);

    }

}