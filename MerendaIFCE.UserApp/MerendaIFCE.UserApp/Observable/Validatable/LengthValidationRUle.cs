using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class LengthValidationRule : IValidationRule
    {
        public LengthValidationRule(string name, int minLength, string message = null)
        {
            ValidationMessage = message ?? $"{name} deve ter no mínimo {minLength} caracteres.";
            MinLength = minLength;
        }

        public LengthValidationRule(string name, int minLength, int maxLength, string message = null)
        {
            ValidationMessage = message ??  $"{name} deve ter no mínimo {minLength} e no máximo {maxLength} caracteres.";
            MinLength = minLength;
        }
        

        public string ValidationMessage { get; set; }

        public int MinLength { get; private set; }

        public int? MaxLength { get; private set; }

        public bool HasError(object value)
        {
            var v = value as string;
            return v?.Length < MinLength || v?.Length > MaxLength;
        }
    }
}
