using Sixeyed.MessageQueue.Integration.Data;
using System.Linq;

namespace Sixeyed.MessageQueue.Integration.Queries
{
    public class DoesUserExist
    {
        public static bool Execute(string emailAddress)
        {
            using (var context = new UserModelContainer())
            {
                return context.Users.Count(x=>x.EmailAddress == emailAddress) == 1;
            }
        }
    }
}
