using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yansoft.Rest;

namespace MerendaIFCE.Sync.Services
{
    public class StringOrObjectDeserializer : IDeserializer
    {
        public JsonSerializer JsonSerializer { get; set; }
        public async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)await content.ReadAsStringAsync();
            }

            using (var stream = await content.ReadAsStreamAsync())
            using (var sr = new StreamReader(stream))
            using (var reader = new JsonTextReader(sr))
            {
                return JsonSerializer.Deserialize<T>(reader);
            }
        }
    }
}
