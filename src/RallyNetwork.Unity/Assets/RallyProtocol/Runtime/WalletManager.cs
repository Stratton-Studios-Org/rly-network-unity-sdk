using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nethereum.Model;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using UnityEngine;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol
{

    public class CreateAccountOptions
    {

        public bool? Overwrite;
        public KeyStorageConfig StorageOptions;

    }

    public class WalletManager
    {

        protected static WalletManager defaultInstance;

        protected Account currentAccount;
        protected KeyManager keyManager;

        public static WalletManager Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new(KeyManager.Default);
                }

                return defaultInstance;
            }
        }

        public Account CurrentAccount => this.currentAccount;

        public WalletManager(KeyManager keyManager)
        {
            this.keyManager = keyManager;
        }

        /// <summary>
        /// Creates a new wallet and saves it to the device based on the storage options provided.
        /// If a wallet already exists, it will throw an error unless the overwrite flag is set to true.
        /// If the overwrite flag is set to true, the existing wallet will be overwritten with the new wallet.
        /// KeyStorageConfig is used to specify the storage options for the wallet.
        /// If no storage options are provided, the default options of attmpting to save to cloud and rejecting on cloud save failure will be used.
        /// The rejectOnCloudSaveFailure flag is used to specify whether to reject the wallet creation if the cloud save fails.
        /// when set to true, the promise will reject if the cloud save fails. When set to false, the promise will resolve even if the cloud save fails and the wallet will be stored only on device.
        /// The saveToCloud flag is used to specify whether to save the wallet to cloud or not. When set to true, the wallet will be saved to cloud. When set to false, the wallet will be saved only on device.
        /// After the wallet is created, you can check the cloud backup status of the wallet using the walletBackedUpToCloud method.
        /// </summary>
        public async Task<Account> CreateAccountAsync(CreateAccountOptions options)
        {
            string mnemonic = await this.keyManager.GenerateNewMnemonic();
            return await SaveMnemonicAsync(mnemonic, options);
        }

        public async Task<Account> ImportExistingAccountAsync(string mnemonic, CreateAccountOptions options)
        {
            return await SaveMnemonicAsync(mnemonic, options);
        }

        /// <summary>
        /// Returns the cloud backup status of the existing wallet.
        /// Returns false if there is currently no wallet. This method should not be used as a check for wallet existence
        /// as it will return false if there is no wallet or if the wallet does exist but is not backed up to cloud.
        ///
        /// If a wallet already exists the reponse will be true or false depending on whether the wallet is backed up to cloud or not.
        /// TRUE response means wallet is backed up to cloud, FALSE means wallet is not backed up to cloud.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsWalletBackedUpToCloudAsync()
        {
            return await this.keyManager.IsMnemonicBackedUpToCloud();
        }

        public async Task<Account> GetAccountAsync()
        {
            if (this.currentAccount != null)
            {
                return this.currentAccount;
            }

            string mnemonic = await this.keyManager.GetMnemonic();
            if (string.IsNullOrEmpty(mnemonic))
            {
                return null;
            }

            Account account = await CreateAccountFromMnemonicAsync(mnemonic);
            this.currentAccount = account;
            return account;
        }

        public async Task<string> GetPublicAddressAsync()
        {
            Account account = await GetAccountAsync();
            if (account == null)
            {
                return null;
            }

            return account.Address;
        }

        public async Task PermanentlyDeleteAccountAsync()
        {
            await this.keyManager.DeleteMnemonic();
            this.currentAccount = null;
        }

        public async Task<string> GetAccountPhraseAsync()
        {
            try
            {
                return await this.keyManager.GetMnemonic();
            }
            catch
            {
                return null;
            }
        }

        public async Task<string> SignMessageAsync(string message)
        {
            Account account = await GetAccountAsync();
            if (account == null)
            {
                throw new Exception("No account");
            }

            EthereumMessageSigner signer = new();
            return signer.EncodeUTF8AndSign(message, new(account.PrivateKey));
        }

        public async Task<string> SignTransactionAsync<T>(T transaction)
            where T : SignedTypeTransaction
        {
            Account account = await GetAccountAsync();
            if (account == null)
            {
                throw new Exception("No account");
            }

            TypeTransactionSigner<T> signer = new();
            return signer.SignTransaction(account.PrivateKey, transaction);
        }

        private async Task<Account> SaveMnemonicAsync(string mnemonic, CreateAccountOptions options)
        {
            bool overwrite = options.Overwrite ?? false;
            Account existingAccount;
            try
            {
                existingAccount = await GetAccountAsync();
            }
            catch (Exception error)
            {
                if (!overwrite)
                {
                    Debug.LogError("Error while reading existing wallet");
                    Debug.LogException(error);
                }

                existingAccount = null;
            }

            if (existingAccount != null && !overwrite)
            {
                throw new System.Exception("Account already exists. Use overwrite flag to overwrite");
            }

            KeyStorageConfig storageOptions = options.StorageOptions ?? new KeyStorageConfig() { SaveToCloud = true, RejectOnCloudSaveFailure = false };

            // Get private key to check for a valid mnemonic first before passing anything invalid into saveMnemonic
            string privateKey = await this.keyManager.GetPrivateKeyFromMnemonic(mnemonic);
            await this.keyManager.SaveMnemonic(mnemonic, storageOptions);
            Account newAccount = new(privateKey);
            this.currentAccount = newAccount;

            return newAccount;
        }

        private async Task<Account> CreateAccountFromMnemonicAsync(string mnemonic)
        {
            string privateKey = await this.keyManager.GetPrivateKeyFromMnemonic(mnemonic);
            return new(privateKey);
        }

    }

}