using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Observable
{
    public class BusyState : IDisposable
    {
        private IBusy target;

        public BusyState(IBusy target)
        {
            this.target = target;
            target.IsBusy = true;
        }

        public void Dispose()
        {
            target.IsBusy = false;
        }
    }
}
