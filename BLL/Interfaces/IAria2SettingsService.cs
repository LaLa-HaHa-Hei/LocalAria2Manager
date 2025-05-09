using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IAria2SettingsService
    {
        Aria2SettingsDto GetAria2Settings();
        void SaveAria2Settings(Aria2SettingsDto settingsDto);
    }
}
