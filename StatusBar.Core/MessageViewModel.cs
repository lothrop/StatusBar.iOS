using System.Collections.ObjectModel;
using Cirrious.MvvmCross.ViewModels;

namespace StatusBar.Core
{
    public class MessageViewModel : MvxViewModel
    {
        private readonly ObservableCollection<IMessageItem> _messages = new ObservableCollection<IMessageItem>();

        public ObservableCollection<IMessageItem> Messages
        {
            get { return _messages; }
        }
    }
}
