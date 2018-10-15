using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class ValidatableProperty<T> : ObservableObject, IValidatable
    {
        public ValidatableProperty() { }

        public ValidatableProperty(IEnumerable<IValidationRule> validationRules)
        {
            ValidationRules.AddRange(validationRules);
        }

        private string[] validationMessages;

        public T Value { get; set; }
        
        public string[] ValidationMessages
        {
            get { return validationMessages; }
            set
            {
                validationMessages = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(PrimaryValidationMessage));
            }
        }

        public string PrimaryValidationMessage => ValidationMessages?.FirstOrDefault();

        public bool HasError => ValidationMessages?.Any() == true;
        
        public List<IValidationRule> ValidationRules { get; private set; } =
            new List<IValidationRule>();

        public bool Validate()
        {
            ValidationMessages =
                ValidationRules
                    .Where(r => r.HasError(Value))
                    .Select(r => r.ValidationMessage)
                    .ToArray();

            return !ValidationMessages.Any();
        }
    }
}
