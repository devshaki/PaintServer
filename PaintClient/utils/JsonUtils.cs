using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using PaintClient.model;
namespace PaintClient.utils
{
    static class JsonUtils
    {
        public static string List2Json(List<ShapeData> list)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            return json;
        }

        public static List<ShapeData> Json2List(string json)
        {
            List<ShapeData> list = JsonConvert.DeserializeObject<List<ShapeData>>(json);
            return list;
        }
    }
}
