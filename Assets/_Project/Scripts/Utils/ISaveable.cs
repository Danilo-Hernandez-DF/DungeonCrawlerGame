using UtilsModule;

namespace Systems.Persistence
{
    public interface ISaveable { 
        SerializableGuid Id { get; set; }
    }
}