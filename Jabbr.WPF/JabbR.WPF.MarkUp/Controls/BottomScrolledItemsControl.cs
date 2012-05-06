using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JabbR.WPF.MarkUp.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:JabbR.WPF.MarkUp.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:JabbR.WPF.MarkUp.Controls;assembly=JabbR.WPF.MarkUp.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class BottomScrolledItemsControl : ItemsControl
    {
        private ScrollViewer _scrollViewer;
        private bool _isAtBottom;
        private object _topItem;

        static BottomScrolledItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BottomScrolledItemsControl), new FrameworkPropertyMetadata(typeof(BottomScrolledItemsControl)));
            ItemsPanelTemplate itemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(VirtualizingStackPanel)));
            ItemsPanelProperty.OverrideMetadata(typeof(BottomScrolledItemsControl), new FrameworkPropertyMetadata(itemsPanel));
        }

        #region ScrollTopThreshold Property

        public static readonly DependencyProperty ScrollTopThresholdProperty =
            DependencyProperty.Register("ScrollTopThreshold", typeof(double), typeof(BottomScrolledItemsControl), new PropertyMetadata(default(double)));

        public double ScrollTopThreshold
        {
            get { return (double)GetValue(ScrollTopThresholdProperty); }
            set { SetValue(ScrollTopThresholdProperty, value); }
        } 

        #endregion

        #region ScrolledToTop Routed Event

        public static readonly RoutedEvent ScrolledToTopEvent = EventManager.RegisterRoutedEvent(
            "ScrolledToTop",
            RoutingStrategy.Bubble,
            typeof(EventHandler<RoutedEventArgs>),
            typeof(BottomScrolledItemsControl));

        public event EventHandler<RoutedEventArgs> ScrolledToTop
        {
            add { AddHandler(ScrolledToTopEvent, value); }
            remove { RemoveHandler(ScrolledToTopEvent, value); }
        }

        protected virtual void OnScrolledToTop()
        {
            if (Items.Count > 0)
                _topItem = Items[0];

            RaiseEvent(new RoutedEventArgs(ScrolledToTopEvent, this));
        }

        #endregion

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset && _topItem != null)
            {
                var item = ItemContainerGenerator.ContainerFromItem(_topItem);
                if(item == null)
                    return;

                var frameworkElement = item as FrameworkElement;
                if(frameworkElement != null)
                    frameworkElement.BringIntoView();
                else if (!IsGrouping && Items.Contains(_topItem))
                {
                }
            }

            base.OnItemsChanged(e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _scrollViewer = GetTemplateChild("scrollViewer") as ScrollViewer;
            
            
            if(_scrollViewer == null)
                return;
                
            _scrollViewer.ScrollToBottom();
            _scrollViewer.ScrollChanged += OnScrollChanged;
            
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            if(_isAtBottom && scrollChangedEventArgs.ExtentHeightChange > 0)
                _scrollViewer.ScrollToBottom();

            _isAtBottom = _scrollViewer.ScrollableHeight.Equals(_scrollViewer.VerticalOffset);

            int scrollTopCompareValue = _scrollViewer.VerticalOffset.CompareTo(ScrollTopThreshold);
            if(scrollTopCompareValue <= 0)
                OnScrolledToTop();
        }
    }
}
