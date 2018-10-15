using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class ValueMatchValidationRuleFactory
    {
        public static ValueMatchValidationRule<T> Create<T>(ValidatableProperty<T> target, string message = "Erro")
        {
            return new ValueMatchValidationRule<T>(target, message);
        }
    }

    public class ValueMatchValidationRule<T> : IValidationRule
    {
        public string ValidationMessage { get; set; }

        public ValidatableProperty<T> Target { get; set; }

        public ValueMatchValidationRule(ValidatableProperty<T> validatable, string validationMessage = "Erro")
        {
            Target = validatable;
            ValidationMessage = validationMessage;
        }

        public bool HasError(object value) =>
            value?.Equals(Target.Value) != true;
    }
}
