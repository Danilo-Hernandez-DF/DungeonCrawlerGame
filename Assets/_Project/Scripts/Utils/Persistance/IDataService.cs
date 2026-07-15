using Game;

namespace Utils {
    public interface IDataService {
        void Save(GameData data, bool overwrite = false, bool tempSave = true);
        GameData Load(string name);
        void Delete(string name);
        void DeleteAll();
        IEnumerable<string> ListSaves();
    }
}