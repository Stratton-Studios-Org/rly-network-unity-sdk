using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

using UnityEditor;
using UnityEditor.Compilation;

using UnityEngine;

namespace StrattonStudios.ExportPackageUtility
{

    public class ExportPackageUtilityWindow : EditorWindow
    {

        protected const string WorkingDirectory = "Library/StrattonStudios/ExportPackageUtility";
        protected const string CsprojTemplateResourcePath = "StrattonStudios/ExportPackageUtility/template.csproj";
        protected const string LogsDirectory = WorkingDirectory + "/Logs";
        protected const string AssemblyCompilationDirectory = WorkingDirectory + "/AssemblyCompilation";

        protected Dictionary<ExportPackageDefinition, bool> packagesBuilt = new();
        protected ExportPackageDefinition finalPackage;

        [MenuItem("Stratton Studios/Export Package Utility")]
        public static ExportPackageUtilityWindow ShowWindow()
        {
            ExportPackageUtilityWindow window = GetWindow<ExportPackageUtilityWindow>();
            window.titleContent = new("Export Package Utility");
            window.Show();
            return window;
        }

        private void OnGUI()
        {

            // Begin assembly compilation
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Assembly Compilation", EditorStyles.boldLabel);

            // Compilation options
            string compilationOptions = string.Join("\n", ExportPackageSettings.instance.AssemblyCompilationOptions);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PrefixLabel("Compilation Options");
            compilationOptions = EditorGUILayout.TextArea(compilationOptions);
            if (compilationOptions.Contains("--framework"))
            {
                EditorGUILayout.HelpBox("The '--framework' option is ignored, modify the Compiler Framework instead.", MessageType.Warning);
            }

            if (EditorGUI.EndChangeCheck())
            {
                ExportPackageSettings.instance.AssemblyCompilationOptions.Clear();
                ExportPackageSettings.instance.AssemblyCompilationOptions.AddRange(compilationOptions.Split("\n"));
            }

            // Compiler framework
            string compilerFramework = ExportPackageSettings.instance.CompileFramework;
            compilerFramework = EditorGUILayout.TextField("Compiler Framework", compilerFramework);
            ExportPackageSettings.instance.CompileFramework = compilerFramework;

            // End assembly compilation
            GUILayout.EndVertical();

            // Final package
            this.finalPackage = ExportPackageSettings.instance.MainExportPackage;
            this.finalPackage = (ExportPackageDefinition)EditorGUILayout.ObjectField("Final", this.finalPackage, typeof(ExportPackageDefinition), false);
            ExportPackageSettings.instance.MainExportPackage = this.finalPackage;

            // Export
            EditorGUI.BeginDisabledGroup(this.finalPackage == null);
            if (GUILayout.Button("Export"))
            {
                ExportPackage(this.finalPackage);
            }

            EditorGUI.EndDisabledGroup();
        }

        public string ExportPackage(ExportPackageDefinition package)
        {
            this.packagesBuilt.Clear();
            string outputPath = BuildPackage(package);
            EditorUtility.RevealInFinder(outputPath);
            return outputPath;
        }

        public string BuildPackage(ExportPackageDefinition package)
        {
            if (this.packagesBuilt.ContainsKey(package))
            {
                return string.Empty;
            }

            this.packagesBuilt[package] = true;

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

            // Copy
            if (package.Copy)
            {
                CopyPackage(package);
            }

            // Compile Assemblies
            if (package.CompileAssemblies)
            {
                CompilePackageAssemblies(package);
            }

            // Export package, Gather asset GUIDs and paths
            if (package.ExportUnityPackage)
            {
                Debug.Log($"Exporting {package.Identifier}...");
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
                    if (!string.IsNullOrEmpty(regex) && Regex.IsMatch(assetPath, regex))
                    {
                        continue;
                    }

                    assetPaths.Add(assetPath);
                }

                // Export package
                Directory.CreateDirectory(Path.GetDirectoryName(package.OutputPath));
                AssetDatabase.ExportPackage(assetPaths.ToArray(), package.OutputPath);
                Debug.Log($"Exported {package.Identifier}");
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            return package.OutputPath;
        }

        public void CopyPackage(ExportPackageDefinition package)
        {
            for (int i = 0; i < package.CopyItems.Count; i++)
            {
                ExportPackageCopyItemConfig copyItem = package.CopyItems[i];
                CopyDirectory(copyItem.SourceFolder, copyItem.DestinationFolder, true);
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            DirectoryInfo dir = new(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");
            }

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath, true);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        public void CompilePackageAssemblies(ExportPackageDefinition package)
        {
            Debug.Log($"Compiling package {package.Identifier} assemblies...");
            for (int i = 0; i < package.Assemblies.Count; i++)
            {
                ExportPackageAssemblyConfig assembly = package.Assemblies[i];
                Debug.Log($"Compiling package {package.Identifier}:{assembly.Identifier} assembly...");
                string baseDepsPath = FormatPackagePath(assembly.BaseDependenciesPath, package);
                string[] assemblyDeps = assembly.Dependencies.ToArray();
                for (int j = 0; j < assemblyDeps.Length; j++)
                {
                    //if (assemblyDeps[j].StartsWith("./"))
                    //{
                    //    assemblyDeps[j] = FormatPackagePath(assemblyDeps[j], package);
                    //    continue;
                    //}

                    assemblyDeps[j] = Path.GetFullPath(Path.Combine(baseDepsPath, FormatPackagePath(assemblyDeps[j], package)));
                }

                string baseSourcePath = FormatPackagePath(assembly.BaseSourcePath, package);
                string[] sourcePaths = assembly.SourcePaths.ToArray();
                for (int j = 0; j < sourcePaths.Length; j++)
                {
                    sourcePaths[j] = Path.GetFullPath(Path.Combine(baseSourcePath, FormatPackagePath(sourcePaths[j], package)));
                }

                CompileAssembly(
                    package.PackageName,
                    FormatPackagePath(assembly.AssemblyName, package),
                    sourcePaths,
                    assemblyDeps,
                    FormatPackagePath(assembly.OutputFolder, package),
                    ExportPackageSettings.instance.CompileFramework,
                    ExportPackageSettings.instance.AssemblyCompilationOptions);
                Debug.Log($"Compiled package {package.Identifier}:{assembly.Identifier} assembly");
            }

            Debug.Log($"Compiled package {package.Identifier} assemblies");
        }

        public string FormatPackagePath(string path, ExportPackageDefinition package)
        {
            return string.Format(path, package.Identifier, package.PackageName);
        }

        public string GenerateCsProject(string dllName, string[] sourcePaths, string[] dependencies)
        {
            TextAsset templateXml = Resources.Load<TextAsset>(CsprojTemplateResourcePath);
            XmlDocument document = new();
            document.LoadXml(templateXml.text);

            // Compile item group
            XmlElement compileItemGroup = document.CreateElement("ItemGroup");
            for (int i = 0; i < sourcePaths.Length; i++)
            {
                string sourcePath = sourcePaths[i];
                if (sourcePath.EndsWith(".cs"))
                {
                    XmlElement compileItem = document.CreateElement("Compile");
                    compileItem.SetAttribute("Include", sourcePath);
                    compileItemGroup.AppendChild(compileItem);
                }
            }

            // Embedded resource group
            XmlElement embeddedResourceItemGroup = document.CreateElement("ItemGroup");
            for (int i = 0; i < sourcePaths.Length; i++)
            {
                string sourcePath = sourcePaths[i];
                if (!sourcePath.EndsWith(".cs"))
                {
                    XmlElement embeddedResource = document.CreateElement("EmbeddedResource");
                    embeddedResource.SetAttribute("Include", sourcePath);
                    embeddedResourceItemGroup.AppendChild(embeddedResource);
                }
            }

            document.FirstChild.AppendChild(compileItemGroup);
            document.FirstChild.AppendChild(embeddedResourceItemGroup);

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

            Directory.CreateDirectory(AssemblyCompilationDirectory);
            string fileName = Path.Combine(AssemblyCompilationDirectory, dllName + ".csproj");
            using (FileStream file = File.Open(fileName, FileMode.Create))
            {
                document.Save(file);
            }
            return fileName;
        }

        public string[] CompileAssembly(string packageName, string assemblyName, string[] sourcePaths, string[] dependencies, string outputFolder, string compileFramework, List<string> compilerOptions)
        {
            List<string> compilerArguments = new();
            for (int i = 0; i < compilerOptions.Count; i++)
            {
                string compilerOption = compilerOptions[i];

                // Ignore framework option
                if (compilerOptions.Contains("--framework"))
                {
                    continue;
                }

                compilerArguments.Add(compilerOption);
            }

            string fileName = GenerateCsProject(assemblyName, sourcePaths, dependencies);
            using (System.Diagnostics.Process process = new())
            {
                process.StartInfo = new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{fileName}\" {string.Join(" ", compilerArguments)} --framework {compileFramework}"
                };
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                Directory.CreateDirectory(LogsDirectory);
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

                Directory.CreateDirectory(outputFolder);
                string[] destinations = new string[] {
                    $"{outputFolder}/{assemblyName}.dll",
                    $"{outputFolder}/{assemblyName}.xml"
                };

                // Copy DLL
                File.Copy($"{AssemblyCompilationDirectory}/bin/Release/{compileFramework}/{assemblyName}.dll", destinations[0], true);

                // Copy documentation
                File.Copy($"{AssemblyCompilationDirectory}/bin/Release/{compileFramework}/{assemblyName}.xml", destinations[1], true);

                // Refresh the asset database to generate meta files and compile DLLs
                AssetDatabase.Refresh();

                return destinations;
            }
        }

    }

}