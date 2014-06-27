using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;

namespace StatusBar.Core
{
    public class AppStart<TViewModel> : MvxNavigatingObject, IMvxAppStart where TViewModel : IMvxViewModel
    {
        private readonly Dictionary<string, string> _parameters; 
        
        public AppStart()
        {
        }

        public AppStart(Dictionary<string, string> parameters)
        {
            _parameters = parameters;
        }

        public void Start(object hint = null)
        {
            if (_parameters != null)
            {
                ShowViewModel<TViewModel>(_parameters);
            }
            else
            {
                ShowViewModel<TViewModel>();
            }
        }
    }
}
