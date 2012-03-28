using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Diagnostics;

namespace Jabbr.WPF.AttachedProperties
{
    public static class BindableInlines
    {
        public static void SetBindableInlines(DependencyObject dp, InlineCollection value)
        {
            dp.SetValue(BindableInlinesProperty, value);
        }

        public static InlineCollection GetBindableInlines(DependencyObject dp)
        {
            return (InlineCollection)dp.GetValue(BindableInlinesProperty);
        }

        public static readonly DependencyProperty BindableInlinesProperty =
            DependencyProperty.RegisterAttached("BindableInlines",
                                                typeof (InlineCollection), typeof (BindableInlines),
                                                new UIPropertyMetadata(null, OnBindableInlinesChanged));

        private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = d as TextBlock;
            if (tb == null)
                return;

            tb.Inlines.Clear();

            InlineCollection inlineCollection = e.NewValue as InlineCollection;
            if(inlineCollection == null || inlineCollection.Count <= 0)
                return;

            Inline[] inlines = new Inline[inlineCollection.Count];
            inlineCollection.CopyTo(inlines, 0);

            foreach (var inline in inlines)
            {
                var hyperlink = inline as Hyperlink;
                if (hyperlink != null)
                    (hyperlink).Click += (sender, args) =>
                    {
                        var hl = (Hyperlink) sender;
                        Process.Start(hl.NavigateUri.ToString());
                    };

                tb.Inlines.Add(inline);
            }
        }
    }
}
