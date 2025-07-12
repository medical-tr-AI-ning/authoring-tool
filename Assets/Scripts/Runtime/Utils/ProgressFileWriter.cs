using System.Collections;
using System.Collections.Generic;
using System.IO;
using medicaltraining.assetstore.ScenarioConfiguration.Serialization.Utils;
using UnityEngine;
using Object = System.Object;

namespace Runtime.Utils
{
    public class ProgressFileWriter : IFileWriter
    {
        private static ProgressFileWriter _instance;

        private readonly List<QueuedFile> _queuedFiles = new List<QueuedFile>();

        public delegate void FileWriteProgressChangedEvent();

        public event FileWriteProgressChangedEvent FileWriteProgressChanged;
        
        public delegate void FileWriteCompletedEvent();

        public event FileWriteCompletedEvent FileWriteCompleted;

        private ProgressFileWriter()
        {
        }

        public static ProgressFileWriter GetSingleton()
        {
            if (_instance == null) _instance = new ProgressFileWriter();
            return _instance;
        }

        public void WriteFile(string targetPath, Object data)
        {
            _queuedFiles.Add(new QueuedFile()
            {
                TargetPath = targetPath,
                Data = data,
                Completed = false
            });
        }

        public float GetProgress()
        {
            if (_queuedFiles.Count == 0) return 1f;
            List<QueuedFile> completedFiles = _queuedFiles.FindAll(file => file.Completed == true);
            if (completedFiles.Count == 0) return 0f;
            return (float)completedFiles.Count / _queuedFiles.Count;
        }

        public IEnumerator WriteQueuedFiles()
        {
            foreach (QueuedFile queuedFile in _queuedFiles)
            {
                Debug.Log($"Writing to {queuedFile.TargetPath}");
                var directory = Path.GetDirectoryName(queuedFile.TargetPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                byte[] bytes = SerializationFileUtils.ToByteArray(queuedFile.Data);
                File.WriteAllBytes(queuedFile.TargetPath, bytes);
                queuedFile.Completed = true;
                OnWriteFileCompleted();
                yield return null;
            }
            _queuedFiles.Clear();
            FileWriteCompleted?.Invoke();
        }

        private void OnWriteFileCompleted()
        {
            FileWriteProgressChanged?.Invoke();
        }

        private class QueuedFile
        {
            public string TargetPath;
            public Object Data;
            public bool Completed;
        }
    }
}