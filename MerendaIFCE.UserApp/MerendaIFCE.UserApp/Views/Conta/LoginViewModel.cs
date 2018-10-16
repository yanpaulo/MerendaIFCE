using MerendaIFCE.UserApp.Models;
using MerendaIFCE.UserApp.Observable;
using MerendaIFCE.UserApp.Observable.Validatable;
using MerendaIFCE.UserApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Views.Conta
{
    public class LoginViewModel : BusyObject
    {
        public ValidatableProperty<string> Login { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule() });

        public ValidatableProperty<string> Senha { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule() });



        public async Task LoginAsync()
        {
            using (BusyState())
            using (var ws = new WebService())
            {
                var user = await ws.LoginAsync(AsLogin());
                App.Current.Usuario = user;
            }
        }

        public Login AsLogin() => new Login
        {
            UserName = Login.Value,
            Password = Senha.Value
        };
    }
}
