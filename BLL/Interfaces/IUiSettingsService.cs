using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IUiSettingsService
    {
        UiSettingsDto GetUiSettings();
        void SaveUiSettings(UiSettingsDto settingsDto);
    }
}
