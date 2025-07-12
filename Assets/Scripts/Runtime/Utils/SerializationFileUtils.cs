using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization.Utils;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace Runtime.Utils
{
    /// <summary>
    /// Handles reading and writing of Unity data to/from disk.
    /// </summary>
    public static class SerializationFileUtils
    {
        
        private static readonly JsonSerializerSettings PresetSerializerSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            Formatting = Formatting.Indented
        };
        public static void SaveScenarioWithMetadata(VariableScenarioConfig scenarioConfig, string title, string description, string filepath, IFileWriter fileWriter)
        {
            scenarioConfig.ScenarioName = title;
            scenarioConfig.DisplayTitle = title;
            scenarioConfig.Description = description;
            scenarioConfig.ModificationDate = DateTime.Now;
            SaveScenarioToJson(scenarioConfig, filepath, fileWriter);
        }
        public static void SaveScenarioToJson(VariableScenarioConfig scenarioConfig, string filePath, IFileWriter fileWriter)
        {
            string json = scenarioConfig.Serialize();
            fileWriter.WriteFile(filePath, Encoding.UTF8.GetBytes(json));
            Debug.Log($"Scenario titled \"{scenarioConfig.ScenarioName}\" has been saved to {filePath}");
        }

        public static VariableScenarioConfig ReadFromJson(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            return VariableScenarioConfig.TryDeserialize(json);
        }

        public static T UnsafeReadPresetFromJson<T>(string jsonPath)
        {
            string json = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<T>(json, PresetSerializerSettings);
        }

        public static void ClearScenarioFolder(string directory)
        {
            Directory.Delete(directory, true);
            Directory.CreateDirectory(directory);
        }

        public static void ExportDirectoryAsZip(string srcDirectory, string targetZip)
        {
            ZipFile.CreateFromDirectory(srcDirectory, targetZip);
        }
        
        public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
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

        public static byte[] ToByteArray(Object obj)
        {
            return obj switch
            {
                byte[] bytes => bytes,
                string str => Encoding.UTF8.GetBytes(str),
                Texture2D texture2D => ImageUtils.SerializeImage(texture2D),
                RenderTexture renderTexture => ImageUtils.SerializeImage(renderTexture),
                _ => throw new NotImplementedException(
                    $"Object of Type {obj.GetType()} could not be converted to byte array (not implemented)")
            };
        }
    }
}