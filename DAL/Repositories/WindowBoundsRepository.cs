using DAL.DbContexts;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class WindowBoundsRepository(AppDbContext context) : IWindowBoundsRepository
    {
        private readonly AppDbContext _context = context;

        public WindowBounds? GetWindowBounds()
        {
            try
            {
                return _context.GetWindowBounds();
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to get Window Bounds.", ex);
            }
        }

        public void SaveWindowBounds(WindowBounds windowBounds)
        {
            try
            {
                _context.SaveWindowBounds(windowBounds);
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to save Window Bounds.", ex);
            }
        }
    }
}
