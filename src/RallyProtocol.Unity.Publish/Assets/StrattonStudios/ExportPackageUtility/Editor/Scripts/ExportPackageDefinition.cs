using System.Collections;
using System.Collections.Generic;

using UnityEditorInternal;

using UnityEngine;

namespace StrattonStudios.ExportPackageUtility
{

    [CreateAssetMenu(menuName = "Stratton Studios/Export Package Utility/Export Package Definition")]
    public class ExportPackageDefinition : ScriptableObject
    {

        [SerializeField]
        protected string identifier = "main";
        [SerializeField]
        protected bool exportUnityPackage = true;
        [SerializeField]
        protected string packageName = "MyPackage";
        [SerializeField]
        protected string outputPath = "Assets/{1}/Installer/Packages/main.unitypackage";
        [SerializeField]
        protected List<string> folders = new()
        {
            "Assets/{1}/Documentation",
            "Assets/{1}/Editor",
            "Assets/{1}/Plugins",
            "Assets/{1}/Resources",
            "Assets/{1}/Runtime",
            "Assets/{1}/Scripts",
            "Assets/{1}/Third Party Notices",
        };
        [SerializeField]
        protected List<ExportPackageDefinition> dependencies = new();
        [SerializeField]
        protected string excludeRegex = "Assets[\\/\\/]{1}[\\/\\/]Runtime[\\/\\/].*(.cs|.jslib|.asmdef|.txt|.md)$";

        [Header("Assemblies")]
        [SerializeField]
        protected bool compileAssemblies = false;
        [SerializeField]
        protected List<ExportPackageAssemblyConfig> assemblies = new()
        {
            new()
        };

        [Header("Copy")]
        [SerializeField]
        protected bool copy = false;
        [SerializeField]
        protected List<ExportPackageCopyItemConfig> copyItems = new();

        public string Identifier => this.identifier;
        public bool ExportUnityPackage => this.exportUnityPackage;
        public string PackageName => this.packageName;
        public string OutputPath => this.outputPath;
        public IReadOnlyList<string> Folders => this.folders;
        public IReadOnlyList<ExportPackageDefinition> Dependencies => this.dependencies;
        public string ExcludeRegex => this.excludeRegex;

        public bool CompileAssemblies => this.compileAssemblies;
        public IReadOnlyList<ExportPackageAssemblyConfig> Assemblies => this.assemblies;

        public bool Copy => this.copy;
        public IReadOnlyList<ExportPackageCopyItemConfig> CopyItems => this.copyItems;

    }

    [System.Serializable]
    public class ExportPackageCopyItemConfig
    {

        [SerializeField]
        protected string identifier;
        [SerializeField]
        protected string sourceFolder;
        [SerializeField]
        protected string destinationFolder;

        public string Identifier => this.identifier;
        public string SourceFolder => this.sourceFolder;
        public string DestinationFolder => this.destinationFolder;

    }

    [System.Serializable]
    public class ExportPackageAssemblyConfig
    {

        [SerializeField]
        protected string identifier = "runtime";
        [SerializeField]
        protected string assemblyName = "{1}.Runtime";
        [SerializeField]
        protected string outputFolder = "Assets/{1}/Runtime";
        [SerializeField]
        protected string baseSourcePath = "";
        [SerializeField]
        protected string baseDependenciesPath = "../Dependencies";
        [SerializeField]
        protected List<string> sourcePaths = new() { "Assets/{1}/Runtime" };
        [SerializeField]
        protected List<string> dependencies = new();

        public string Identifier => this.identifier;
        public string AssemblyName => this.assemblyName;
        public string OutputFolder => this.outputFolder;
        public string BaseSourcePath => this.baseSourcePath;
        public string BaseDependenciesPath => this.baseDependenciesPath;
        public IReadOnlyList<string> SourcePaths => this.sourcePaths;
        public IReadOnlyList<string> Dependencies => this.dependencies;

    }

}