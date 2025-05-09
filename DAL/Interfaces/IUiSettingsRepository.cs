using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IUiSettingsRepository
    {
        UiSettings? GetSettings();
        void SaveSettings(UiSettings settings);
    }
}
