using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Cysharp.Threading.Tasks;

using RallyProtocol;
using RallyProtocol.Networks;

using UnityEngine;
using UnityEngine.Events;

namespace RallyProtocolUnity.Components
{
    /// <summary>
    /// Unity component for transferring RLY or other ERC-20 tokens to specified addresses.
    /// </summary>
    /// <remarks>
    /// The RallyTransfer component provides a comprehensive and flexible interface for transferring tokens
    /// within a Unity application. It supports multiple transfer methods including decimal amounts for user-friendly
    /// transfers and exact BigInteger amounts for precise transfers.
    /// 
    /// This component can be configured through the Unity Inspector or manipulated through code, making it
    /// suitable for both designer-driven and programmer-driven workflows. It handles waiting for pending
    /// transfers to complete before starting new ones, preventing concurrent transfer issues.
    /// 
    /// Like other Rally Protocol components, it uses gasless transactions, so users don't need ETH or other
    /// native tokens to pay for transaction fees.
    /// </remarks>
    /// <example>
    /// Basic usage through Inspector:
    /// <code>
    /// // Add the component to a GameObject and configure in Inspector
    /// // Then trigger the transfer from a button click or other event
    /// public void OnTransferButtonClicked()
    /// {
    ///     GetComponent&lt;RallyTransfer&gt;().Transfer();
    /// }
    /// </code>
    /// 
    /// Code-based usage:
    /// <code>
    /// // Reference to the component
    /// [SerializeField] private RallyTransfer transferComponent;
    /// 
    /// // Transfer 10 tokens to a specific address
    /// public void SendTokens(string address)
    /// {
    ///     transferComponent.Transfer(address, 10.0m);
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RallyBehaviour"/>
    /// <seealso cref="RallyUnityManager"/>
    [AddComponentMenu(AddComponentMenuNameBase + "/Transfer (Rally Protocol)")]
    public class RallyTransfer : RallyBehaviour
    {

        #region Events

        /// <summary>
        /// Event that is triggered when a transfer operation begins.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when a token transfer starts,
        /// such as displaying a loading indicator or disabling UI elements during the operation.
        /// </remarks>
        public event EventHandler Transferring;

        /// <summary>
        /// Event that is triggered when a transfer operation completes successfully.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions after tokens have been successfully transferred,
        /// such as updating UI, playing animations, or triggering subsequent game logic.
        /// 
        /// The event args contain the transaction hash, which can be used to track the transaction on a blockchain explorer.
        /// </remarks>
        /// <seealso cref="RallyTransferEventArgs"/>
        public event EventHandler<RallyTransferEventArgs> Transferred;

        #endregion

        #region Fields

        /// <summary>
        /// The blockchain address to which tokens will be transferred.
        /// </summary>
        /// <remarks>
        /// This must be a valid Ethereum-compatible address in the format '0x...' with 40 hexadecimal characters.
        /// </remarks>
        [SerializeField]
        [Tooltip("The destination wallet address where tokens will be sent")]
        protected string destinationAddress = string.Empty;

        /// <summary>
        /// The amount of tokens to transfer, represented as a decimal string.
        /// </summary>
        /// <remarks>
        /// This value is used when calling the standard Transfer methods.
        /// It represents a human-readable token amount (e.g., "10.5" for 10.5 tokens).
        /// </remarks>
        [SerializeField]
        [Tooltip("The amount of tokens to transfer (as a decimal number)")]
        protected string amount = "0";

        /// <summary>
        /// The exact amount of tokens to transfer, represented as a BigInteger string.
        /// </summary>
        /// <remarks>
        /// This value is used when calling the TransferExact methods.
        /// It represents the raw token amount in the smallest unit (e.g., "10500000000000000000" for 10.5 tokens with 18 decimals).
        /// Use this for precise control over the transfer amount when decimal representation might cause rounding issues.
        /// </remarks>
        [SerializeField]
        [Tooltip("The exact amount of tokens to transfer in the smallest unit (as a BigInteger)")]
        protected string exactAmount = "0";

        /// <summary>
        /// Determines whether the component should automatically detect the appropriate meta-transaction method.
        /// </summary>
        /// <remarks>
        /// When set to true, the Rally Protocol will determine the best meta-transaction method to use based on the current
        /// network and contract configuration. When false, the method specified in <see cref="metaTxMethod"/> will be used.
        /// </remarks>
        [SerializeField]
        [Tooltip("If enabled, automatically detect the best meta-transaction method")]
        protected bool detectMetaTxMethod = false;

        /// <summary>
        /// The meta-transaction method to use for gasless transactions.
        /// </summary>
        /// <remarks>
        /// This setting is only used when <see cref="detectMetaTxMethod"/> is false.
        /// Different methods may be supported on different networks or for different token contracts.
        /// </remarks>
        [SerializeField]
        [Tooltip("The specific meta-transaction method to use (only used if auto-detection is disabled)")]
        protected MetaTxMethod metaTxMethod = MetaTxMethod.ExecuteMetaTransaction;

        /// <summary>
        /// The token contract address for the tokens to be transferred.
        /// </summary>
        /// <remarks>
        /// If left empty, the default RLY token address for the current network will be used.
        /// This field allows transferring non-RLY ERC-20 tokens that are supported by the Rally Protocol.
        /// </remarks>
        [SerializeField]
        [Tooltip("The token contract address (leave empty for RLY tokens)")]
        protected string tokenAddress = string.Empty;

        /// <summary>
        /// Unity event that is invoked when a transfer operation begins.
        /// </summary>
        /// <remarks>
        /// Use this to connect UI elements or other game logic in the Unity Editor without writing code.
        /// </remarks>
        [Header("Events")]
        [SerializeField]
        [Tooltip("Event triggered when a transfer operation begins")]
        protected UnityEvent transferringEvent;

        /// <summary>
        /// Unity event that is invoked when a transfer operation completes successfully.
        /// </summary>
        /// <remarks>
        /// The string parameter contains the transaction hash of the successful transfer.
        /// Use this to connect UI elements or other game logic in the Unity Editor without writing code.
        /// </remarks>
        [SerializeField]
        [Tooltip("Event triggered when a transfer completes, with transaction hash as parameter")]
        protected UnityEvent<string> transferredEvent;

        /// <summary>
        /// Task completion source for tracking pending transfers.
        /// </summary>
        /// <remarks>
        /// This is used internally to ensure that only one transfer operation happens at a time,
        /// preventing issues with concurrent blockchain transactions from the same account.
        /// </remarks>
        protected UniTaskCompletionSource pendingTransfer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the destination address for token transfers.
        /// </summary>
        /// <value>The Ethereum-compatible address where tokens will be sent.</value>
        public string DestinationAddress
        {
            get
            {
                return this.destinationAddress;
            }
            set
            {
                this.destinationAddress = value;
            }
        }

        /// <summary>
        /// Gets or sets the amount of tokens to transfer as a decimal string.
        /// </summary>
        /// <value>A string representing the decimal amount of tokens (e.g., "10.5").</value>
        public string Amount
        {
            get
            {
                return this.amount;
            }
            set
            {
                this.amount = value;
            }
        }

        /// <summary>
        /// Gets or sets the exact amount of tokens to transfer as a BigInteger string.
        /// </summary>
        /// <value>A string representing the exact token amount in the smallest unit (e.g., "10500000000000000000" for 10.5 tokens with 18 decimals).</value>
        public string ExactAmount
        {
            get
            {
                return this.exactAmount;
            }
            set
            {
                this.exactAmount = value;
            }
        }

        /// <summary>
        /// Gets or sets the meta-transaction method to use for gasless transactions.
        /// </summary>
        /// <value>The <see cref="MetaTxMethod"/> to use when <see cref="detectMetaTxMethod"/> is false.</value>
        public MetaTxMethod MetaTxMethod
        {
            get
            {
                return this.metaTxMethod;
            }
            set
            {
                this.metaTxMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets the token contract address for the tokens to be transferred.
        /// </summary>
        /// <value>The Ethereum-compatible address of the token contract, or empty for the default RLY token.</value>
        public string TokenAddress
        {
            get
            {
                return this.tokenAddress;
            }
            set
            {
                this.tokenAddress = value;
            }
        }

        /// <summary>
        /// Gets the meta-transaction method to use, considering the detection setting.
        /// </summary>
        /// <value>
        /// Null if <see cref="detectMetaTxMethod"/> is true (auto-detection),
        /// otherwise the value of <see cref="metaTxMethod"/>.
        /// </value>
        public MetaTxMethod? DetectOrMetaTxMethod => this.detectMetaTxMethod ? null : this.metaTxMethod;

        /// <summary>
        /// Gets the decimal representation of the <see cref="amount"/> string.
        /// </summary>
        /// <value>The amount as a decimal value.</value>
        public decimal AmountAsDecimal => decimal.Parse(amount);

        /// <summary>
        /// Gets the BigInteger representation of the <see cref="exactAmount"/> string.
        /// </summary>
        /// <value>The exact amount as a BigInteger value.</value>
        public BigInteger ExactAmountAsBigInteger => BigInteger.Parse(exactAmount);

        /// <summary>
        /// Gets the pending transfer task, if one exists.
        /// </summary>
        /// <value>The UniTask representing the current transfer operation, or null if no transfer is in progress.</value>
        public UniTask? PendingTransferTask => this.pendingTransfer != null ? this.pendingTransfer.Task : null;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Triggers the transferring events to notify listeners that a transfer operation has started.
        /// </summary>
        /// <remarks>
        /// This method is called internally when a transfer operation begins.
        /// It creates a new task completion source for tracking the transfer and triggers both the
        /// <see cref="Transferring"/> event and the <see cref="transferringEvent"/> UnityEvent.
        /// </remarks>
        protected void OnTransferring()
        {
            this.pendingTransfer = new();
            this.transferringEvent?.Invoke();
            Transferring?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the transferred events to notify listeners that a transfer operation has completed successfully.
        /// </summary>
        /// <param name="txHash">The transaction hash of the successful transfer operation.</param>
        /// <remarks>
        /// This method is called internally when a transfer operation completes successfully.
        /// It marks the pending transfer as complete and triggers both the
        /// <see cref="Transferred"/> event and the <see cref="transferredEvent"/> UnityEvent.
        /// </remarks>
        protected void OnTransferred(string txHash)
        {
            this.pendingTransfer.TrySetResult();
            this.transferredEvent?.Invoke(txHash);
            Transferred?.Invoke(this, new(txHash));
        }

        #endregion

        #region Transfer Methods (External Invocation)

        /// <summary>
        /// Initiates a token transfer using the component's configured settings.
        /// </summary>
        /// <remarks>
        /// This method uses the values set in the Inspector or via the component's properties:
        /// <list type="bullet">
        /// <item><description><see cref="destinationAddress"/> for the recipient</description></item>
        /// <item><description><see cref="amount"/> for the token amount</description></item>
        /// <item><description><see cref="DetectOrMetaTxMethod"/> for the transaction method</description></item>
        /// <item><description><see cref="tokenAddress"/> for the token contract (empty for RLY)</description></item>
        /// </list>
        /// </remarks>
        public async void Transfer()
        {
            await TransferAsync();
        }

        /// <summary>
        /// Initiates a token transfer to the specified address using the component's configured amount.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <remarks>
        /// Uses the configured <see cref="amount"/>, <see cref="DetectOrMetaTxMethod"/>, and <see cref="tokenAddress"/>.
        /// </remarks>
        public async void Transfer(string destinationAddress)
        {
            await TransferAsync(destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer of the specified amount to the component's configured address.
        /// </summary>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <remarks>
        /// Uses the configured <see cref="destinationAddress"/>, <see cref="DetectOrMetaTxMethod"/>, and <see cref="tokenAddress"/>.
        /// </remarks>
        public async void Transfer(decimal amount)
        {
            await TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer of the specified amount and token to the component's configured address.
        /// </summary>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="tokenAddress">The contract address of the token to transfer.</param>
        /// <remarks>
        /// Uses the configured <see cref="destinationAddress"/> and <see cref="DetectOrMetaTxMethod"/>.
        /// </remarks>
        public async void Transfer(decimal amount, string tokenAddress)
        {
            await TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer of the specified amount to the specified address.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <remarks>
        /// Uses the configured <see cref="DetectOrMetaTxMethod"/> and <see cref="tokenAddress"/>.
        /// </remarks>
        public async void Transfer(string destinationAddress, decimal amount)
        {
            await TransferAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer with the specified address, amount, and meta-transaction method.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="metaTxMethod">The meta-transaction method to use.</param>
        /// <remarks>
        /// Uses the configured <see cref="tokenAddress"/>.
        /// </remarks>
        public async void Transfer(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod)
        {
            await TransferAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a fully customized token transfer with all parameters specified.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="metaTxMethod">The meta-transaction method to use, or null for auto-detection.</param>
        /// <param name="tokenAddress">The contract address of the token to transfer.</param>
        /// <remarks>
        /// This is the most flexible transfer method, allowing complete customization of the transfer.
        /// </remarks>
        public async void Transfer(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod, string tokenAddress)
        {
            await TransferAsync(destinationAddress, amount, metaTxMethod, tokenAddress);
        }

        #endregion

        #region Transfer Methods

        /// <summary>
        /// Initiates a token transfer using the component's configured settings and returns a task that can be awaited.
        /// </summary>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// This method uses the values set in the Inspector or via the component's properties.
        /// It's the asynchronous equivalent of <see cref="Transfer()"/>.
        /// </remarks>
        public UniTask TransferAsync()
        {
            return TransferAsync(this.destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer to the specified address and returns a task that can be awaited.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// Uses the configured <see cref="amount"/>, <see cref="DetectOrMetaTxMethod"/>, and <see cref="tokenAddress"/>.
        /// It's the asynchronous equivalent of <see cref="Transfer(string)"/>.
        /// </remarks>
        public UniTask TransferAsync(string destinationAddress)
        {
            return TransferAsync(destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer of the specified amount and returns a task that can be awaited.
        /// </summary>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// Uses the configured <see cref="destinationAddress"/>, <see cref="DetectOrMetaTxMethod"/>, and <see cref="tokenAddress"/>.
        /// It's the asynchronous equivalent of <see cref="Transfer(decimal)"/>.
        /// </remarks>
        public UniTask TransferAsync(decimal amount)
        {
            return TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer of the specified amount and token and returns a task that can be awaited.
        /// </summary>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="tokenAddress">The contract address of the token to transfer.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// Uses the configured <see cref="destinationAddress"/> and <see cref="DetectOrMetaTxMethod"/>.
        /// It's the asynchronous equivalent of <see cref="Transfer(decimal, string)"/>.
        /// </remarks>
        public UniTask TransferAsync(decimal amount, string tokenAddress)
        {
            return TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer to the specified address with the specified amount and returns a task that can be awaited.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// Uses the configured <see cref="DetectOrMetaTxMethod"/> and <see cref="tokenAddress"/>.
        /// It's the asynchronous equivalent of <see cref="Transfer(string, decimal)"/>.
        /// </remarks>
        public UniTask TransferAsync(string destinationAddress, decimal amount)
        {
            return TransferAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a token transfer with the specified parameters and returns a task that can be awaited.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="metaTxMethod">The meta-transaction method to use.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// Uses the configured <see cref="tokenAddress"/>.
        /// It's the asynchronous equivalent of <see cref="Transfer(string, decimal, MetaTxMethod)"/>.
        /// </remarks>
        public UniTask TransferAsync(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod)
        {
            return TransferAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

        /// <summary>
        /// Initiates a fully customized token transfer and returns a task that can be awaited.
        /// </summary>
        /// <param name="destinationAddress">The recipient's blockchain address.</param>
        /// <param name="amount">The decimal amount of tokens to transfer.</param>
        /// <param name="metaTxMethod">The meta-transaction method to use, or null for auto-detection.</param>
        /// <param name="tokenAddress">The contract address of the token to transfer.</param>
        /// <returns>A <see cref="UniTask"/> that completes when the transfer operation finishes.</returns>
        /// <remarks>
        /// This is the core transfer method that all other transfer methods ultimately call.
        /// It handles waiting for any pending transfers to complete before starting a new one,
        /// and also manages the event triggers for transfer start and completion.
        /// 
        /// The method connects to the Rally Protocol network through the <see cref="RallyUnityManager"/>
        /// singleton and uses its transfer capabilities.
        /// 
        /// It's the asynchronous equivalent of <see cref="Transfer(string, decimal, MetaTxMethod?, string)"/>.
        /// </remarks>
        public async UniTask TransferAsync(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod, string tokenAddress)
        {
            if (this.pendingTransfer != null)
            {
                await this.pendingTransfer.Task;
            }

            OnTransferring();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.TransferAsync(destinationAddress, amount, metaTxMethod, tokenAddress);
            OnTransferred(txHash);
        }

        #endregion

        #region Transfer Exact Methods (External Invocation)

        public async void TransferExact()
        {
            await TransferExactAsync();
        }

        public async void TransferExact(string destinationAddress)
        {
            await TransferExactAsync(destinationAddress, ExactAmountAsBigInteger, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void TransferExact(BigInteger amount)
        {
            await TransferExactAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void TransferExact(BigInteger amount, string tokenAddress)
        {
            await TransferExactAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        public async void TransferExact(string destinationAddress, BigInteger amount)
        {
            await TransferExactAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod metaTxMethod)
        {
            await TransferExactAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

        public async void TransferExact(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod, string tokenAddress)
        {
            await TransferExactAsync(destinationAddress, amount, metaTxMethod, tokenAddress);
        }

        #endregion

        #region Transfer Exact Methods

        public UniTask TransferExactAsync()
        {
            return TransferExactAsync(this.destinationAddress, ExactAmountAsBigInteger, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferExactAsync(string destinationAddress)
        {
            return TransferExactAsync(destinationAddress, ExactAmountAsBigInteger, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferExactAsync(BigInteger amount)
        {
            return TransferExactAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferExactAsync(BigInteger amount, string tokenAddress)
        {
            return TransferExactAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        public UniTask TransferExactAsync(string destinationAddress, BigInteger amount)
        {
            return TransferExactAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod metaTxMethod)
        {
            return TransferExactAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

        public async UniTask TransferExactAsync(string destinationAddress, BigInteger amount, MetaTxMethod? metaTxMethod, string tokenAddress)
        {
            if (this.pendingTransfer != null)
            {
                await this.pendingTransfer.Task;
            }

            OnTransferring();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string txHash = await rlyNetwork.TransferExactAsync(destinationAddress, amount, metaTxMethod, tokenAddress);
            OnTransferred(txHash);
        }

        #endregion

    }

    /// <summary>
    /// Event arguments for a successful token transfer operation.
    /// </summary>
    /// <remarks>
    /// This class encapsulates information about a successful token transfer operation,
    /// providing the transaction hash for reference or display to the user.
    /// </remarks>
    /// <seealso cref="RallyTransfer"/>
    public class RallyTransferEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the transaction hash of the successful transfer operation.
        /// </summary>
        /// <value>
        /// The transaction hash as a string, which can be used to look up the transaction on a blockchain explorer.
        /// </value>
        public string TransactionHash { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RallyTransferEventArgs"/> class with the specified transaction hash.
        /// </summary>
        /// <param name="txHash">The transaction hash of the successful transfer operation.</param>
        public RallyTransferEventArgs(string txHash)
        {
            TransactionHash = txHash;
        }
    }

}