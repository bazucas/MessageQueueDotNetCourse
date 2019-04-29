using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Messages.Event
{
    public class UserUnsubscribed
    {
        public string EmailAddress { get; set; }
    }
}
