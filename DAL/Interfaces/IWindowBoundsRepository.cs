using DAL.Entities;

namespace DAL.Interfaces
{
    public interface IWindowBoundsRepository
    {
        WindowBounds? GetWindowBounds();
        void SaveWindowBounds(WindowBounds windowBounds);
    }
}
