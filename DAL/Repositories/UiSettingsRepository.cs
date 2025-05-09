using DAL.DbContexts;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class UiSettingsRepository(AppDbContext context) : IUiSettingsRepository
    {
        private readonly AppDbContext _context = context;

        public UiSettings? GetSettings()
        {
            try
            {
                return _context.GetUiSettings();
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to get UI settings.", ex);
            }
        }

        public void SaveSettings(UiSettings settings)
        {
            try
            {
                _context.SaveUiSettings(settings);
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to save UI settings.", ex);
            }
        }
    }
}
