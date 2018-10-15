using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class RequiredValidationRule : IValidationRule
    {
        public string ValidationMessage => "Campo obrigatório";

        public bool HasError(object value) => value == null;
    }
}
