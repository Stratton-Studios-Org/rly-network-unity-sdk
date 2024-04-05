using System.Threading.Tasks;

namespace RallyProtocol
{

    public interface IPlatformKeyManager
    {

        Task<string> GetBundleId();
        Task<string> GetMnemonic();
        Task<string> GenerateNewMnemonic();
        Task<bool> SaveMnemonic(string mnemonic, bool saveToCloud, bool rejectOnCloudSaveFailure);
        Task<bool> DeleteMnemonic();
        Task<bool> IsMnemonicBackedUpToCloud();
        Task<string> GetPrivateKeyFromMnemonic(string mnemonic);

    }

}