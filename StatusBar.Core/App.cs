using Cirrious.CrossCore;
using Cirrious.MvvmCross.ViewModels;

namespace StatusBar.Core
{
    public class App : MvxApplication
    {
        public App()
        {
            Mvx.RegisterSingleton<IMvxAppStart>(new AppStart<MainViewModel>());
        }

        public override void Initialize()
        {
            Register();
        }

        private static void Register()
        {
        }
    }
}