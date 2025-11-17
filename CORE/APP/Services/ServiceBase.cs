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
                return _cultureInfo;
            }
            set
            {
                _cultureInfo = value;

                Thread.CurrentThread.CurrentCulture = _cultureInfo;

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
