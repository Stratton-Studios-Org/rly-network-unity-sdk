using System;
using System.Threading.Tasks;

namespace RallyProtocol.Core
{

    /// <summary>
    /// Unsupported platform key manager.
    /// </summary>
    public class UnsupportedPlatformKeyManager : IPlatformKeyManager
    {

        public Task<string> GetBundleId() => throw new NotImplementedException();
        public Task<string> GetMnemonic() => throw new NotImplementedException();
        public Task<string> GenerateNewMnemonic() => throw new NotImplementedException();
        public Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig? options = null) => throw new NotImplementedException();
        public Task<bool> DeleteMnemonic() => throw new NotImplementedException();
        public Task<bool> DeleteCloudMnemonic() => throw new NotImplementedException();
        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic) => throw new NotImplementedException();
        public Task<bool> IsMnemonicEligibleForCloudSync() => throw new NotImplementedException();

    }

}