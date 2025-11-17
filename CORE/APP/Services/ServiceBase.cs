using System.Globalization;
using CORE.APP.Models;

namespace CORE.APP.Services;

public abstract class ServiceBase
    {
        private CultureInfo _cultureInfo;
        protected CultureInfo CultureInfo
        {
            get
            {
                // Return the current culture info.
                return _cultureInfo;
            }
            set
            {
                // Update the backing field with the new culture info.
                _cultureInfo = value;

                // Apply the new culture to the current thread for formatting (e.g., numbers, dates).
                Thread.CurrentThread.CurrentCulture = _cultureInfo;

                // Apply the new culture to the current thread for UI localization (e.g., resource lookups).
                Thread.CurrentThread.CurrentUICulture = _cultureInfo;
            }
        }
        
        protected ServiceBase()
        {
            // Set the default culture to English (United States), "tr-TR" parameter can be used for Turkish
            CultureInfo = new CultureInfo("en-US");
        }
        protected CommandResponse Success(string message, int id) => new CommandResponse(true, message, id);
        protected CommandResponse Error(string message) => new CommandResponse(false, message);
    }