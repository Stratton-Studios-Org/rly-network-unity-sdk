using System;
using System.Collections;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Nethereum.Web3.Accounts;

using RallyProtocol.Accounts;
using RallyProtocol.Networks;

using UnityEngine;
using UnityEngine.Events;

namespace RallyProtocolUnity.Components
{

    [AddComponentMenu(AddComponentMenuNameBase + "/Initialize Account (Rally Protocol)")]
    public class RallyInitializeAccount : RallyBehaviour
    {

        #region Events

        public event EventHandler AccountCreated;
        public event EventHandler AccountLoaded;
        public event EventHandler Initialized;
        public event EventHandler NoAccountFound;

        #endregion

        #region Fields

        [Header("Initialization")]
        [SerializeField]
        protected bool initializeOnStart = true;

        [Header("Create Account")]
        [Tooltip("Whether to create a new account if there are no accounts")]
        [SerializeField]
        protected bool createAccount = true;
        [SerializeField]
        protected CreateAccountOptions createAccountOptions = new()
        {
            Overwrite = false,
            StorageOptions = new()
            {
                RejectOnCloudSaveFailure = true,
                SaveToCloud = true
            }
        };

        [Header("Events")]
        [SerializeField]
        protected UnityEvent accountLoadedEvent;
        [SerializeField]
        protected UnityEvent accountCreatedEvent;
        [SerializeField]
        protected UnityEvent initializedEvent;
        [SerializeField]
        protected UnityEvent noAccountFoundEvent;

        #endregion

        #region Unity Messages

        private async void Start()
        {
            if (this.initializeOnStart)
            {
                await InitializeAsync();
            }
        }

        #endregion

        #region Protected Methods

        protected void OnAccountCreated()
        {
            this.accountCreatedEvent?.Invoke();
            AccountCreated?.Invoke(this, EventArgs.Empty);
        }

        protected void OnAccountLoaded()
        {
            this.accountLoadedEvent?.Invoke();
            AccountLoaded?.Invoke(this, EventArgs.Empty);
        }

        protected void OnInitialized()
        {
            this.initializedEvent?.Invoke();
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        protected void OnNoAccountFound()
        {
            this.noAccountFoundEvent?.Invoke();
            NoAccountFound?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Public Methods

        public async void Initialize()
        {
            await InitializeAsync();
        }

        /// <summary>
        /// Initializes a new Rally account, gets the existing account if there are any, otherwise creates a new account using the <see cref="CreateAccountOptions"/> provided.
        /// </summary>
        public async UniTask InitializeAsync()
        {
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;

            // Load existing account if there are any
            Account account = await rlyNetwork.AccountManager.GetAccountAsync();

            // If there are no accounts, create a new one
            if (account == null)
            {
                if (this.createAccount)
                {
                    account = await rlyNetwork.AccountManager.CreateAccountAsync(this.createAccountOptions);
                    OnAccountCreated();
                }
                else
                {
                    OnNoAccountFound();
                    return;
                }
            }

            // Either newly created, or loaded
            if (account != null)
            {
                OnAccountLoaded();
            }

            OnInitialized();
        }

        #endregion

    }

}