using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Diagnostics;

namespace Jabbr.WPF.Resources.AttachedProperties
{
    public static class TextBlockHelper
    {
        public static void SetXamlString(DependencyObject dp, string value)
        {
            dp.SetValue(XamlStringProperty, value);
        }

        public static string GetXamlString(DependencyObject dp)
        {
            return (string) dp.GetValue(XamlStringProperty);
        }

        public static readonly DependencyProperty XamlStringProperty =
            DependencyProperty.RegisterAttached("XamlString", typeof (string), typeof (TextBlockHelper),
                                                new UIPropertyMetadata(null, OnXamlStringChanged));

        public static void SetBindableInlines(DependencyObject dp, Inline[] inlineCollection)
        {
            dp.SetValue(BindableInlinesProperty, inlineCollection);
        }

        public static Inline[] GetBindableInlines(DependencyObject dp)
        {
            return (Inline[]) dp.GetValue(BindableInlinesProperty);
        }

        public static readonly DependencyProperty BindableInlinesProperty =
            DependencyProperty.RegisterAttached("BindableInlines", typeof (Inline[]), typeof (TextBlockHelper),
                                                new UIPropertyMetadata(null, OnBindableInlinesChanged));

        private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = d as TextBlock;
            if (tb == null)
                return;

            Inline[] inlines = e.NewValue as Inline[];
            PopulateTextBlockInlines(tb, inlines);
        }

        private static void OnXamlStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBlock tb = d as TextBlock;
            if(tb == null)
                return;

            string xamlString = e.NewValue as string;
            if(string.IsNullOrEmpty(xamlString))
                return;

            InlineCollection inlineCollection = CreateInlineCollection(xamlString);
            if(inlineCollection == null || inlineCollection.Count == 0)
                return;

            Inline[] inlines = new Inline[inlineCollection.Count];
            inlineCollection.CopyTo(inlines, 0);

            PopulateTextBlockInlines(tb, inlines);
        }

        private static void PopulateTextBlockInlines(TextBlock tb, Inline[] inlines)
        {
            if (inlines == null || inlines.Length == 0 || tb == null)
                return;

            tb.Inlines.Clear();
            foreach (var inline in inlines)
            {
                var hyperLink = inline as Hyperlink;
                if (hyperLink != null)
                    hyperLink.Click += (sender, args) =>
                    {
                        var hl = (Hyperlink)sender;
                        Process.Start(hl.NavigateUri.ToString());
                    };

                tb.Inlines.Add(inline);
            }
        }

        private static InlineCollection CreateInlineCollection(string xamlString)
        {
            try
            {
                var parsedXaml = XamlReader.Parse(xamlString);
                return ((Paragraph) ((Section) parsedXaml).Blocks.FirstBlock).Inlines;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
