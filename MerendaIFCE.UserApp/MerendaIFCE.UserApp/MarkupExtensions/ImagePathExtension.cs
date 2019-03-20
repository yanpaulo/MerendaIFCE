using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MerendaIFCE.UserApp.MarkupExtensions
{
    [ContentProperty(nameof(Path))]
    public class ImagePathExtension : IMarkupExtension
    {
        public string Path { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    return Path.Replace("-", "_");
                case Device.iOS:
                    return Path;
                case Device.UWP:
                    return $"Assets/{Path}";
                default:
                    return new InvalidOperationException("Trying to get image path on unsupported Device type.");
            }
        }
    }
}
