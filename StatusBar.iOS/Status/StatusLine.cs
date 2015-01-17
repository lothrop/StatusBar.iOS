using System;
using CoreGraphics;
using UIKit;
using StatusBar.Core;

namespace StatusBar.iOS.Status
{
    public enum Severity
    {
        Information,
        Warning,
        Error
    }

    public enum ImageType
    {
        TypeIcon,
        ConfirmIcon
    }

    public static class SeverityExtensions
    {
        public static string GetFileName(this Severity severity, ImageType type)
        {
            string fileName;
            switch (type)
            {
                case ImageType.TypeIcon:
                    switch (severity)
                    {
                        case Severity.Information:
                            fileName = "Icon_Information.png";
                            break;
                        case Severity.Warning:
                            fileName = "Icon_Warning.png";
                            break;
                        case Severity.Error:
                            fileName = "Icon_Error.png";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                case ImageType.ConfirmIcon:
                    switch (severity)
                    {
                        case Severity.Information:
                            fileName = "Icon_Information_Accept.png";
                            break;
                        case Severity.Warning:
                            fileName = "Icon_Warning_Accept.png";
                            break;
                        case Severity.Error:
                            fileName = "Icon_Error_Accept.png";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return string.Format("Status/{0}", fileName);
        }

        public static Severity ToSeverity(this MessageTypes messageType)
        {
            switch (messageType)
            {
                case MessageTypes.Information:
                    return Severity.Information;
                case MessageTypes.Warning:
                    return Severity.Warning;
                case MessageTypes.Error:
                    return Severity.Error;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    public sealed class StatusLine : UIView
    {
        public event EventHandler LineConfirmed;

        private Severity _severity;
        private readonly UIImageView _typeIcon;
        private readonly UILabel _message;
        private readonly UIButton _confirmButton;

        public StatusLine()
        {
            BackgroundColor = Theme.BackgroundColor;
            _typeIcon = new UIImageView
                {
                    ContentMode = UIViewContentMode.ScaleAspectFit,
                    Image = new UIImage()
                };
            _message = new UILabel
                {
                    Font = Theme.RegularFont,
                    TextColor = UIColor.White
                };
            _confirmButton = new UIButton();
            _confirmButton.TouchUpInside += (sender, args) =>
                {
                    var confirmed = LineConfirmed;
                    if (confirmed != null)
                    {
                        confirmed(this, new EventArgs());
                    }
                };
            var line = new UIView { BackgroundColor = Theme.BorderColor };

            AddSubviews(_typeIcon, _message, _confirmButton, line);

            const float padding = 24;
            const float iconHeightAndWidth = 22;
            const float spacing = 15;
            const float lineHeight = 1;

            ConstraintHelpers.AddConstantConstraint(this, _typeIcon, NSLayoutAttribute.Height, iconHeightAndWidth);
            ConstraintHelpers.AddConstantConstraint(this, _typeIcon, NSLayoutAttribute.Width, iconHeightAndWidth);
            ConstraintHelpers.AddConstantConstraint(this, _confirmButton, NSLayoutAttribute.Height, iconHeightAndWidth);
            ConstraintHelpers.AddConstantConstraint(this, _confirmButton, NSLayoutAttribute.Width, iconHeightAndWidth);
            ConstraintHelpers.AddConstantConstraint(this, line, NSLayoutAttribute.Height, lineHeight);
            ConstraintHelpers.AddEqualityConstraint(this, _typeIcon, NSLayoutAttribute.CenterY);
            ConstraintHelpers.AddEqualityConstraint(this, _message, NSLayoutAttribute.CenterY);
            ConstraintHelpers.AddEqualityConstraint(this, _confirmButton, NSLayoutAttribute.CenterY);
            ConstraintHelpers.AddRelativeConstraint(this, _typeIcon, this, NSLayoutAttribute.Left,
                NSLayoutAttribute.Left, padding);
            ConstraintHelpers.AddRelativeConstraint(this, _typeIcon, _message, NSLayoutAttribute.Right,
                NSLayoutAttribute.Left, -spacing);
            ConstraintHelpers.AddRelativeConstraint(this, _message, _confirmButton, NSLayoutAttribute.Right,
                NSLayoutAttribute.Left, -spacing);
            ConstraintHelpers.AddRelativeConstraint(this, _confirmButton, this, NSLayoutAttribute.Right,
                NSLayoutAttribute.Right, -padding);
            ConstraintHelpers.AddEqualityConstraint(this, _confirmButton, this, NSLayoutAttribute.CenterY);
            ConstraintHelpers.AddEqualityConstraint(this, line, NSLayoutAttribute.Left);
            ConstraintHelpers.AddEqualityConstraint(this, line, NSLayoutAttribute.Right);
        }

        public override CGSize IntrinsicContentSize
        {
            get
            {
                return new CGSize(0, 50);
            }
        }

        public Severity Severity
        {
            get { return _severity; }
            set
            {
                _severity = value;
                _typeIcon.Image = UIImage.FromFile(value.GetFileName(ImageType.TypeIcon));
                _confirmButton.SetImage(UIImage.FromFile(value.GetFileName(ImageType.ConfirmIcon)), UIControlState.Normal);
            }
        }

        public string Message
        {
            get { return _message.Text; }
            set { _message.Text = value; }
        }
    }
}