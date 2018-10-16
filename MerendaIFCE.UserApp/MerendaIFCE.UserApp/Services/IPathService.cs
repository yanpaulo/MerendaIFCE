using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Services
{
    public interface IPathService
    {
        string GetLocalFilePath(string filename);
    }
}
