using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MASA.Views.Controls.HackerNews
{
    public sealed partial class CommentControl : UserControl
    {
        private static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(String),
            typeof(CommentControl), new PropertyMetadata(String.Empty, TextChanged));

        private static void TextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((CommentControl) dependencyObject).TextChanged(
                (String) dependencyPropertyChangedEventArgs.OldValue,
                (String) dependencyPropertyChangedEventArgs.NewValue);
        }

        private static int AbsMin(params int[] numbers)
        {
            return numbers.Where(n => n > -1).Min();
        }

        private readonly String tagParagraph = "<p>";
        private readonly String tagItalics = "<i>";
        private readonly String tagItalicsEnd = "</i>";
        private readonly String tagLink = "<a href=";
        private readonly String tagLinkEnd = "</a>";
        private readonly String tagCode = "<pre><code>";
        private readonly String tagCodeEnd = "</code></pre>";

        private void TextChanged(String oldValue, String newValue)
        {
            var inlines = CommentTextBlock.Inlines;
            inlines.Clear();

            var filteredText = WebUtility.HtmlDecode(newValue);
            var text = filteredText;

            while (text.Any())
            {
                int matchParagraph = text.IndexOf(tagParagraph, StringComparison.CurrentCulture);
                int matchItalics = text.IndexOf(tagItalics, StringComparison.CurrentCulture);
                int matchLink = text.IndexOf(tagLink, StringComparison.CurrentCulture);
                int matchCode = text.IndexOf(tagCode, StringComparison.CurrentCulture);

                if (matchParagraph != -1 &&
                    matchParagraph == AbsMin(matchParagraph, matchItalics, matchLink, matchCode))
                {
                    inlines.Add(new Run()
                    {
                        Text = text.Substring(0, matchParagraph)
                    });
                    inlines.Add(new LineBreak());
                    inlines.Add(new LineBreak());

                    text = text.Remove(0, matchParagraph + tagParagraph.Length);
                }
                else if (matchItalics != -1 &&
                         matchItalics == AbsMin(matchParagraph, matchItalics, matchLink, matchCode))
                {
                    inlines.Add(new Run()
                    {
                        Text = text.Substring(0, matchItalics)
                    });

                    text = text.Remove(0, matchItalics + tagItalics.Length);

                    int italicsEnd = text.IndexOf(tagItalicsEnd, StringComparison.CurrentCulture);
                    var italic = new Italic();
                    italic.Inlines.Add(new Run()
                    {
                        Text = text.Substring(0, italicsEnd)
                    });
                    inlines.Add(italic);

                    text = text.Remove(0, italicsEnd + tagItalicsEnd.Length);
                }
                else if (matchLink != -1 &&
                         matchLink == AbsMin(matchParagraph, matchItalics, matchLink, matchCode))
                {
                    inlines.Add(new Run()
                    {
                        Text = text.Substring(0, matchLink)
                    });

                    text = text.Remove(0, matchLink + tagLink.Length);

                    int closing = text.IndexOf(">", StringComparison.CurrentCulture);
                    String linkUrl = text.Substring(0, closing).Trim('\"');

                    text = text.Remove(0, closing + ">".Length);

                    int linkEnd = text.IndexOf(tagLinkEnd, StringComparison.CurrentCulture);
                    var linkText = new Run()
                    {
                        Text = text.Substring(0, linkEnd)
                    };

                    text = text.Remove(0, linkEnd + tagLinkEnd.Length);

                    var hyperlink = new Hyperlink();
                    hyperlink.NavigateUri = new Uri(linkUrl);
                    hyperlink.Inlines.Add(linkText);
                    inlines.Add(hyperlink);
                }
                else if (matchCode != -1 &&
                         matchCode == AbsMin(matchParagraph, matchItalics, matchLink, matchCode))
                {
                    inlines.Add(new Run()
                    {
                        Text = text.Substring(0, matchCode)
                    });

                    text = text.Remove(0, matchCode + tagCode.Length);

                    int codeEnd = text.IndexOf(tagCodeEnd, StringComparison.CurrentCulture);
                    var code = new Run()
                    {
                        Text = text.Substring(0, codeEnd)
                    };
                    inlines.Add(code);

                    text = text.Remove(0, codeEnd + tagCodeEnd.Length);
                }
                else
                {
                    inlines.Add(new Run()
                    {
                        Text = text
                    });

                    text = text.Remove(0, text.Length);
                }
            }
        }

        public String Text
        {
            get { return (String) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public CommentControl()
        {
            this.InitializeComponent();
        }
    }
}
