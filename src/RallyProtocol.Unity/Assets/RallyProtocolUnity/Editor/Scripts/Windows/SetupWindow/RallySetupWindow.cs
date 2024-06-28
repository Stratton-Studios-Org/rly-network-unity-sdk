using System;
using System.IO;

using RallyProtocol.Networks;

using RallyProtocolUnity.Networks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace RallyProtocolUnity.Editor
{

    public class RallySetupWindow : EditorWindow
    {

        #region Constants

        public const string AppUrl = "https://app.rallyprotocol.com/";

        protected const string DefaultNetworkType = "Amoy";

        #endregion

        #region Fields

        protected static readonly string[] NetworkTypes = new string[] { "Amoy", "Polygon", "Local", "Custom" };

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        protected TextField apiKeyField;
        protected EnumField networkTypeField;

        protected Button saveButton;
        protected Button getApiKeyButton;

        protected GroupBox customConfig;

        protected TextField customRlyERC20Address;
        protected TextField customTokenFaucetAddress;

        protected TextField customPaymasterAddress;
        protected TextField customForwarderAddress;
        protected TextField customRelayHubAddress;
        protected TextField customRelayWorkerAddress;
        protected TextField customRelayUrl;
        protected TextField customRpcUrl;
        protected TextField customChainId;
        protected TextField customMaxAcceptanceBudget;
        protected TextField customDomainSeparatorName;

        protected IntegerField customGtxDataNonZero;
        protected IntegerField customGtxDataZero;
        protected IntegerField customRequestValidSeconds;
        protected IntegerField customMaxPaymasterDataLength;
        protected IntegerField customMaxApprovalDataLength;
        protected IntegerField customMaxRelayNonceGap;

        protected RallyProtocolSettingsPreset preset;

        #endregion

        [MenuItem("Window/Rally Protocol/Setup")]
        public static void Open()
        {
            RallySetupWindow wnd = GetWindow<RallySetupWindow>();
            wnd.titleContent = new GUIContent("Rally Setup");
            wnd.minSize = wnd.maxSize = new Vector2(380, 280);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement uxml = m_VisualTreeAsset.Instantiate();
            uxml.style.flexGrow = 1f;
            root.Add(uxml);

            this.apiKeyField = root.Query<TextField>("apiKey");
            this.networkTypeField = root.Query<EnumField>("networkType");
            this.networkTypeField.RegisterValueChangedCallback(new EventCallback<ChangeEvent<Enum>>(OnNetworkChanged));

            this.saveButton = root.Query<Button>("saveButton");
            this.saveButton.clicked += SaveMainSettingsPreset;

            this.getApiKeyButton = root.Query<Button>("getApiKey");
            this.getApiKeyButton.clicked += GetApiKey;

            this.preset = RallyUnityNetworkFactory.LoadMainSettingsPreset();
            if (this.preset != null)
            {
                this.apiKeyField.value = this.preset.ApiKey;
                this.networkTypeField.value = this.preset.NetworkType;
            }

            this.customConfig = root.Query<GroupBox>("customConfig");
            OnNetworkChanged(null);

            this.customRlyERC20Address = root.Query<TextField>("customRlyERC20Address");
            this.customTokenFaucetAddress = root.Query<TextField>("customTokenFaucetAddress");

            this.customPaymasterAddress = root.Query<TextField>("customPaymasterAddress");
            this.customForwarderAddress = root.Query<TextField>("customForwarderAddress");
            this.customRelayHubAddress = root.Query<TextField>("customRelayHubAddress");
            this.customRelayWorkerAddress = root.Query<TextField>("customRelayWorkerAddress");
            this.customRelayUrl = root.Query<TextField>("customRelayUrl");
            this.customRpcUrl = root.Query<TextField>("customRpcUrl");
            this.customChainId = root.Query<TextField>("customChainId");
            this.customMaxAcceptanceBudget = root.Query<TextField>("customMaxAcceptanceBudget");
            this.customDomainSeparatorName = root.Query<TextField>("customDomainSeparatorName");

            this.customGtxDataNonZero = root.Query<IntegerField>("customGtxDataNonZero");
            this.customGtxDataZero = root.Query<IntegerField>("customGtxDataZero");
            this.customRequestValidSeconds = root.Query<IntegerField>("customRequestValidSeconds");
            this.customMaxPaymasterDataLength = root.Query<IntegerField>("customMaxPaymasterDataLength");
            this.customMaxApprovalDataLength = root.Query<IntegerField>("customMaxApprovalDataLength");
            this.customMaxRelayNonceGap = root.Query<IntegerField>("customMaxRelayNonceGap");
        }

        protected void OnNetworkChanged(ChangeEvent<Enum> value)
        {
            RallyNetworkType selectedNetworkType = (RallyNetworkType)this.networkTypeField.value;
            customConfig.style.display = selectedNetworkType == RallyNetworkType.Custom ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void GetApiKey()
        {
            Application.OpenURL(AppUrl);
        }

        public void SaveMainSettingsPreset()
        {
            if (preset == null)
            {
                preset = CreateInstance<RallyProtocolSettingsPreset>();
                Directory.CreateDirectory(Path.Combine("Assets", "Resources", Path.GetDirectoryName(RallyUnityNetworkFactory.MainSettingsResourcesPath)));
                AssetDatabase.CreateAsset(preset, $"Assets/Resources/{RallyUnityNetworkFactory.MainSettingsResourcesPath}.asset");
                AssetDatabase.SaveAssets();
            }

            using (SerializedObject so = new(preset))
            {
                so.FindProperty("apiKey").stringValue = this.apiKeyField.value;
                so.FindProperty("networkType").enumValueIndex = Convert.ToInt32(this.networkTypeField.value);
                so.ApplyModifiedProperties();
            }
        }

    }

}