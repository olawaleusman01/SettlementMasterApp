using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Dapper.Data
{
    public interface ILoginMultiple
    {
        bool IsYourLoginStillTrue(string userId, string sessionId);
        bool IsUserLoggedOnElsewhere(string userId, string sessionId);
        bool LogEveryoneElseOut(string userId, string sessionId);
        int PostLogins(string userId, string sessionId);
    }
}
