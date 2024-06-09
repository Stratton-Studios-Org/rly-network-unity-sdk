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

    [AddComponentMenu("Rally Protocol/Get Account Public Address")]
    public class RallyGetAccountPublicAddress : MonoBehaviour
    {

        #region Events

        public event EventHandler Retrieving;
        public event EventHandler<RallyAccountPublicAddressEventArgs> Retrieved;

        #endregion

        #region Fields

        [SerializeField]
        protected bool getOnStart = true;

        [Header("Events")]
        [SerializeField]
        protected UnityEvent retrievingEvent;
        [SerializeField]
        protected UnityEvent<string> retrievedEvent;

        #endregion

        #region Unity Messages

        protected async void Start()
        {
            if (this.getOnStart)
            {
                await GetPublicAddressAsync();
            }
        }

        #endregion

        #region Protected Methods

        protected void OnRetrieving()
        {
            this.retrievingEvent?.Invoke();
            Retrieving?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRetrieved(string publicAddress)
        {
            this.retrievedEvent?.Invoke(publicAddress);
            Retrieved?.Invoke(this, new(publicAddress));
        }

        #endregion

        #region Public Methods

        public async void GetPublicAddress()
        {
            await GetPublicAddressAsync();
        }

        public async UniTask GetPublicAddressAsync()
        {
            OnRetrieving();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string publicAddress = await rlyNetwork.AccountManager.GetPublicAddressAsync();
            OnRetrieved(publicAddress);
        }

        #endregion

    }

    public class RallyAccountPublicAddressEventArgs : EventArgs
    {

        public string PublicAddress { get; protected set; }

        public RallyAccountPublicAddressEventArgs(string publicAddress)
        {
            PublicAddress = publicAddress;
        }


    }

}