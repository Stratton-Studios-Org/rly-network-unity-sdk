using System.Runtime.InteropServices;
using System.Threading.Tasks;

using RallyProtocol.Core;

namespace RallyProtocolUnity.Storage
{

    public class IOSKeyManager : IPlatformKeyManager
    {

        #region External Methods

#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        static extern string getBundleId();

        [DllImport("__Internal")]
        static extern string getMnemonic();

        [DllImport("__Internal")]
        static extern string generateMnemonic();

        [DllImport("__Internal")]
        static extern bool saveMnemonic(string mnemonic, bool saveToCloud, bool rejectOnCloudSaveFailure);

        [DllImport("__Internal")]
        static extern bool deleteMnemonic();

        [DllImport("__Internal")]
        static extern bool deleteCloudMnemonic();

        [DllImport("__Internal")]
        static extern string getPrivateKeyFromMnemonic(string mnemonic);
        
        [DllImport("__Internal")]
        static extern bool mnemonicBackedUpToCloud();
#else
#pragma warning disable IDE0060, IDE1006
        static string getBundleId() => string.Empty;
        static string getMnemonic() => string.Empty;
        static string generateMnemonic() => string.Empty;
        static bool saveMnemonic(string mnemonic, bool saveToCloud, bool rejectOnCloudSaveFailure) => false;
        static bool deleteMnemonic() => false;
        static bool deleteCloudMnemonic() => false;
        static string getPrivateKeyFromMnemonic(string mnemonic) => string.Empty;
        static bool mnemonicBackedUpToCloud() => false;
#pragma warning restore IDE0060, IDE1006
#endif

        #endregion

        #region Public Methods

        public Task<string> GetBundleId() => Task.FromResult(getBundleId());

        public Task<string> GetMnemonic() => Task.FromResult(getMnemonic());

        public Task<string> GenerateNewMnemonic() => Task.FromResult(generateMnemonic());

        public Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig options = null) => Task.FromResult(saveMnemonic(mnemonic, options.SaveToCloud.GetValueOrDefault(), options.RejectOnCloudSaveFailure.GetValueOrDefault()));

        public Task<bool> DeleteMnemonic() => Task.FromResult(deleteMnemonic());

        public Task<bool> DeleteCloudMnemonic() => Task.FromResult(deleteMnemonic());

        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic) => Task.FromResult(getPrivateKeyFromMnemonic(mnemonic));

        public Task<bool> IsMnemonicEligibleForCloudSync() => Task.FromResult(mnemonicBackedUpToCloud());

        #endregion

    }

}