﻿using MerendaIFCE.UserApp.Views.Conta;
using MerendaIFCE.UserApp.Views.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MerendaIFCE.UserApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPageMaster : ContentPage
    {
        public ListView ListView;

        public RootPageMaster()
        {
            InitializeComponent();

            BindingContext = new RootPageMasterViewModel();
            ListView = MenuItemsListView;
        }

        class RootPageMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<RootPageMenuItem> MenuItems { get; set; }
            
            public RootPageMasterViewModel()
            {
                MenuItems = new ObservableCollection<RootPageMenuItem>(new[]
                {
                    new RootPageMenuItem { Id = 0, Title = "Confirmações", TargetType = typeof(ConfirmacoesView) },
                    new RootPageMenuItem { Id = 1, Title = "Dias", TargetType = typeof(SelecaoDiasPage) },
                    new RootPageMenuItem { Id = 2, Title = "Sair", TargetType = typeof(LogoutPage) },
                });
            }
            
            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}