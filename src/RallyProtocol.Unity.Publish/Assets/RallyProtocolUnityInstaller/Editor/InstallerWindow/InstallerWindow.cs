using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Windows.Speech;

namespace RallyProtocolUnityInstaller
{

    public class InstallerWindow : EditorWindow
    {

        public const string GitInstallUrl = "https://git-scm.com/";
        public const string GitCommand = "git";

        public const string BasePath = "Assets/RallyProtocolUnityInstaller";
        public const string PackagesPath = BasePath + "/Packages";
        public const string MainPackagePath = PackagesPath + "/main.unitypackage";
        public const string SamplesPackagePath = PackagesPath + "/samples.unitypackage";

        public const string MainAssemblyDefinition = "RallyProtocolUnity.Runtime";
        public const string SamplesPath = "Assets/RallyProtocolUnity/Samples";
        public const string MainCheckType = "RallyProtocolUnity.Networks.RallyUnityNetworkFactory";

        protected const string ShowOnStratupKey = "RallyProtocolUnityInstaller.InstallerWindow.ShowOnStratup";

        public enum DependencyInstallType
        {
            GitUrl,
            PackageName
        }

        [System.Serializable]
        public class DependencyInfo
        {

            public string PackageName;
            public string Label;
            public DependencyInstallType InstallType;
            public string InstallData;
            public string CheckType;

            public DependencyInfo(string packageName, string label, DependencyInstallType installType, string installData, string checkType)
            {
                this.PackageName = packageName;
                this.Label = label;
                this.InstallType = installType;
                this.InstallData = installData;
                this.CheckType = checkType;
            }

        }

        protected static readonly DependencyInfo[] dependencies = new DependencyInfo[] {
            new("com.unity.nuget.newtonsoft-json", "Newtonsoft.Json", DependencyInstallType.PackageName, "com.unity.nuget.newtonsoft-json", "Newtonsoft.Json.JsonConvert"),
            new("com.nethereum.unity", "Nethereum.Unity", DependencyInstallType.GitUrl, "https://github.com/Nethereum/Nethereum.Unity.git", "Nethereum.Web3.Web3"),
            new("com.cysharp.unitask", "UniTask", DependencyInstallType.GitUrl, "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask", "Cysharp.Threading.Tasks.UniTask"),
        };

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        protected Button installButton;
        protected VisualElement loading;
        protected Label statusLabel;
        protected AddRequest addRequest;
        protected AddAndRemoveRequest bulkAddRequest;
        protected ListRequest listRequest;
        protected bool listCompleted = false;

        protected Dictionary<string, Toggle> packageNameToToggle = new();

        [MenuItem("Window/Rally Protocol/Installer")]
        public static void Open()
        {
            InstallerWindow wnd = GetWindow<InstallerWindow>();
            wnd.titleContent = new GUIContent("Rally Protocol Unity Installer");
            wnd.minSize = wnd.maxSize = new Vector2(380, 280);
        }

        [InitializeOnLoadMethod]
        private static void ShowOnStartup()
        {
            bool show = EditorPrefs.GetBool(ShowOnStratupKey, true);
            if (show)
            {
                Open();
                EditorPrefs.SetBool(ShowOnStratupKey, false);
            }
        }


        private async void OnEnable()
        {
            this.listCompleted = false;
            await FetchPackages();
        }

        private void Update()
        {
            if (this.listRequest != null && this.listRequest.IsCompleted && !this.listCompleted)
            {
                if (this.loading != null)
                {
                    this.listCompleted = true;
                    HideLoading();
                    UpdateGUI();
                }
            }
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement uxml = m_VisualTreeAsset.Instantiate();
            uxml.style.flexGrow = 1f;
            root.Add(uxml);

            this.loading = root.Q<VisualElement>("loading");
            Foldout dependenciesFoldout = root.Q<Foldout>("dependenciesFoldout");
            for (int i = 0; i < dependencies.Length; i++)
            {
                DependencyInfo dependencyInfo = dependencies[i];
                Toggle dependencyToggle = new(dependencyInfo.Label);
                dependencyToggle.SetEnabled(false);
                dependenciesFoldout.Add(dependencyToggle);
                this.packageNameToToggle[dependencyInfo.PackageName] = dependencyToggle;
            }

            this.installButton = root.Q<Button>("installButton");
            this.installButton.clicked += Install;
        }

        protected async Task FetchPackages()
        {
            this.listRequest = Client.List(true);
            this.listCompleted = false;
            while (!this.listRequest.IsCompleted)
            {
                await Task.Yield();
            }
        }

        protected void UpdateGUI()
        {
            if (this.listRequest != null)
            {
                foreach (var package in listRequest.Result)
                {
                    if (packageNameToToggle.TryGetValue(package.name, out Toggle toggle))
                    {
                        toggle.value = true;
                    }
                }
            }

            bool installEnabled = true;
            if (IsMainPackageInstalled())
            {
                if (AreSamplesInstalled())
                {
                    this.installButton.text = "Installed";
                    installEnabled = false;
                }
                else
                {
                    this.installButton.text = "Install Samples";
                }
            }
            else
            {
                this.installButton.text = "Install";
            }

            this.installButton.SetEnabled(installEnabled);
        }

        protected void HideLoading()
        {
            this.loading.style.display = DisplayStyle.None;
        }

        protected void ShowLoading()
        {
            this.loading.style.display = DisplayStyle.Flex;
        }

        public bool IsDependencyInstalled(DependencyInfo dependency)
        {
            if (this.packageNameToToggle.TryGetValue(dependency.PackageName, out Toggle toggle))
            {
                return toggle.value;
            }

            return false;
        }

        public bool AreSamplesInstalled()
        {
            return Directory.Exists(SamplesPath);
        }

        public bool IsMainPackageInstalled()
        {
            string assemblyPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(MainAssemblyDefinition);
            if (string.IsNullOrEmpty(assemblyPath))
            {
                return false;
            }

            return true;
        }

        public async Task InstallDependency(DependencyInfo dependency)
        {
            Debug.Log($"Installing dependency '{dependency.PackageName}'...");
            if (IsDependencyInstalled(dependency))
            {
                return;
            }

            if (this.addRequest != null && !this.addRequest.IsCompleted)
            {
                while (!this.addRequest.IsCompleted)
                {
                    await Task.Yield();
                }
            }

            this.addRequest = Client.Add(dependency.InstallData);
            while (!this.addRequest.IsCompleted)
            {
                await Task.Yield();
            }

            Debug.Log($"Installed dependency '{dependency.PackageName}'");
        }

        public async Task InstallDependencies()
        {
            if (this.listRequest != null && !this.listRequest.IsCompleted)
            {
                while (!this.listRequest.IsCompleted)
                {
                    await Task.Yield();
                }
            }

            List<string> packagesToAdd = new();
            for (int i = 0; i < dependencies.Length; i++)
            {
                DependencyInfo dependency = dependencies[i];

                // Skip installed dependencies
                if (this.packageNameToToggle.TryGetValue(dependency.PackageName, out Toggle toggle))
                {
                    if (toggle.value)
                    {
                        continue;
                    }
                }

                packagesToAdd.Add(dependency.InstallData);
            }

            if (packagesToAdd.Count > 0)
            {
                this.bulkAddRequest = Client.AddAndRemove(packagesToAdd.ToArray());
                while (!this.bulkAddRequest.IsCompleted)
                {
                    await Task.Yield();
                }
            }

            this.listCompleted = false;
            await FetchPackages();
        }

        public void InstallMain()
        {
            AssetDatabase.ImportPackage(MainPackagePath, true);
        }

        public void InstallSamples()
        {
            AssetDatabase.ImportPackage(SamplesPackagePath, true);
        }

        public async void Install()
        {
            if (!IsGitInstalled())
            {
                if (EditorUtility.DisplayDialog("Git is not installed", "You do not have Git installed on your system, but Git is required to be present for the installation of dependencies and packages through Unity package manager, press Install Git to open Git's website.\n\nIf you've just installed git, make sure to restart Unity and Unity hub", "Install Git", "Cancel"))
                {
                    Application.OpenURL(GitInstallUrl);
                }

                return;
            }

            ShowLoading();
            if (IsMainPackageInstalled())
            {
                InstallSamples();
            }
            else
            {
                await InstallDependencies();
                InstallMain();
            }

            HideLoading();
        }

        /// <summary>
        /// Checks whether the git command exsits.
        /// </summary>
        /// <returns></returns>
        public static bool IsGitInstalled()
        {
            try
            {
                using (System.Diagnostics.Process p = new())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.FileName = GitCommand;
                    p.Start();
                    p.WaitForExit();

                    // If the process runs successfully it means the command exists
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}