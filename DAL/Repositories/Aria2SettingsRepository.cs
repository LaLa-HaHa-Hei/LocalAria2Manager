using DAL.DbContexts;
using DAL.Entities;
using DAL.Exceptions;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class Aria2SettingsRepository(AppDbContext context) : IAria2SettingsRepository
    {
        private readonly AppDbContext _context = context;

        public Aria2Settings? GetSettings()
        {
            try
            {
                return _context.GetAria2Settings();
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to get Aria2 settings.", ex);
            }
        }

        public void SaveSettings(Aria2Settings settings)
        {
            try
            {
                _context.SaveAria2Settings(settings);
            }
            catch (Exception ex)
            {
                throw new DbOperationException("Failed to save Aria2 settings.", ex);
            }
        }
    }
}
