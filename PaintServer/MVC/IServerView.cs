using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaintServer.MVC
{
    interface IServerView
    {
        void UpdateFileNames(List<string> filenames);
        event Action Suspand;
    }
}
