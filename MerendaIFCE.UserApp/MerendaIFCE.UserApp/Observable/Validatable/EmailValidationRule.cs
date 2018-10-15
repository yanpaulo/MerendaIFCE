using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class EmailValidationRule : IValidationRule
    {
        public string ValidationMessage => "Email inválido";

        public bool HasError(object value) => value != null ?
            !Regex.IsMatch(value as string, @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$") : false;
    }
}
