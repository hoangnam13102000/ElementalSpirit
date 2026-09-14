using ElementalSpirit.Domain.SaveData;

namespace ElementalSpirit.Services.Abstractions
{
    public interface ISaveGameService
    {
        bool HasSavedGame { get; }
        void Save(GameSaveData data);
        GameSaveData? Load();
        void DeleteSave();
    }
}