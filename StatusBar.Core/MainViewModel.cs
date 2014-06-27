using Cirrious.MvvmCross.ViewModels;

namespace StatusBar.Core
{
    public class MainViewModel : MvxViewModel
    {
        private MessageViewModel _messages = new MessageViewModel();

        public MessageViewModel Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaisePropertyChanged(() => Messages);
            }
        }
    }
}
