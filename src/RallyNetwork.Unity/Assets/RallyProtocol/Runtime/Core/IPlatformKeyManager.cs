using System.Threading.Tasks;

namespace RallyProtocol.Core
{

    public interface IPlatformKeyManager
    {

        #region Public Methods

        Task<string> GetBundleId();
        Task<string> GetMnemonic();
        Task<string> GenerateNewMnemonic();
        Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig options = null);
        Task<bool> DeleteMnemonic();
        Task<bool> IsMnemonicBackedUpToCloud();
        Task<string> GetPrivateKeyFromMnemonic(string mnemonic);

        #endregion

    }

}