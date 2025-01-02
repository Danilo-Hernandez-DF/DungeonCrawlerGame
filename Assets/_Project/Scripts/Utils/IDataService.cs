using System.Collections.Generic;

namespace Systems.Persistence {
    public interface IDataService {
        void Save(GameData data, bool overwrite = false);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}