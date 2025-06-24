using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintServer.MVC
{
    interface IServerModel
    {
        event Action OnFileChange;
        string OpenFile(string filename, string clientId);
        void SaveFile(string filename, string jsonData, string clientId);
        void CloseFile(string filename, string clientId);
        List<string> GetAllFiles();
    }
}
