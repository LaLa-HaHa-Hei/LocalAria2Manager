using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IAria2SettingsRepository
    {
        Aria2Settings? GetSettings();
        void SaveSettings(Aria2Settings settings);
    }
}
