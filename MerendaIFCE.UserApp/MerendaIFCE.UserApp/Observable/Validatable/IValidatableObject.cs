using System.Collections.Generic;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public interface IValidatable
    {
        bool HasError { get; }
        string[] ValidationMessages { get; set; }
        List<IValidationRule> ValidationRules { get; }

        bool Validate();
    }
}