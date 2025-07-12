using System.IO;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization.Utils;
using UnityEngine;

namespace Runtime.Utils
{
    public class SimpleFileWriter : IFileWriter
    {
        public void WriteFile(string targetPath, object obj)
        {
            Debug.Log($"Writing to {targetPath}");
            var directory = Path.GetDirectoryName(targetPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var data = SerializationFileUtils.ToByteArray(obj);
            File.WriteAllBytes(targetPath, data);
        }
    }
}