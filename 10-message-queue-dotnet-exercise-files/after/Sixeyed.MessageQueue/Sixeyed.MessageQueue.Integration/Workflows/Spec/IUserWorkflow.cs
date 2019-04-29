using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Integration.Workflows
{
    public interface IUserWorkflow
    {
        string EmailAddress { get;  set; }

        void Run();
    }
}
