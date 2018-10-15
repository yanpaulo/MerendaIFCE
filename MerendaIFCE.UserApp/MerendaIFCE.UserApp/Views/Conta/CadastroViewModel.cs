using MerendaIFCE.UserApp.Models;
using MerendaIFCE.UserApp.Observable.Validatable;
using MerendaIFCE.UserApp.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Views.Conta
{
    public class RegisterPageViewModel : ValidatableBusyObject
    {
        public ValidatableProperty<string> Email { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule(), new EmailValidationRule() });

        public ValidatableProperty<string> Matricula { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule(), new LengthValidationRule("", 14, 14, "Matrícula deve ter 14 dígitos") });

        public ValidatableProperty<string> Senha { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule() });

        public ValidatableProperty<string> ConfirmarSenha { get; set; }
            = new ValidatableProperty<string>(new IValidationRule[] { new RequiredValidationRule() });

        public RegisterPageViewModel()
        {
            ConfirmarSenha.ValidationRules.Add(ValueMatchValidationRuleFactory.Create(Senha, "Confirmação de senha não confere"));
        }

        public async Task CadastraAsync()
        {
            if (Validate())
            {
                using (BusyState())
                {

                    var ws = new WebService();
                    await ws.CadastraAsync(AsCadastro());
                } 
            }
        }

        public Cadastro AsCadastro() =>
            new Cadastro
            {
                Login = Email.Value,
                Matricula = Matricula.Value,
                Senha = Senha.Value
            };


    }
}
