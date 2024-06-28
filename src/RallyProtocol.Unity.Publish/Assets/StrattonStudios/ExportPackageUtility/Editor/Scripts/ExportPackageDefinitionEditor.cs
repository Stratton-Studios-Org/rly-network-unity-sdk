using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

namespace StrattonStudios.ExportPackageUtility
{

    [CustomEditor(typeof(ExportPackageDefinition))]
    public class ExportPackageDefinitionEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Compile Assemblies"))
            {
                ExportPackageDefinition package = target as ExportPackageDefinition;
                ExportPackageUtilityWindow window = ExportPackageUtilityWindow.ShowWindow();
                window.CompilePackageAssemblies(package);
            }

            if (GUILayout.Button("Copy"))
            {
                ExportPackageDefinition package = target as ExportPackageDefinition;
                ExportPackageUtilityWindow window = ExportPackageUtilityWindow.ShowWindow();
                window.CopyPackage(package);
            }

            if (GUILayout.Button("Export"))
            {
                ExportPackageDefinition package = target as ExportPackageDefinition;
                ExportPackageUtilityWindow window = ExportPackageUtilityWindow.ShowWindow();
                window.ExportPackage(package);
            }
        }

    }

}