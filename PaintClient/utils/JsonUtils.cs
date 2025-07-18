﻿using System;
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
            string json = JsonConvert.SerializeObject(list, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return json;
        }

        public static List<T> Json2List<T>(string json)
        {
            List<T> list = JsonConvert.DeserializeObject<List<T>>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            System.Diagnostics.Debug.WriteLine(list[0].ToString());
            return list ?? new List<T>();
            
        }
    }
}
