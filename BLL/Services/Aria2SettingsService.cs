using BLL.Defaults;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class Aria2SettingsService(IAria2SettingsRepository aria2SettingsRepository) : IAria2SettingsService
    {
        private readonly IAria2SettingsRepository _aria2SettingsRepository = aria2SettingsRepository;

        public Aria2SettingsDto GetAria2Settings()
        {
            var settings = _aria2SettingsRepository.GetSettings();
            if (settings == null)
            {
                settings = DefaultSettings.DefaultAria2Settings;
                _aria2SettingsRepository.SaveSettings(settings);
            }

            return new Aria2SettingsDto
            {
                DirectoryPath = settings.DirectoryPath,
                RpcListenPort = settings.RpcListenPort
            };
        }

        public void SaveAria2Settings(Aria2SettingsDto settingsDto)
        {
            Aria2Settings settings = new()
            {
                DirectoryPath = settingsDto.DirectoryPath,
                RpcListenPort = settingsDto.RpcListenPort
            };

            _aria2SettingsRepository.SaveSettings(settings);
        }
    }
}
