using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;

using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using RallyProtocolUnity.Accounts;
using RallyProtocolUnity.Components;
using RallyProtocolUnity.Logging;
using RallyProtocolUnity.Networks;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace RallyProtocol.Samples
{

    public class RallyExample : MonoBehaviour
    {

        #region Constants

        public const string NftContractAddress = "0xe7d4b732df200fefb28e5bb1f2cac129155f511a";

        #endregion

        #region Fields

        [SerializeField]
        protected CanvasGroup canvasGroup;
        [SerializeField]
        protected TMP_Text infoText;

        [SerializeField]
        protected TMP_InputField recipientField;
        [SerializeField]
        protected TMP_InputField amountField;

        protected IRallyAccountManager accountManager;
        protected IRallyNetwork rlyNetwork;

        #endregion

        #region Unity Messages

        async void Start()
        {
            this.canvasGroup.interactable = false;

            // You can set the logging filter using this in Unity
            RallyUnityLogger.Default.UnityLogger.filterLogType = LogType.Log;

            // Load account
            this.accountManager = RallyUnityAccountManager.Default;
            Account account = await this.accountManager.GetAccountAsync();
            if (account != null)
            {
                Debug.Log("Account loaded successfully");
            }
            else
            {
                account = await this.accountManager.CreateAccountAsync();
                Debug.Log("Account created successfully");
            }

            Debug.Log("Initializing Rally network...");

            // Retrieve the Rally network instance from the Main settings preset created by Window > Rally Protocol > Setup window
            this.rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            Debug.Log("Initialized Rally network");
            await UpdateInfoText();
            this.canvasGroup.interactable = true;
        }

        #endregion

        #region Protected Methods

        protected async Task UpdateInfoText()
        {
            try
            {
                Account account = await this.accountManager.GetAccountAsync();
                string mnemonic = await this.accountManager.GetAccountPhraseAsync();
                string address = await this.accountManager.GetPublicAddressAsync();
                decimal balance = await this.rlyNetwork.GetDisplayBalanceAsync();
                BigInteger exactBalance = await this.rlyNetwork.GetExactBalanceAsync();
                bool backedUp = await this.accountManager.IsWalletBackedUpToCloudAsync();

                this.infoText.text = $"Mnemonic: {mnemonic}<br>Address: {address}<br>Balance: {balance}<br>Exact Balance: {exactBalance}<br>Backed Up: {backedUp}";
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        #endregion

        #region Public Methods

        public async void CreateAccount()
        {
            this.canvasGroup.interactable = false;
            try
            {
                Debug.Log("Creating account...");

                // Create a new account & overwrite everytime for testing purposes
                await this.accountManager.CreateAccountAsync(new() { Overwrite = true });
                Debug.Log("Account created successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Account creation failed");
                Debug.LogException(ex);
            }

            await UpdateInfoText();
            this.canvasGroup.interactable = true;
        }

        public async void ClaimRly()
        {
            this.canvasGroup.interactable = false;
            try
            {
                Debug.Log("Claiming RLY...");
                await this.rlyNetwork.ClaimRlyAsync();
                Debug.Log("Claimed RLY successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError("Claiming RLY failed");
                Debug.LogException(ex);
            }

            await UpdateInfoText();
            this.canvasGroup.interactable = true;
        }

        public async void MintNft()
        {
            this.canvasGroup.interactable = false;
            Web3 provider = await this.rlyNetwork.GetProviderAsync();
            string walletAddress = await this.rlyNetwork.AccountManager.GetPublicAddressAsync();
            NFT nft = new NFT(NftContractAddress, walletAddress, provider);
            int nextNFTId = await nft.GetCurrentNFTIdAsync();
            GsnTransactionDetails gsnTx = await nft.GetMinftNFTTx();
            try
            {
                string txHash = await this.rlyNetwork.RelayAsync(gsnTx);
                string tokenURI = await nft.GetTokenURIAsync(nextNFTId);
                string[] parts = tokenURI.Split(',');
                string base64Data = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));

                Dictionary<string, string> json = JsonConvert.DeserializeObject<Dictionary<string, string>>(base64Data);
                string imageData = json!["image"].Split(",")[1];

                string text = Encoding.UTF8.GetString(Convert.FromBase64String(imageData));
                Debug.Log($"NFT Image text: {text}");
                Debug.Log($"NFT Image data: {imageData}");
            }
            catch (Exception ex)
            {
                Debug.LogError("Miting NFT failed");
                Debug.LogException(ex);
            }

            this.canvasGroup.interactable = true;
        }

        public async void Transfer()
        {
            this.canvasGroup.interactable = false;
            try
            {
                await this.rlyNetwork.TransferAsync(this.recipientField.text, decimal.Parse(this.amountField.text), MetaTxMethod.ExecuteMetaTransaction);
                await UpdateInfoText();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            this.canvasGroup.interactable = true;
        }

        #endregion

    }

}