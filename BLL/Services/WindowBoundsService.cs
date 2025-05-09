using BLL.Defaults;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class WindowBoundsService(IWindowBoundsRepository windowBoundsRepository) : IWindowBoundsService
    {
        private readonly IWindowBoundsRepository _windowBoundsRepository = windowBoundsRepository;

        public WindowBoundsDto GetWindowBounds()
        {
            var windowBounds = _windowBoundsRepository.GetWindowBounds();
            if (windowBounds == null)
            {
                // 使用默认设置，并保存到数据库
                windowBounds = DefaultSettings.DefaultWindowBounds;
                _windowBoundsRepository.SaveWindowBounds(windowBounds);  // 保存默认设置到数据库
            }

            return new WindowBoundsDto
            {
                Width = windowBounds.Width,
                Height = windowBounds.Height,
                Left = windowBounds.Left,
                Top = windowBounds.Top
            };
        }

        public void SaveWindowBounds(WindowBoundsDto windowBoundsDto)
        {
            WindowBounds windowBounds = new()
            {
                Width = windowBoundsDto.Width,
                Height = windowBoundsDto.Height,
                Left = windowBoundsDto.Left,
                Top = windowBoundsDto.Top
            };

            _windowBoundsRepository.SaveWindowBounds(windowBounds);
        }
    }
}
