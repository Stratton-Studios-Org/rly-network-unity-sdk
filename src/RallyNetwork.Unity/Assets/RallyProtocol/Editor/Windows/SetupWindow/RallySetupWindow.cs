using System;
using System.IO;

using RallyProtocol.Networks;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace RallyProtocol.Editor
{

    public class RallySetupWindow : EditorWindow
    {

        protected static readonly string[] NetworkTypes = new string[] { "Amoy", "Polygon", "Local", "Custom" };
        protected const string DefaultNetworkType = "Amoy";

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        protected TextField apiKeyField;
        protected EnumField networkTypeField;

        protected Button saveButton;

        protected RallyProtocolSettingsPreset preset;

        [MenuItem("Window/Rally Protocol/Setup")]
        public static void ShowExample()
        {
            RallySetupWindow wnd = GetWindow<RallySetupWindow>();
            wnd.titleContent = new GUIContent("Rally Setup");
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement uxml = m_VisualTreeAsset.Instantiate();
            root.Add(uxml);

            this.apiKeyField = root.Query<TextField>("apiKey");
            this.networkTypeField = root.Query<EnumField>("networkType");
            this.saveButton = root.Query<Button>("saveButton");
            this.saveButton.clicked += SaveMainSettingsPreset;

            this.preset = RallyUnityNetworkFactory.LoadMainSettingsPreset();
            if (this.preset != null)
            {
                this.apiKeyField.value = this.preset.ApiKey;
                this.networkTypeField.value = this.preset.NetworkType;
            }
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