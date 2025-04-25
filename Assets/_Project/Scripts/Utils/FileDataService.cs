using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Systems.Persistence {
    public class FileDataService : IDataService {
        readonly ISerializer serializer;
        readonly string dataPath;
        readonly string fileExtension;

        public FileDataService(ISerializer serializer) { 
            this.dataPath = Path.Combine(Application.persistentDataPath, "Saves");
            this.fileExtension = "json";
            this.serializer = serializer;
        }

        public string GetPathToFile(string fileName) {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void DeleteAll() {
            foreach(string filePath in Directory.GetFiles(dataPath)) {
                File.Delete(filePath);
            }
        }

        public IEnumerable<string> ListSaves() {
            return Directory.GetFiles(dataPath, $"*.{fileExtension}").Select(Path.GetFileNameWithoutExtension);
        }

        public GameData Load(string name) {
            string fileLocation = GetPathToFile(name);

            if(!File.Exists(fileLocation)) {
                return new GameData();
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Save(GameData data, bool overwrite = false) {
            string fileLocation = GetPathToFile(data.name);

            if(!overwrite && File.Exists(fileLocation)) {
                throw new IOException($"The file '{data.name}.{fileExtension}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public void Delete(string name) {
            string fileLocation = GetPathToFile(name);

            if(File.Exists(fileLocation)) {
                File.Delete(fileLocation);
            }
        }
    }
}