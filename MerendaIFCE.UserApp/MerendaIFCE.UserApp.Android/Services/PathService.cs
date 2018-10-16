using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Xamarin.Forms;
using MerendaIFCE.UserApp.Services;

[assembly: Dependency(typeof(MerendaIFCE.UserApp.Android.Services.PathService))]
namespace MerendaIFCE.UserApp.Android.Services
{
    public class PathService : IPathService
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}