using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixeyed.MessageQueue.Messages
{
    public static class ObjectExtensions
    {
        public static string GetMessageType(this object obj)
        {
            return obj.GetType().AssemblyQualifiedName;
        }

        public static string ToJsonString(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static Stream ToJsonStream(this object obj)
        {
            var json = obj.ToJsonString();
            return new MemoryStream(Encoding.Default.GetBytes(json));
        }
    }
}
