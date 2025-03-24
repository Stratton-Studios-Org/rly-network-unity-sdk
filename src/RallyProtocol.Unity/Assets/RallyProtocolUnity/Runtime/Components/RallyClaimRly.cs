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
    /// <summary>
    /// Unity component that facilitates claiming RLY tokens in a game or application.
    /// </summary>
    /// <remarks>
    /// The RallyClaimRly component provides a convenient way to enable players to claim RLY tokens
    /// within a Unity application. This component can be configured to automatically claim RLY
    /// tokens when the game starts, or it can be triggered manually through code.
    /// 
    /// This component uses gasless transactions via the Rally Protocol network, eliminating the need
    /// for users to have ETH or other native tokens to pay for gas fees.
    /// 
    /// Note that RLY token claiming is subject to:
    /// - User having a valid account
    /// - Network-specific claiming rules (cooldown periods, maximum amounts, etc.)
    /// - API key permissions
    /// </remarks>
    /// <example>
    /// Basic usage - automatically claim on start:
    /// <code>
    /// // Add the component to a GameObject
    /// var claimComponent = gameObject.AddComponent&lt;RallyClaimRly&gt;();
    /// claimComponent.claimRlyOnStart = true;
    /// </code>
    /// 
    /// Claim manually through code:
    /// <code>
    /// // Reference to the component
    /// [SerializeField] private RallyClaimRly claimComponent;
    /// 
    /// // Call the claim method when needed
    /// public void OnClaimButtonPressed()
    /// {
    ///     claimComponent.ClaimRly();
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="RallyBehaviour"/>
    /// <seealso cref="RallyUnityManager"/>
    [AddComponentMenu(AddComponentMenuNameBase + "/Claim RLY (Rally Protocol)")]
    public class RallyClaimRly : RallyBehaviour
    {

        #region Events

        /// <summary>
        /// Event that is triggered when the claiming process begins.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions when the token claiming process starts,
        /// such as displaying a loading indicator to the user.
        /// </remarks>
        public event EventHandler Claiming;

        /// <summary>
        /// Event that is triggered when the claiming process completes successfully.
        /// </summary>
        /// <remarks>
        /// Subscribe to this event to perform actions after tokens have been successfully claimed,
        /// such as updating UI, playing animations, or granting in-game rewards.
        /// 
        /// The event args contain the transaction hash, which can be used to track the transaction on a blockchain explorer.
        /// </remarks>
        /// <seealso cref="RallyClaimRlyEventArgs"/>
        public event EventHandler<RallyClaimRlyEventArgs> Claimed;

        #endregion

        #region Fields

        /// <summary>
        /// Determines whether to automatically claim RLY tokens when the component starts.
        /// </summary>
        /// <remarks>
        /// When set to true, the component will attempt to claim RLY tokens during the Start method.
        /// This is useful for applications that want to give users tokens as soon as they start the game.
        /// 
        /// If set to false, claiming must be triggered manually by calling <see cref="ClaimRly"/> or <see cref="ClaimRlyAsync"/>.
        /// </remarks>
        [SerializeField]
        [Tooltip("If enabled, will automatically attempt to claim RLY tokens when the component starts")]
        protected bool claimRlyOnStart = true;

        /// <summary>
        /// Unity event that is invoked when the claiming process begins.
        /// </summary>
        /// <remarks>
        /// Use this to connect UI elements or other game logic in the Unity Editor without writing code.
        /// </remarks>
        [Header("Events")]
        [SerializeField]
        [Tooltip("Event triggered when the claiming process begins")]
        protected UnityEvent claimingEvent;

        /// <summary>
        /// Unity event that is invoked when the claiming process completes successfully.
        /// </summary>
        /// <remarks>
        /// The string parameter contains the transaction hash of the successful claim.
        /// Use this to connect UI elements or other game logic in the Unity Editor without writing code.
        /// </remarks>
        [SerializeField]
        [Tooltip("Event triggered when the claiming process completes, with transaction hash as parameter")]
        protected UnityEvent<string> claimedEvent;

        #endregion

        #region Unity Messages

        /// <summary>
        /// Unity's Start method. If <see cref="claimRlyOnStart"/> is true, this will automatically
        /// attempt to claim RLY tokens.
        /// </summary>
        protected async void Start()
        {
            if (this.claimRlyOnStart)
            {
                await ClaimRlyAsync();
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Triggers the claiming events to notify listeners that a claim operation has started.
        /// </summary>
        /// <remarks>
        /// This method is called internally when a claim operation begins.
        /// It triggers both the <see cref="Claiming"/> event and the <see cref="claimingEvent"/> UnityEvent.
        /// </remarks>
        protected void OnClaiming()
        {
            this.claimingEvent?.Invoke();
            Claiming?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Triggers the claimed events to notify listeners that a claim operation has completed successfully.
        /// </summary>
        /// <param name="txHash">The transaction hash of the successful claim operation.</param>
        /// <remarks>
        /// This method is called internally when a claim operation completes successfully.
        /// It triggers both the <see cref="Claimed"/> event and the <see cref="claimedEvent"/> UnityEvent.
        /// </remarks>
        protected void OnClaimed(string txHash)
        {
            this.claimedEvent?.Invoke(txHash);
            Claimed?.Invoke(this, new(txHash));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initiates the process of claiming RLY tokens.
        /// </summary>
        /// <remarks>
        /// This is a non-blocking call that will execute the claim process asynchronously.
        /// Events will be triggered to notify listeners of the claim status.
        /// 
        /// If the user has already claimed tokens recently, the operation will silently complete
        /// without triggering the Claimed event (due to PriorDustingException).
        /// </remarks>
        /// <seealso cref="ClaimRlyAsync"/>
        public async void ClaimRly()
        {
            await ClaimRlyAsync();
        }

        /// <summary>
        /// Initiates the process of claiming RLY tokens and returns a task that can be awaited.
        /// </summary>
        /// <returns>A <see cref="UniTask"/> that completes when the claim operation finishes.</returns>
        /// <remarks>
        /// This method attempts to claim RLY tokens for the current user through the Rally Protocol network.
        /// It will trigger events to notify listeners of the claim status.
        /// 
        /// Exceptions are handled internally:
        /// - PriorDustingException: Silently handled (user already claimed recently)
        /// - Other exceptions: Logged but not rethrown
        /// 
        /// For better error handling in your application, you may want to implement a custom version
        /// of this method that handles exceptions according to your needs.
        /// </remarks>
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

    /// <summary>
    /// Event arguments for the successful claiming of RLY tokens.
    /// </summary>
    /// <remarks>
    /// This class encapsulates information about a successful RLY token claim operation,
    /// providing the transaction hash for reference or display to the user.
    /// </remarks>
    /// <seealso cref="RallyClaimRly"/>
    public class RallyClaimRlyEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the transaction hash of the successful claim operation.
        /// </summary>
        /// <value>
        /// The transaction hash as a string, which can be used to look up the transaction on a blockchain explorer.
        /// </value>
        public string TransactionHash { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RallyClaimRlyEventArgs"/> class with the specified transaction hash.
        /// </summary>
        /// <param name="txHash">The transaction hash of the successful claim operation.</param>
        public RallyClaimRlyEventArgs(string txHash)
        {
            TransactionHash = txHash;
        }

    }

}