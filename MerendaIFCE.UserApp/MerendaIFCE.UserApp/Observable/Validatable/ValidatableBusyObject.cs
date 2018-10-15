using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace MerendaIFCE.UserApp.Observable.Validatable
{
    public class ValidatableBusyObject : BusyObject
    {

        public bool Validate()
        {
            var hasErrors =
                GetType().GetTypeInfo().DeclaredProperties
                    .Where(dp => dp.GetValue(this) is IValidatable v && !v.Validate())
                    .ToList()
                    .Any();

            return !hasErrors;
        }
    }
}
