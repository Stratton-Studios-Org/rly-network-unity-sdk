using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using Cysharp.Threading.Tasks;

using RallyProtocol;
using RallyProtocol.Networks;

using RallyProtocolUnity.Logging;

using UnityEngine;
using UnityEngine.Events;

namespace RallyProtocolUnity.Components
{

    [AddComponentMenu(AddComponentMenuNameBase + "/Get Balance (Rally Protocol)")]
    public class RallyGetBalance : RallyBehaviour
    {

        #region Events

        public event EventHandler ExactBalanceRetrieving;
        public event EventHandler<RallyGetExactBalanceEventArgs> ExactBalanceRetrieved;

        public event EventHandler DisplayBalanceRetrieving;
        public event EventHandler<RallyGetDisplayBalanceEventArgs> DisplayBalanceRetrieved;

        #endregion

        #region Fields

        [SerializeField]
        protected bool getDisplayBalanceOnStart = true;
        [SerializeField]
        protected bool getExactBalanceOnStart = false;

        [Header("Events")]
        [SerializeField]
        protected UnityEvent<string> displayBalanceUpdateString;
        [SerializeField]
        protected UnityEvent<string> exactBalanceUpdateString;

        #endregion

        #region Unity Messages

        protected async void Start()
        {
            if (this.getDisplayBalanceOnStart)
            {
                await GetDisplayBalanceAsync();
            }

            if (this.getExactBalanceOnStart)
            {
                await GetExactBalanceAsync();
            }
        }

        #endregion

        #region Protected Methods

        protected void OnExactBalanceRetrieving()
        {
            ExactBalanceRetrieving?.Invoke(this, EventArgs.Empty);
        }

        protected void OnExactBalanceRetrieved(BigInteger exactBalance)
        {
            this.exactBalanceUpdateString?.Invoke(exactBalance.ToString());
            ExactBalanceRetrieved?.Invoke(this, new(exactBalance));
        }

        protected void OnDisplayBalanceRetrieving()
        {
            DisplayBalanceRetrieving?.Invoke(this, EventArgs.Empty);
        }

        protected void OnDisplayBalanceRetrieved(decimal displayBalance)
        {
            this.displayBalanceUpdateString?.Invoke(displayBalance.ToString());
            DisplayBalanceRetrieved?.Invoke(this, new(displayBalance));
        }

        #endregion

        #region Public Methods

        public async void GetExactBalance()
        {
            await GetExactBalanceAsync();
        }

        public async void GetDisplayBalance()
        {
            await GetDisplayBalanceAsync();
        }

        public async UniTask GetExactBalanceAsync()
        {
            OnExactBalanceRetrieving();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            BigInteger exactBalance = await rlyNetwork.GetExactBalanceAsync();
            OnExactBalanceRetrieved(exactBalance);
        }

        public async UniTask GetDisplayBalanceAsync()
        {
            OnDisplayBalanceRetrieving();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            decimal displayBalance = await rlyNetwork.GetDisplayBalanceAsync();
            OnDisplayBalanceRetrieved(displayBalance);
        }

        #endregion

    }

    public class RallyGetExactBalanceEventArgs : EventArgs
    {

        public BigInteger ExactBalance { get; protected set; }

        public RallyGetExactBalanceEventArgs(BigInteger exactBalance)
        {
            ExactBalance = exactBalance;
        }

    }

    public class RallyGetDisplayBalanceEventArgs : EventArgs
    {

        public decimal DisplayBalance { get; protected set; }

        public RallyGetDisplayBalanceEventArgs(decimal displayBalance)
        {
            DisplayBalance = displayBalance;
        }

    }

}