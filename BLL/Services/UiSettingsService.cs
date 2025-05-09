using BLL.Defaults;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class UiSettingsService(IUiSettingsRepository uiSettingsRepository) : IUiSettingsService
    {
        private readonly IUiSettingsRepository _uiSettingsRepository = uiSettingsRepository;

        public UiSettingsDto GetUiSettings()
        {
            var settings = _uiSettingsRepository.GetSettings();
            if (settings == null)
            {
                // 使用默认设置，并保存到数据库
                settings = DefaultSettings.DefaultUiSettings;
                _uiSettingsRepository.SaveSettings(settings);  // 保存默认设置到数据库
            }

            return new UiSettingsDto
            {
                ShowOnStartup = settings.ShowOnStartup
            };
        }

        public void SaveUiSettings(UiSettingsDto settingsDto)
        {
            UiSettings settings = new()
            {
                ShowOnStartup = settingsDto.ShowOnStartup
            };

            _uiSettingsRepository.SaveSettings(settings);
        }
    }
}
