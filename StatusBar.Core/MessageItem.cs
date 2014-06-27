using System;
using Cirrious.MvvmCross.ViewModels;

namespace StatusBar.Core
{
    public class MessageItem : MvxNotifyPropertyChanged, IMessageItem
    {
        private bool _accepted;
        private MessageTypes _msgType;
        private string _message;

        public MessageItem()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; private set;}

        public string Message
        {
            get { return _message; }
            set
            {
                if (_message == value)
                {
                    return;
                }
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public MessageTypes MsgType
        {
            get { return _msgType; }
            set
            {
                if (_msgType == value)
                {
                    return;
                }
                _msgType = value;
                RaisePropertyChanged(() => MsgType);
            }
        }

        public bool Accepted
        {
            get { return _accepted; }
            set
            {
                if (_accepted == value)
                {
                    return;
                }
                _accepted = value;
                RaisePropertyChanged(() => Accepted);
            }
        }
    }
}