using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Yansoft.Rest;

namespace MerendaIFCE.Sync.Services
{
    public class StringOrObjectDeserializer : IDeserializer
    {
        public T Deserialize<T>(string content)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }

            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
