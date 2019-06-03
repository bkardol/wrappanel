using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WrapPanel
{
    public class WrapPanel : Panel
    {
        private readonly IDictionary<FrameworkElement, int> elementPositions;

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(WrapPanel), new PropertyMetadata(null, ItemsSource_Changed));

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(WrapPanel), new PropertyMetadata(null));

        public WrapPanel()
        {
            elementPositions = new Dictionary<FrameworkElement, int>();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Just take up all of the width
            Size finalSize = new Size { Width = double.IsInfinity(availableSize.Width) ? int.MaxValue : availableSize.Width };
            double x = 0d;
            double rowHeight = 0d;
            foreach (FrameworkElement child in GetChildrenOrdered())
            {
                // Tell the child control to determine the size needed
                child.Measure(availableSize);

                x += child.DesiredSize.Width;
                if (x > availableSize.Width)
                {
                    // this item will start the next row
                    x = child.DesiredSize.Width;

                    // adjust the height of the panel
                    finalSize.Height += rowHeight;
                    rowHeight = child.DesiredSize.Height;
                }
                else
                {
                    // Get the tallest item
                    rowHeight = Math.Max(child.DesiredSize.Height, rowHeight);
                }
            }
            if (double.IsInfinity(availableSize.Width))
                finalSize.Width = x;

            // Add the final height
            finalSize.Height += rowHeight;
            return finalSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect finalRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            double rowHeight = 0;

            foreach (var child in GetChildrenOrdered())
            {
                if ((child.DesiredSize.Width + finalRect.X) > finalSize.Width)
                {
                    // next row!
                    finalRect.X = 0;
                    finalRect.Y += rowHeight;
                    rowHeight = 0;
                }
                // Place the item
                child.Arrange(new Rect(finalRect.X, finalRect.Y, child.DesiredSize.Width, child.DesiredSize.Height));

                // adjust the location for the next items
                finalRect.X += child.DesiredSize.Width;
                rowHeight = Math.Max(child.DesiredSize.Height, rowHeight);
            }
            return finalSize;
        }

        private static void ItemsSource_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WrapPanel instance = d as WrapPanel;
            if (e.NewValue != null)
            {
                if (e.OldValue != null)
                    instance.ItemsSource_CollectionChanged(instance, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ((IEnumerable)e.OldValue).Cast<object>().ToList()));
                instance.RegisterCollectionChanged();
            }
        }

        private void RegisterCollectionChanged()
        {
            int i = -1;
            foreach (var item in ItemsSource)
                AddItem(item, ++i);
            if (ItemsSource is INotifyCollectionChanged incc)
                incc.CollectionChanged += ItemsSource_CollectionChanged;
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    {
                        var items = ItemsSource.Cast<object>().ToList();
                        foreach (var item in items)
                        {
                            if (!Children.Any(c => ((FrameworkElement)c).DataContext == item))
                                AddItem(item, e.NewStartingIndex);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            foreach (FrameworkElement child in Children)
                            {
                                if (elementPositions.TryGetValue(child, out int position) && position >= e.NewStartingIndex)
                                    elementPositions[child] = ++position;
                            }
                            AddItem(item, e.NewStartingIndex);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            var childToRemove = Children.FirstOrDefault(c => ((FrameworkElement)c).DataContext == item);
                            if (childToRemove != null)
                                Children.Remove(childToRemove);

                            foreach (FrameworkElement child in Children)
                            {
                                if (elementPositions.TryGetValue(child, out int position) && position > e.OldStartingIndex)
                                    elementPositions[child] = --position;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var items = ItemsSource.Cast<object>().ToList();
                        foreach (FrameworkElement child in Children)
                        {
                            var matchingItem = items.FirstOrDefault(i => i == child.DataContext);
                            if (matchingItem != null)
                                elementPositions[child] = items.IndexOf(matchingItem);
                        }
                    }
                    break;

            }
            InvalidateArrange();
        }

        private IEnumerable<FrameworkElement> GetChildrenOrdered()
        {
            return Children.Where(c => c is FrameworkElement).Select(c => (FrameworkElement)c).OrderBy(c =>
            {
                if (c is FrameworkElement && elementPositions.TryGetValue(c, out int position))
                    return position;
                else
                    return -1;
            }).ToList();
        }

        private void AddItem(object item, int index)
        {
            var childToAdd = new ContentControl
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                DataContext = item,
                ContentTemplate = ItemTemplate
            };
            elementPositions[childToAdd] = index;
            Children.Add(childToAdd);
        }
    }
}
