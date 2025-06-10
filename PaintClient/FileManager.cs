using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PaintClient.model;
using Newtonsoft.Json;

namespace PaintClient
{
    class FileManager
    {
        private static FileManager fileManager;
        private FileManager() { }

        public static FileManager GetFileManager()
        {
            if (fileManager == null)
            {
                fileManager = new FileManager();
            }
            return fileManager;
        }

        public string List2Json(List<ShapeData> list)
        {
            string json = JsonConvert.SerializeObject(list, Formatting.Indented);
            return json;
        }

        public List<ShapeData> Json2List(string json)
        {
            List<ShapeData> list = JsonConvert.DeserializeObject<List<ShapeData>>(json);
            return list;
        }

    }
}
