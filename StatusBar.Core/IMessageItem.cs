using System.ComponentModel;

namespace StatusBar.Core
{
    public interface IMessageItem : INotifyPropertyChanged
    {
        string Id { get; }
        string Message { get; }
        MessageTypes MsgType { get; }
        bool Accepted { get; set; }
    }
}
