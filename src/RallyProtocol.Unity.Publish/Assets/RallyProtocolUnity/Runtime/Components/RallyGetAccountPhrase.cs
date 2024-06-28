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

    [AddComponentMenu(AddComponentMenuNameBase + "/Get Account Phrase (Rally Protocol)")]
    public class RallyGetAccountPhrase : RallyBehaviour
    {

        #region Events

        public event EventHandler Retrieving;
        public event EventHandler<RallyAccountPhraseEventArgs> Retrieved;

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
                await GetAccountPhraseAsync();
            }
        }

        #endregion

        #region Protected Methods

        protected void OnRetrieving()
        {
            this.retrievingEvent?.Invoke();
            Retrieving?.Invoke(this, EventArgs.Empty);
        }

        protected void OnRetrieved(string phrase)
        {
            this.retrievedEvent?.Invoke(phrase);
            Retrieved?.Invoke(this, new(phrase));
        }

        #endregion

        #region Public Methods

        public async void GetAccountPhrase()
        {
            await GetAccountPhraseAsync();
        }

        public async UniTask GetAccountPhraseAsync()
        {
            OnRetrieving();
            IRallyNetwork rlyNetwork = RallyUnityManager.Instance.RlyNetwork;
            string phrase = await rlyNetwork.AccountManager.GetAccountPhraseAsync();
            OnRetrieved(phrase);
        }

        #endregion

    }

    public class RallyAccountPhraseEventArgs : EventArgs
    {

        public string Phrase { get; protected set; }

        public RallyAccountPhraseEventArgs(string phrase)
        {
            Phrase = phrase;
        }


    }

}