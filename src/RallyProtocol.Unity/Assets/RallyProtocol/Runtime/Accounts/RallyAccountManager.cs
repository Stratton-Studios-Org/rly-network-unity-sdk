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

using RallyProtocol.Core;
using RallyProtocol.Logging;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.Accounts
{

    /// <summary>
    /// The default implementation of <see cref="IRallyAccountManager"/>.
    /// </summary>
    public class RallyAccountManager : IRallyAccountManager
    {

        #region Fields

        /// <summary>
        /// Current active account.
        /// </summary>
        protected Account? currentAccount;

        /// <summary>
        /// The custom logger.
        /// </summary>
        protected IRallyLogger logger;

        /// <summary>
        /// The platform's key manager.
        /// </summary>
        protected IPlatformKeyManager keyManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently active account.
        /// </summary>
        public Account? CurrentAccount => this.currentAccount;

        /// <summary>
        /// Gets the logger instance used.
        /// </summary>
        public IRallyLogger Logger => this.logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="RallyAccountManager"/>
        /// </summary>
        /// <param name="keyManager">The platform key manager</param>
        /// <param name="logger">The logger</param>
        public RallyAccountManager(IPlatformKeyManager keyManager, IRallyLogger logger)
        {
            this.keyManager = keyManager;
            this.logger = logger;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Saves the mnemonic to disk/cloud based on the platform using <see cref="IPlatformKeyManager"/>.
        /// </summary>
        /// <param name="mnemonic">The mnemonic to save</param>
        /// <param name="options">The options</param>
        /// <returns>Returns the newly created account based on the mnemonic before storage</returns>
        /// <exception cref="System.Exception">Thrown when the account already exists and the <see cref="CreateAccountOptions.Overwrite"/> flag is not set</exception>
        private async Task<Account> SaveMnemonicAsync(string mnemonic, CreateAccountOptions options)
        {
            bool overwrite = options.Overwrite ?? false;
            Account? existingAccount;
            try
            {
                existingAccount = await GetAccountAsync();
            }
            catch (Exception error)
            {
                if (!overwrite)
                {
                    Logger.LogError("Error while reading existing wallet");
                    Logger.LogException(error);
                }

                existingAccount = null;
            }

            if (existingAccount != null && !overwrite)
            {
                throw new RallyAccountExistsException();
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

        #endregion

        #region Public Methods

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
        public async Task<Account> CreateAccountAsync(CreateAccountOptions? options = null)
        {
            if (options == null)
            {
                options = new();
            }

            string mnemonic = await this.keyManager.GenerateNewMnemonic();
            return await SaveMnemonicAsync(mnemonic, options);
        }

        /// <summary>
        /// Imports an existing mnemonic phrase and creates a wallet from that.
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase to import and create account from</param>
        /// <param name="options">The create account options</param>
        /// <returns>Returns a newly created account from the mnemonic</returns>
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
            return await this.keyManager.IsMnemonicEligibleForCloudSync();
        }

        /// <summary>
        /// Checks if there is any account currently loaded, if not, checks if there are any mnemonic stored and if there are no mnemonic stored, returns null, otherwise creates a new account based on the mnemonic and caches it for further calls.
        /// </summary>
        /// <returns>Returns the existing account or the newly created account</returns>
        public async Task<Account?> GetAccountAsync()
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

        /// <summary>
        /// Gets account's public address.
        /// </summary>
        /// <returns>Returns the existing account's public address, otherwise null</returns>
        public async Task<string?> GetPublicAddressAsync()
        {
            Account? account = await GetAccountAsync();
            if (account == null)
            {
                return null;
            }

            return account.Address;
        }

        /// <summary>
        /// Permanently deletes the account.
        /// </summary>
        public async Task PermanentlyDeleteAccountAsync()
        {
            await this.keyManager.DeleteMnemonic();
            this.currentAccount = null;
        }

        /// <summary>
        /// Gets the account mnemonic/seed phrase.
        /// </summary>
        /// <returns>Returns the account mnemonic/seed phrase if it exists, otherwise null</returns>
        public async Task<string?> GetAccountPhraseAsync()
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

        /// <summary>
        /// Signs the message using the existing account's private key.
        /// </summary>
        /// <param name="message">The message to sign</param>
        /// <returns>Returns the signed message</returns>
        /// <exception cref="Exception">Thrown when there is no existing account</exception>
        public async Task<string> SignMessageAsync(string message)
        {
            Account? account = await GetAccountAsync();
            if (account == null)
            {
                throw new RallyNoAccountException();
            }

            EthereumMessageSigner signer = new();
            return signer.EncodeUTF8AndSign(message, new(account.PrivateKey));
        }

        /// <summary>
        /// Signs the transaction using the existing account's private key.
        /// </summary>
        /// <typeparam name="T">The transaction type</typeparam>
        /// <param name="transaction">The transaction</param>
        /// <returns>Returns the signature</returns>
        /// <exception cref="Exception">Thrown when there is no existing account</exception>
        public async Task<string> SignTransactionAsync<T>(T transaction)
            where T : SignedTypeTransaction
        {
            Account? account = await GetAccountAsync();
            if (account == null)
            {
                throw new RallyNoAccountException();
            }

            TypeTransactionSigner<T> signer = new();
            return signer.SignTransaction(account.PrivateKey, transaction);
        }

        #endregion

    }

}