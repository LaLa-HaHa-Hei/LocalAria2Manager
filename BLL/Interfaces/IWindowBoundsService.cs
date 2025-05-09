using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IWindowBoundsService
    {
        WindowBoundsDto GetWindowBounds();
        void SaveWindowBounds(WindowBoundsDto windowBounds);
    }
}
