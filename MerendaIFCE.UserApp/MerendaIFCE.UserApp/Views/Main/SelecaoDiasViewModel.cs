using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MerendaIFCE.UserApp.Models;
using MerendaIFCE.UserApp.Observable;
using MerendaIFCE.UserApp.Services;
using MerendaIFCE.UserApp.Exceptions;

namespace MerendaIFCE.UserApp.Views.Main
{
    public class SelecaoDiasViewModel
    {
        private readonly AppDbContext db = AppDbContext.Instance;
        private App app = App.Current;
        private Usuario usuario;

        public IList<SelecaoDiasViewModelItem> Dias { get; private set; }

        public SelecaoDiasViewModel()
        {
            usuario = app.Usuario;
            var dias = usuario.Inscricao.Dias;
            Dias = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>().Select(day =>
            new SelecaoDiasViewModelItem
            {
                Nome = day.ToString(),
                Dia = day,
                InscricaoDia = dias.SingleOrDefault(d => d.Dia == day),
                Habilitado = dias.Any(d => d.Dia == day)
            }).ToList();
        }
    }

    public class SelecaoDiasViewModelItem : ObservableObject
    {
        private bool changing;
        private bool habilitado;
        private InscricaoDia inscricaoDia;

        public string Nome { get; set; }

        public DayOfWeek Dia { get; set; }

        public InscricaoDia InscricaoDia
        {
            get { return inscricaoDia; }
            set { inscricaoDia = value; OnPropertyChanged(); OnPropertyChanged(nameof(Habilitado)); }
        }

        public bool Habilitado
        {
            get { return habilitado; }
            set { habilitado = value; OnPropertyChanged(); }
        }

        public async Task OnChangedAsync()
        {
            if (!changing)
            {
                try
                {
                    changing = true;
                    await ExecutaChangingAsync();
                }
                finally
                {
                    changing = false;
                } 
            }
        }

        private async Task ExecutaChangingAsync()
        {
            using (var ws = new WebService())
            {
                try
                {
                    if (Habilitado)
                    {
                        var dia = new InscricaoDia
                        {
                            Dia = Dia,
                            InscricaoId = App.Current.Usuario.Inscricao.Id
                        };
                        InscricaoDia = await ws.PostDiaAsync(dia);
                        App.Current.AddDia(InscricaoDia);

                    }
                    else
                    {
                        await ws.DeleteDiaAsync(InscricaoDia);
                        App.Current.RemoveDia(InscricaoDia);
                        InscricaoDia = null;
                    }
                }
                catch (ServerException)
                {
                    Habilitado = !Habilitado;
                }
            }
        }
    }
}
