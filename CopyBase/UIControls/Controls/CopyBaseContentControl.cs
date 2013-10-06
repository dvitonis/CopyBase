using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace UIControls.Controls
{
    /// <summary>
    /// Originally from http://xamlcoder.com/blog/2010/11/04/creating-a-metro-ui-style-control/
    /// </summary>
    public class CopyBaseContentControl : ContentControl
    {
        public static readonly DependencyProperty ReverseTransitionProperty = DependencyProperty.Register("ReverseTransition", typeof(bool), typeof(CopyBaseContentControl), new FrameworkPropertyMetadata(false));

        public bool ReverseTransition
        {
            get { return (bool)GetValue(ReverseTransitionProperty); }
            set { SetValue(ReverseTransitionProperty, value); }
        }

        public static readonly DependencyProperty TransitionsEnabledProperty = DependencyProperty.Register("TransitionsEnabled", typeof(bool), typeof(CopyBaseContentControl), new FrameworkPropertyMetadata(true));

        public bool TransitionsEnabled
        {
            get { return (bool)GetValue(TransitionsEnabledProperty); }
            set { SetValue(TransitionsEnabledProperty, value); }
        }

        public CopyBaseContentControl()
        {
            DefaultStyleKey = typeof(CopyBaseContentControl);

            Loaded += MetroContentControlLoaded;
            Unloaded += MetroContentControlUnloaded;

            IsVisibleChanged += MetroContentControlIsVisibleChanged;
        }

        void MetroContentControlIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (TransitionsEnabled)
            {
                if (!IsVisible)
                    VisualStateManager.GoToState(this, ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", false);
                else
                    VisualStateManager.GoToState(this, ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", true);
            }
        }

        private void MetroContentControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (TransitionsEnabled)
                VisualStateManager.GoToState(this, ReverseTransition ? "AfterUnLoadedReverse" : "AfterUnLoaded", false);
        }

        private void MetroContentControlLoaded(object sender, RoutedEventArgs e)
        {
            if (TransitionsEnabled)
                VisualStateManager.GoToState(this, ReverseTransition ? "AfterLoadedReverse" : "AfterLoaded", true);
            else
            {
                var root = ((Grid)GetTemplateChild("root"));
                root.Opacity = 1.0;
                var transform = ((System.Windows.Media.TranslateTransform)root.RenderTransform);
                if (transform.IsFrozen)
                {
                    var modifiedTransform = transform.Clone();
                    modifiedTransform.X = 0;
                    root.RenderTransform = modifiedTransform;
                }
                else
                {
                    transform.X = 0;
                }
            }
        }

        public void Reload()
        {
            if (!TransitionsEnabled) return;

            if (ReverseTransition)
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterUnLoadedReverse", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "BeforeLoaded", true);
                VisualStateManager.GoToState(this, "AfterLoaded", true);
            }
        }

        public override void OnApplyTemplate()
        {
            DependencyObject sb = GetTemplateChild("PART_AfterUnLoadedReverseStoryboard");
            if (sb != null)
            {
                (sb as Storyboard).Completed += AfterUnLoadedReverseCompleted;
            }

            base.OnApplyTemplate();
        }

        private void AfterUnLoadedReverseCompleted(object sender, EventArgs e)
        {
            OnExitAnimationCompleted(e);
        }

        public event EventHandler ExitAnimationCompleted;

        private void OnExitAnimationCompleted(EventArgs e)
        {
            EventHandler handler = ExitAnimationCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

    }
}
