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

    [AddComponentMenu(AddComponentMenuNameBase + "/Transfer (Rally Protocol)")]
    public class RallyTransfer : RallyBehaviour
    {

        #region Events

        public event EventHandler Transferring;
        public event EventHandler<RallyTransferEventArgs> Transferred;

        #endregion

        #region Fields

        [SerializeField]
        protected string destinationAddress = string.Empty;
        [SerializeField]
        protected string amount = "0";
        [SerializeField]
        protected string exactAmount = "0";
        [SerializeField]
        protected bool detectMetaTxMethod = false;
        [SerializeField]
        protected MetaTxMethod metaTxMethod = MetaTxMethod.ExecuteMetaTransaction;
        [SerializeField]
        protected string tokenAddress = string.Empty;

        [Header("Events")]
        [SerializeField]
        protected UnityEvent transferringEvent;
        [SerializeField]
        protected UnityEvent<string> transferredEvent;

        protected UniTaskCompletionSource pendingTransfer;

        #endregion

        #region Properties

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

        public MetaTxMethod? DetectOrMetaTxMethod => this.detectMetaTxMethod ? null : this.metaTxMethod;

        public decimal AmountAsDecimal => decimal.Parse(amount);

        public BigInteger ExactAmountAsBigInteger => BigInteger.Parse(exactAmount);

        public UniTask? PendingTransferTask => this.pendingTransfer != null ? this.pendingTransfer.Task : null;

        #endregion

        #region Protected Methods

        protected void OnTransferring()
        {
            this.pendingTransfer = new();
            this.transferringEvent?.Invoke();
            Transferring?.Invoke(this, EventArgs.Empty);
        }

        protected void OnTransferred(string txHash)
        {
            this.pendingTransfer.TrySetResult();
            this.transferredEvent?.Invoke(txHash);
            Transferred?.Invoke(this, new(txHash));
        }

        #endregion

        #region Transfer Methods (External Invocation)

        public async void Transfer()
        {
            await TransferAsync();
        }

        public async void Transfer(string destinationAddress)
        {
            await TransferAsync(destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void Transfer(decimal amount)
        {
            await TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void Transfer(decimal amount, string tokenAddress)
        {
            await TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        public async void Transfer(string destinationAddress, decimal amount)
        {
            await TransferAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public async void Transfer(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod)
        {
            await TransferAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

        public async void Transfer(string destinationAddress, decimal amount, MetaTxMethod? metaTxMethod, string tokenAddress)
        {
            await TransferAsync(destinationAddress, amount, metaTxMethod, tokenAddress);
        }

        #endregion

        #region Transfer Methods

        public UniTask TransferAsync()
        {
            return TransferAsync(this.destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferAsync(string destinationAddress)
        {
            return TransferAsync(destinationAddress, AmountAsDecimal, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferAsync(decimal amount)
        {
            return TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferAsync(decimal amount, string tokenAddress)
        {
            return TransferAsync(this.destinationAddress, amount, DetectOrMetaTxMethod, tokenAddress);
        }

        public UniTask TransferAsync(string destinationAddress, decimal amount)
        {
            return TransferAsync(destinationAddress, amount, DetectOrMetaTxMethod, this.tokenAddress);
        }

        public UniTask TransferAsync(string destinationAddress, decimal amount, MetaTxMethod metaTxMethod)
        {
            return TransferAsync(destinationAddress, amount, metaTxMethod, this.tokenAddress);
        }

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

    public class RallyTransferEventArgs : EventArgs
    {

        public string TransactionHash { get; protected set; }

        public RallyTransferEventArgs(string txHash)
        {
            TransactionHash = txHash;
        }

    }

}