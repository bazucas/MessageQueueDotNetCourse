using Sixeyed.MessageQueue.Integration.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Integration.Validators
{
    public class UserValidator
    {
        public string EmailAddress { get; private set; }

        public UserValidator(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public bool Exists()
        {
            using (var context = new UserModelContainer())
            {
                return context.Users.Count(x=>x.EmailAddress == EmailAddress) == 1;
            }
        }
    }
}
