using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace StrattonStudios.ExportPackageUtility
{

    [FilePath("StrattonStudios/ExportPackageUtility.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ExportPackageSettings : ScriptableSingleton<ExportPackageSettings>
    {

        [SerializeField]
        public List<ExportPackageDefinition> ExportPackages = new();
        [SerializeField]
        public ExportPackageDefinition MainExportPackage;
        [SerializeField]
        public string CompileFramework = "netstandard2.1";
        [SerializeField]
        public List<string> DllCompilationOptions = new()
        {
            "--configuration Release",
            "--framework netstandard2.1",
            "--no-incremental"
        };

    }

}