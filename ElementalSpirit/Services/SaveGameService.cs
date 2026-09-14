using ElementalSpirit.Domain.SaveData;
using ElementalSpirit.Services.Abstractions;

namespace ElementalSpirit.Services
{
    public class SaveGameService : ISaveGameService
    {
        public bool HasSavedGame => GameSaveFileStore.SaveFileExists;
        public void Save(GameSaveData data) => GameSaveFileStore.Save(data);
        public GameSaveData? Load() => GameSaveFileStore.Load();
        public void DeleteSave() => GameSaveFileStore.Delete();
    }
}