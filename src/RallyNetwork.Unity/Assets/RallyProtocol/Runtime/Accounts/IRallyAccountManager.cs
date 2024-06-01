using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using Nethereum.Model;
using Nethereum.Web3.Accounts;

using Account = Nethereum.Web3.Accounts.Account;

namespace RallyProtocol.Accounts
{

    /// <summary>
    /// Manages Rally's EOA wallet/account.
    /// </summary>
    public interface IRallyAccountManager
    {

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
        public Task<Account> CreateAccountAsync(CreateAccountOptions? options = null);

        /// <summary>
        /// Imports an existing mnemonic phrase and creates a wallet from that.
        /// </summary>
        /// <param name="mnemonic">The mnemonic phrase to import and create account from</param>
        /// <param name="options">The create account options</param>
        /// <returns>Returns a newly created account from the mnemonic</returns>
        public Task<Account> ImportExistingAccountAsync(string mnemonic, CreateAccountOptions options);

        /// <summary>
        /// Returns the cloud backup status of the existing wallet.
        /// Returns false if there is currently no wallet. This method should not be used as a check for wallet existence
        /// as it will return false if there is no wallet or if the wallet does exist but is not backed up to cloud.
        ///
        /// If a wallet already exists the reponse will be true or false depending on whether the wallet is backed up to cloud or not.
        /// TRUE response means wallet is backed up to cloud, FALSE means wallet is not backed up to cloud.
        /// </summary>
        /// <returns></returns>
        public Task<bool> IsWalletBackedUpToCloudAsync();

        /// <summary>
        /// Checks if there is any account currently loaded, if not, checks if there are any mnemonic stored and if there are no mnemonic stored, returns null, otherwise creates a new account based on the mnemonic and caches it for further calls.
        /// </summary>
        /// <returns>Returns the existing account or the newly created account</returns>
        public Task<Account?> GetAccountAsync();

        /// <summary>
        /// Gets account's public address.
        /// </summary>
        /// <returns>Returns the existing account's public address, otherwise null</returns>
        public Task<string?> GetPublicAddressAsync();

        /// <summary>
        /// Permanently deletes the account.
        /// </summary>
        public Task PermanentlyDeleteAccountAsync();

        /// <summary>
        /// Gets the account mnemonic/seed phrase.
        /// </summary>
        /// <returns>Returns the account mnemonic/seed phrase if it exists, otherwise null</returns>
        public Task<string?> GetAccountPhraseAsync();

        /// <summary>
        /// Signs the message using the existing account's private key.
        /// </summary>
        /// <param name="message">The message to sign</param>
        /// <returns>Returns the signed message</returns>
        /// <exception cref="System.Exception">Thrown when there is no existing account</exception>
        public Task<string> SignMessageAsync(string message);

        /// <summary>
        /// Signs the transaction using the existing account's private key.
        /// </summary>
        /// <typeparam name="T">The transaction type</typeparam>
        /// <param name="transaction">The transaction</param>
        /// <returns>Returns the signature</returns>
        /// <exception cref="System.Exception">Thrown when there is no existing account</exception>
        public Task<string> SignTransactionAsync<T>(T transaction)
            where T : SignedTypeTransaction;

        #endregion

    }

}