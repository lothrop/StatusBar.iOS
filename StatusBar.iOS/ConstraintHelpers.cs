using MonoTouch.UIKit;

namespace StatusBar.iOS
{
    public static class ConstraintHelpers
    {
        public static NSLayoutConstraint AddConstantConstraint(UIView parent, UIView child, NSLayoutAttribute attribute,
            float constant)
        {
            child.TranslatesAutoresizingMaskIntoConstraints = false;

            var constraint = NSLayoutConstraint.Create(child, attribute, NSLayoutRelation.Equal, null,
                                                       NSLayoutAttribute.NoAttribute, 1, constant);

            parent.AddConstraint(constraint);

            return constraint;
        }

        public static NSLayoutConstraint AddEqualityConstraint(UIView parent, UIView child, NSLayoutAttribute attribute)
        {
            return AddEqualityConstraint(parent, child, parent, attribute);
        }

        public static NSLayoutConstraint AddEqualityConstraint(UIView parent, UIView child, UIView relativeToView,
            NSLayoutAttribute attribute)
        {
            return AddRelativeConstraint(parent, child, relativeToView, attribute, attribute);
        }

        public static NSLayoutConstraint AddRelativeConstraint(UIView parent, UIView child, UIView relativeToView,
            NSLayoutAttribute childAttribute, NSLayoutAttribute relativeToAttribute, float constant = 0)
        {
            child.TranslatesAutoresizingMaskIntoConstraints = false;

            var constraint = NSLayoutConstraint.Create(child, childAttribute, NSLayoutRelation.Equal, relativeToView,
                                                       relativeToAttribute, 1, constant);

            parent.AddConstraint(constraint);

            return constraint;
        }
    }
}
