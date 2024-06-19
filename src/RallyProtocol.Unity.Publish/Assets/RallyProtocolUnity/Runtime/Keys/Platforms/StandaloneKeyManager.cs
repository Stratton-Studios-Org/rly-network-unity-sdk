using System;
using System.IO;
using System.Threading.Tasks;

using NBitcoin;

using Nethereum.HdWallet;

using RallyProtocol.Core;

using UnityEngine;

namespace RallyProtocolUnity.Storage
{

    public class StandaloneKeyManager : IPlatformKeyManager
    {

        #region Constants

        public const string DefaultFileName = "rally_mnemonic.txt";

        #endregion

        #region Fields

        protected string fileName;

        #endregion

        #region Constructors

        public StandaloneKeyManager(string fileName = DefaultFileName)
        {
            if (!Application.isEditor)
            {
                throw new InvalidOperationException($"{nameof(StandaloneKeyManager)} can only be used on Android");
            }

            this.fileName = fileName;
        }

        #endregion

        #region Public Methods

        public string GetFilePath()
        {
            return Path.Combine(Application.persistentDataPath, this.fileName);
        }

        public Task<string> GetBundleId()
        {
            return Task.FromResult(Application.identifier);
        }

        public Task<string> GetMnemonic()
        {
            return Task.FromResult(File.ReadAllText(GetFilePath()));
        }

        public Task<string> GenerateNewMnemonic()
        {
            Mnemonic mnemonic = new(Wordlist.English, WordCount.Twelve);
            return Task.FromResult(mnemonic.ToString());
        }

        public Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig options = null)
        {
            File.WriteAllText(GetFilePath(), mnemonic);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteMnemonic()
        {
            File.Delete(GetFilePath());
            return Task.FromResult(true);
        }

        public Task<bool> DeleteCloudMnemonic()
        {
            Debug.LogError("Deleting cloud mnemonic is not implemented on standalone platform");
            return Task.FromResult(false);
        }

        public Task<bool> IsMnemonicEligibleForCloudSync()
        {
            Debug.LogError("Backup functionality is not implemented on standalone platform");
            return Task.FromResult(false);
        }

        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic)
        {
            Wallet wallet = new(mnemonic, "");
            return Task.FromResult(wallet.GetAccount(0).PrivateKey);
        }

        #endregion

    }

}