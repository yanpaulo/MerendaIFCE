using MerendaIFCE.UserApp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(MerendaIFCE.UserApp.UWP.Services.PathService))]
namespace MerendaIFCE.UserApp.UWP.Services
{
    public class PathService : IPathService
    {
        public string GetLocalFilePath(string filename) =>
            Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
    }
}
