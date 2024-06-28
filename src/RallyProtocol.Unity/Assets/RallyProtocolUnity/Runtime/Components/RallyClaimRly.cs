using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using RallyProtocol;
using RallyProtocol.Networks;

using RallyProtocolUnity.Logging;

using UnityEngine;
using UnityEngine.Events;

namespace RallyProtocolUnity.Components
{

    [AddComponentMenu(AddComponentMenuNameBase + "/Claim RLY (Rally Protocol)")]
    public class RallyClaimRly : RallyBehaviour
    {

        #region Events

        public event EventHandler Claiming;
        public event EventHandler<RallyClaimRlyEventArgs> Claimed;

        #endregion

        #region Fields

        [SerializeField]
        protected bool claimRlyOnStart = true;

        [Header("Events")]
        [SerializeField]
        protected UnityEvent claimingEvent;
        [SerializeField]
        protected UnityEvent<string> claimedEvent;

        #endregion

        #region Unity Messages

        protected async void Start()
        {
            if (this.claimRlyOnStart)
            {
                await ClaimRlyAsync();
            }
        }

        #endregion

        #region Protected Methods

        protected void OnClaiming()
        {
            this.claimingEvent?.Invoke();
            Claiming?.Invoke(this, EventArgs.Empty);
        }

        protected void OnClaimed(string txHash)
        {
            this.claimedEvent?.Invoke(txHash);
            Claimed?.Invoke(this, new(txHash));
        }

        #endregion

        #region Public Methods

        public async void ClaimRly()
        {
            await ClaimRlyAsync();
        }

        public async UniTask ClaimRlyAsync()
        {
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            try
            {
                OnClaiming();
                string txHash = await rlyNetwork.ClaimRlyAsync();
                OnClaimed(txHash);
            }
            catch (PriorDustingException)
            {
                return; // Already claimed
            }
            catch (System.Exception ex)
            {
                rlyNetwork.Logger.LogException(ex);
            }
        }

        #endregion

    }

    public class RallyClaimRlyEventArgs : EventArgs
    {

        public string TransactionHash { get; protected set; }

        public RallyClaimRlyEventArgs(string txHash)
        {
            TransactionHash = txHash;
        }

    }

}