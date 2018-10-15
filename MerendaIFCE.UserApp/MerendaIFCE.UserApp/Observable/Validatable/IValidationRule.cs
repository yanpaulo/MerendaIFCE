using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public interface IValidationRule
    {
        string ValidationMessage { get; }

        bool HasError(object value);
    }
}
