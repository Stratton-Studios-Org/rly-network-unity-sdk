using System;
using System.Threading.Tasks;

using RallyProtocol.Core;

using UnityEngine;

namespace RallyProtocolUnity.Storage
{

    public class AndroidKeyManager : IPlatformKeyManager
    {

        #region Constants

        const string UNITY_SDK_PLUGIN_CLASS = "com.rlynetworkmobilesdk.UnitySdkPlugin";

        #endregion

        #region Definitions

        class ResultCallback<T> : AndroidJavaProxy
        {

            #region Fields

            readonly AndroidKeyManager _plugin;
            readonly Action<T> _onSuccess;
            readonly Action<string> _onError;

            #endregion

            #region Constructors

            public ResultCallback(AndroidKeyManager plugin, Action<T> onSuccess, Action<string> onError) : base($"{UNITY_SDK_PLUGIN_CLASS}$ResultCallback")
            {
                _plugin = plugin;
                _onSuccess = onSuccess;
                _onError = onError;
            }

            #endregion

            #region Public Methods

#pragma warning disable IDE1006
            public void onSuccess(T result) => _onSuccess?.Invoke(result);
            public void onError(string error) => _onError?.Invoke(error);
#pragma warning restore IDE1006

            #endregion

        }

        #endregion

        #region Fields

        private readonly AndroidJavaObject currentActivity;
        private readonly AndroidJavaObject pluginInstance;

        #endregion

        #region Constructors

        public AndroidKeyManager()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                throw new InvalidOperationException($"{nameof(AndroidKeyManager)} can only be used on Android");
            }

            using AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            using AndroidJavaClass pluginClass = new(UNITY_SDK_PLUGIN_CLASS);
            pluginInstance = new AndroidJavaObject(UNITY_SDK_PLUGIN_CLASS, currentActivity);
        }

        #endregion

        #region Public Methods

        public Task<string> GetBundleId()
        {
            TaskCompletionSource<string> tcs = new();

            pluginInstance.Call("getBundleId", new ResultCallback<string>(this, bundleId => tcs.SetResult(bundleId), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<string> GetMnemonic()
        {
            TaskCompletionSource<string> tcs = new();

            pluginInstance.Call("getMnemonic", new ResultCallback<string>(this, mnemonic => tcs.SetResult(mnemonic), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<string> GenerateNewMnemonic()
        {
            TaskCompletionSource<string> tcs = new();

            pluginInstance.Call("generateNewMnemonic", new ResultCallback<string>(this, mnemonic => tcs.SetResult(mnemonic), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<bool> SaveMnemonic(string mnemonic, KeyStorageConfig options = null)
        {
            TaskCompletionSource<bool> tcs = new();

            pluginInstance.Call("saveMnemonic", mnemonic, options.SaveToCloud.GetValueOrDefault(), options.RejectOnCloudSaveFailure.GetValueOrDefault(), new ResultCallback<bool>(this, result => tcs.SetResult(result), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<bool> DeleteMnemonic()
        {
            TaskCompletionSource<bool> tcs = new();

            pluginInstance.Call("deleteMnemonic", new ResultCallback<bool>(this, result => tcs.SetResult(result), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<bool> IsMnemonicBackedUpToCloud()
        {
            TaskCompletionSource<bool> tcs = new();

            pluginInstance.Call("mnemonicBackedUpToCloud", new ResultCallback<bool>(this, result => tcs.SetResult(result), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        public Task<string> GetPrivateKeyFromMnemonic(string mnemonic)
        {
            TaskCompletionSource<string> tcs = new();

            pluginInstance.Call("getPrivateKeyFromMnemonic", mnemonic, new ResultCallback<string>(this, result => tcs.SetResult(result), err => tcs.SetException(new Exception(err))));

            return tcs.Task;
        }

        #endregion

    }

}