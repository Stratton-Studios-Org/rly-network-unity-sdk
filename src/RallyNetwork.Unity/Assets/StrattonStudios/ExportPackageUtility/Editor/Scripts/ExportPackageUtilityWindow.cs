using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using UnityEditor;

using UnityEngine;

namespace StrattonStudios.ExportPackageUtility
{

    public class ExportPackageUtilityWindow : EditorWindow
    {

        protected const string CsprojTemplateResourcePath = "StrattonStudios/ExportPackageUtility/template.csproj";
        protected const string LogsDirectory = "ExportPackageUtility/Logs";
        protected const string DllCompilationDirectory = "ExportPackageUtility/DllCompilation";

        protected ExportPackageDefinition finalPackage;
        protected List<string> dllCompilationOptions = new();

        [MenuItem("Stratton Studios/Export Package Utility")]
        public static void ShowWindow()
        {
            ExportPackageUtilityWindow window = GetWindow<ExportPackageUtilityWindow>();
            window.titleContent = new("Export Package Utility");
            window.Show();
        }

        private void OnGUI()
        {
            ExportPackageSettings.instance.MainExportPackage = (ExportPackageDefinition)EditorGUILayout.ObjectField("Final", ExportPackageSettings.instance.MainExportPackage, typeof(ExportPackageDefinition), false);

            EditorGUI.BeginDisabledGroup(this.finalPackage == null);
            if (GUILayout.Button("Export"))
            {
                string outputPath = BuildPackage(this.finalPackage);
                EditorUtility.RevealInFinder(outputPath);
            }

            EditorGUI.EndDisabledGroup();
        }

        public string BuildPackage(ExportPackageDefinition package)
        {
            Debug.Log($"Exporting {package.Identifier}...");

            // Build dependencies
            if (package.Dependencies.Count > 0)
            {
                for (int i = 0; i < package.Dependencies.Count; i++)
                {
                    ExportPackageDefinition dependency = package.Dependencies[i];
                    if (dependency != null)
                    {
                        BuildPackage(dependency);
                    }
                }
            }

            // Compile Assemblies
            if (package.CompileAssemblies)
            {
                for (int i = 0; i < package.Assemblies.Count; i++)
                {
                    ExportPackageAssemblyConfig assembly = package.Assemblies[i];
                    string[] assemblyDeps = assembly.Dependencies.ToArray();
                    for (int j = 0; j < assemblyDeps.Length; j++)
                    {
                        assemblyDeps[j] = FormatPackagePath(assemblyDeps[j], package);
                    }

                    CompileAssembly(
                        package.PackageName,
                        assembly.AssemblyName,
                        FormatPackagePath(assembly.SourcePath, package),
                        assemblyDeps,
                        FormatPackagePath(assembly.OutputFolder, package),
                        ExportPackageSettings.instance.CompileFramework,
                        ExportPackageSettings.instance.DllCompilationOptions);
                }
            }

            // Gather asset GUIDs and paths
            string regex = FormatPackagePath(package.ExcludeRegex, package);
            string[] folders = package.Folders.ToArray();
            for (int i = 0; i < folders.Length; i++)
            {
                folders[i] = FormatPackagePath(folders[i], package);
            }

            string[] assetGuids = AssetDatabase.FindAssets(string.Empty, folders);
            List<string> assetPaths = new();
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetGuid = assetGuids[i];
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);

                // Exclusions
                if (Regex.IsMatch(assetPath, regex))
                {
                    continue;
                }

                assetPaths.Add(assetPath);
            }

            // Export package
            AssetDatabase.ExportPackage(assetPaths.ToArray(), package.OutputPath);
            Debug.Log($"Exported {package.Identifier}");
            return package.OutputPath;
        }

        public string FormatPackagePath(string path, ExportPackageDefinition package)
        {
            return string.Format(path, package.Identifier, package.PackageName);
        }

        public string GenerateCsProject(string dllName, string sourcePath, string[] dependencies)
        {
            TextAsset templateXml = Resources.Load<TextAsset>(CsprojTemplateResourcePath);
            XmlDocument document = new();
            document.LoadXml(templateXml.text);

            // Compile item group
            XmlElement compileItemGroup = document.CreateElement("ItemGroup");
            XmlElement compileItem = document.CreateElement("Compile");
            compileItem.SetAttribute("Include", sourcePath);
            compileItemGroup.AppendChild(compileItem);
            document.FirstChild.AppendChild(compileItemGroup);

            // Reference item group
            XmlElement referenceItemGroup = document.CreateElement("ItemGroup");
            document.FirstChild.AppendChild(referenceItemGroup);

            for (int i = 0; i < dependencies.Length; i++)
            {
                string dependency = dependencies[i];
                XmlElement referenceItem = document.CreateElement("Reference");
                referenceItemGroup.AppendChild(referenceItem);
                referenceItem.SetAttribute("Include", Path.GetFileNameWithoutExtension(dependency));
                XmlElement hintPath = document.CreateElement("HintPath");
                referenceItem.AppendChild(hintPath);
                hintPath.InnerText = dependency;
            }

            Directory.CreateDirectory(DllCompilationDirectory);
            string fileName = Path.Combine(DllCompilationDirectory, dllName + ".csproj");
            using (FileStream file = File.Open(fileName, FileMode.Create))
            {
                document.Save(file);
            }
            return fileName;
        }

        public string[] CompileAssembly(string packageName, string assemblyName, string sourcePath, string[] dependencies, string outputFolder, string compileFramework, List<string> compilerOptions)
        {
            string fileName = GenerateCsProject(assemblyName, sourcePath, dependencies);
            using (System.Diagnostics.Process process = new())
            {
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = $"build {fileName}{string.Join(" ", compilerOptions)} --framework {compileFramework}"
                };
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                if (!string.IsNullOrEmpty(output))
                {
                    File.WriteAllText($"{LogsDirectory}/{packageName}-output.txt", output);
                    Debug.Log(output);
                }

                string error = process.StandardError.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                {
                    File.WriteAllText($"{LogsDirectory}/{packageName}-error.txt", error);
                    Debug.LogError(error);
                }

                process.WaitForExit();

                string[] destinations = new string[] {
                    $"{outputFolder}/{assemblyName}.dll",
                    $"{outputFolder}/{assemblyName}.xml"
                };

                // Copy DLL
                File.Copy($"{DllCompilationDirectory}/bin/Release/{compileFramework}/{assemblyName}.dll", destinations[0], true);

                // Copy documentation
                File.Copy($"{DllCompilationDirectory}/bin/Release/{compileFramework}/{assemblyName}.xml", destinations[1], true);

                // Refresh the asset database to generate meta files and compile DLLs
                AssetDatabase.Refresh();

                return destinations;
            }
        }

    }

}