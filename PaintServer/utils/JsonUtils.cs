using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
namespace PaintClient.utils
{
    static class JsonUtils
    {
        public static string List2Json(List<string> list)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return json;
        }

        public static List<string> Json2List(string json)
        {
            List<string> list = JsonConvert.DeserializeObject<List<string>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            System.Diagnostics.Debug.WriteLine(list[0].ToString());
            return list ?? new List<string>();
            
        }
    }
}
