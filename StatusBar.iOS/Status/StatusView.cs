using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Touch.Views;
using MonoTouch.UIKit;
using StatusBar.Core;

namespace StatusBar.iOS.Status
{
    public sealed class StatusView : MvxView
    {
        private IEnumerable _itemsSource;
        private IDisposable _subscription;
        private const float LineHeight = 50;
        private const float TopLineHeight = 4;
        private readonly UIView _top;

        private struct StatusLineElement
        {
            public StatusLine Line;
            public NSLayoutConstraint TopConstraint;
            public NSLayoutConstraint LeftConstraint;
            public EventHandler ConfirmedHandler;
        }

        private readonly List<StatusLineElement> _statusLines = new List<StatusLineElement>();
        private readonly NSLayoutConstraint _totalHeightConstraint;

        public StatusView()
        {
            _top = new UIView
            {
                BackgroundColor = Theme.BackgroundColor
            };
            var topLine = new UIView
            {
                BackgroundColor = Theme.ContrastColor
            };
            _top.AddSubview(topLine);
            AddSubview(_top);

            ConstraintHelpers.AddEqualityConstraint(this, _top, NSLayoutAttribute.Left);
            ConstraintHelpers.AddEqualityConstraint(this, _top, NSLayoutAttribute.Right);
            ConstraintHelpers.AddEqualityConstraint(this, _top, NSLayoutAttribute.Top);
            ConstraintHelpers.AddEqualityConstraint(this, _top, NSLayoutAttribute.Bottom);
            _totalHeightConstraint = ConstraintHelpers.AddConstantConstraint(this, _top, NSLayoutAttribute.Height, 0);
            ConstraintHelpers.AddEqualityConstraint(this, topLine, _top, NSLayoutAttribute.Left);
            ConstraintHelpers.AddEqualityConstraint(this, topLine, _top, NSLayoutAttribute.Right);
            ConstraintHelpers.AddEqualityConstraint(this, topLine, _top, NSLayoutAttribute.Top);
            ConstraintHelpers.AddConstantConstraint(this, topLine, NSLayoutAttribute.Height, TopLineHeight);

            var bindings = this.CreateBindingSet<StatusView, MessageViewModel>();
            bindings.Bind(this).For(view => view.ItemsSource).To(vm => vm.Messages).OneWay();
            bindings.Apply();
        }

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set { SetItemsSource(value); }
        }

        private void SetItemsSource(IEnumerable value)
        {
            if (_itemsSource == value)
                return;

            if (_subscription != null)
            {
                _subscription.Dispose();
                _subscription = null;
            }
            _itemsSource = value;

            ReloadAllItems();

            var newObservable = _itemsSource as INotifyCollectionChanged;
            if (newObservable != null)
            {
                _subscription = newObservable.WeakSubscribe(OnItemsSourceCollectionChanged);
            }
        }

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            InvokeOnMainThread(() =>
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            AddItems(e.NewItems.Cast<IMessageItem>(), e.NewStartingIndex, true);
                            break;
                        case NotifyCollectionChangedAction.Remove:
                            RemoveItems(e.OldItems.Cast<IMessageItem>(), e.OldStartingIndex, true);
                            break;
                        case NotifyCollectionChangedAction.Replace:
                            RemoveItems(e.OldItems.Cast<IMessageItem>(), e.OldStartingIndex, true);
                            AddItems(e.NewItems.Cast<IMessageItem>(), e.NewStartingIndex, true);
                            break;
                        case NotifyCollectionChangedAction.Move:
                            RemoveItems(e.OldItems.Cast<IMessageItem>(), e.OldStartingIndex, true);
                            AddItems(e.NewItems.Cast<IMessageItem>(), e.NewStartingIndex, true);
                            break;
                        case NotifyCollectionChangedAction.Reset:
                            ReloadAllItems();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
        }

        private void ReloadAllItems()
        {
            for (int position = _statusLines.Count - 1; position >= 0; --position)
            {
                RemoveItem(position, false);
            }
            AddItems(_itemsSource.Cast<IMessageItem>(), 0, false);
        }

        private void AddItems(IEnumerable<IMessageItem> newItems, int newStartingIndex, bool animate)
        {
            foreach (var item in newItems)
            {
                AddItem(item, newStartingIndex, animate);
                ++newStartingIndex;
            }
        }

        private void RemoveItems(IEnumerable<IMessageItem> oldItems, int oldStartingIndex, bool animate)
        {
            for (int position = oldStartingIndex + oldItems.Count() - 1; position >= oldStartingIndex; --position)
            {
                RemoveItem(position, animate);
            }
        }

        private async void AddItem(IMessageItem newItem, int position, bool animate)
        {
            var linesToMove = new List<StatusLineElement>();
            if (position <= _statusLines.Count)
            {
                linesToMove = _statusLines.Skip(position).ToList();
            }
            var newLine = new StatusLine
            {
                Severity = newItem.MsgType.ToSeverity(),
                Message = newItem.Message
            };
            EventHandler lineConfirmed = (s, e) => OnLineConfirmed(newItem);
            newLine.LineConfirmed += lineConfirmed;
            AddSubview(newLine);

            NSLayoutConstraint leftConstraint;
            if (animate)
            {
                leftConstraint = ConstraintHelpers.AddRelativeConstraint(this, newLine, _top, NSLayoutAttribute.Right,
                    NSLayoutAttribute.Left);
            }
            else
            {
                leftConstraint = ConstraintHelpers.AddEqualityConstraint(this, newLine, _top, NSLayoutAttribute.Left);
            }
            ConstraintHelpers.AddEqualityConstraint(this, newLine, _top, NSLayoutAttribute.Width);
            var topConstraint = ConstraintHelpers.AddRelativeConstraint(this, newLine, _top, NSLayoutAttribute.Top,
                NSLayoutAttribute.Top, position * LineHeight + TopLineHeight);
            ConstraintHelpers.AddEqualityConstraint(this, newLine, _top, NSLayoutAttribute.Width);
            ConstraintHelpers.AddConstantConstraint(this, newLine, NSLayoutAttribute.Height, LineHeight);

            var statusLineElement = new StatusLineElement
            {
                Line = newLine,
                ConfirmedHandler = lineConfirmed,
                TopConstraint = topConstraint,
                LeftConstraint = leftConstraint
            };

            var topLinePosition = (_statusLines.Count + 1) * LineHeight + TopLineHeight;
            if (animate)
            {
                await AnimateAsync(0.2, () =>
                {
                    _totalHeightConstraint.Constant = topLinePosition;
                    linesToMove.ForEach(line => line.TopConstraint.Constant += LineHeight);
                    LayoutIfNeeded();
                });
                await AnimateAsync(0.45, () =>
                {
                    RemoveConstraint(leftConstraint);
                    leftConstraint.Dispose();
                    leftConstraint = ConstraintHelpers.AddEqualityConstraint(this, newLine, _top, NSLayoutAttribute.Left);
                    LayoutIfNeeded();
                });
                statusLineElement.LeftConstraint = leftConstraint;
            }
            else
            {
                _totalHeightConstraint.Constant = topLinePosition;
                linesToMove.ForEach(line => line.TopConstraint.Constant += LineHeight);
                LayoutIfNeeded();
            }
            _statusLines.Insert(position, statusLineElement);
        }

        private async void RemoveItem(int position, bool animate)
        {
            var linesToMove = new List<StatusLineElement>();
            if (position < _statusLines.Count)
            {
                linesToMove = _statusLines.Skip(position + 1).ToList();
            }
            var lineToRemove = _statusLines[position];
            lineToRemove.Line.LineConfirmed -= lineToRemove.ConfirmedHandler;
            _statusLines.RemoveAt(position);
            var topLinePosition = _statusLines.Count * LineHeight + (_statusLines.Count == 0 ? 0f : TopLineHeight);
            if (animate)
            {
                await AnimateAsync(0.45, () =>
                {
                    RemoveConstraint(lineToRemove.LeftConstraint);
                    lineToRemove.LeftConstraint.Dispose();
                    lineToRemove.LeftConstraint = ConstraintHelpers.AddRelativeConstraint(this, lineToRemove.Line, _top,
                        NSLayoutAttribute.Left, NSLayoutAttribute.Right);
                    LayoutIfNeeded();
                });
                await AnimateAsync(0.2, () =>
                {
                    _totalHeightConstraint.Constant = topLinePosition;
                    linesToMove.ForEach(line => line.TopConstraint.Constant -= LineHeight);
                    LayoutIfNeeded();
                });
            }
            else
            {
                _totalHeightConstraint.Constant = topLinePosition;
                linesToMove.ForEach(line => line.TopConstraint.Constant -= LineHeight);
                LayoutIfNeeded();
            }
            lineToRemove.Line.RemoveFromSuperview();
        }

        private static void OnLineConfirmed(IMessageItem item)
        {
            item.Accepted = true;
        }
    }
}