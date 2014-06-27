using System;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.UIKit;
using StatusBar.Core;
using StatusBar.iOS.Status;

namespace StatusBar.iOS
{
    [MvxViewFor(typeof(MainViewModel))]
    public class MainViewController : MvxViewController
    {
        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var statusView = new StatusView();
            Add(statusView);

            ConstraintHelpers.AddEqualityConstraint(View, statusView, NSLayoutAttribute.Left);
            ConstraintHelpers.AddEqualityConstraint(View, statusView, NSLayoutAttribute.Width);
            ConstraintHelpers.AddEqualityConstraint(View, statusView, NSLayoutAttribute.Bottom);

            var bindings = this.CreateBindingSet<MainViewController, MainViewModel>();
            bindings.Bind(statusView).For(view => view.ItemsSource).To(vm => vm.Messages.Messages).OneWay();
            bindings.Apply();

            await ShowTestMessages();
        }

        private async Task ShowTestMessages()
        {
            var messages = ((MainViewModel)ViewModel).Messages.Messages;
            await Task.Delay(TimeSpan.FromSeconds(3));
            messages.Add(new MessageItem { MsgType = MessageTypes.Information, Message = "Testing Information" });

            await Task.Delay(TimeSpan.FromSeconds(1));
            messages.Add(new MessageItem { MsgType = MessageTypes.Error, Message = "Testing Error" });

            await Task.Delay(TimeSpan.FromSeconds(1));
            messages.Insert(1, new MessageItem { MsgType = MessageTypes.Warning, Message = "Testing Warning" });

            await Task.Delay(TimeSpan.FromSeconds(1));
            messages.RemoveAt(1);

            await Task.Delay(TimeSpan.FromSeconds(1));
            messages.RemoveAt(0);

            await Task.Delay(TimeSpan.FromSeconds(1));
            messages.RemoveAt(0);
        }
    }
}