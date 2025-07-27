using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using FluentAvalonia.UI.Controls;
using Humanizer;
using Ryujinx.Ava.Common.Locale;
using Ryujinx.Ava.UI.Controls;
using Ryujinx.Ava.UI.Helpers;
using Starscript;
using Starscript.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.Ava.Systems.Starscript
{
    public partial class StarscriptTextBox : RyujinxControl<StarscriptTextBoxViewModel>
    {
        public IReadOnlyList<string> CurrentSuggestions => ViewModel.CurrentSuggestions;

        public ParserResult CurrentScriptSource => ViewModel.CurrentScriptSource;
        public Exception Exception => ViewModel.Exception;
        public Script CurrentScript => ViewModel.CurrentScript;
        public StringSegment CurrentScriptResult => ViewModel.CurrentScriptResult;
        
        public StarscriptTextBox()
        {
            InitializeComponent();
            
            InputBox.AsyncPopulator = GetSuggestionsAsync;
            InputBox.MinimumPopulateDelay = 0.Seconds();
            InputBox.TextFilter = (_, _) => true;
            InputBox.TextSelector = (text, suggestion) =>
            {
                if (text is not null && suggestion is null)
                    return text;
                if (text is null && suggestion is not null)
                    return suggestion;
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (text is null && suggestion is null)
                    return string.Empty;

                var sb = new StringBuilder(text.Length + suggestion.Length + 1);
                sb.Append(text);

                for (int i = 0; i < suggestion.Length - 1; i++)
                {
                    if (text.EndsWith(suggestion[..(suggestion.Length - i - 1)]))
                    {
                        suggestion = suggestion[(suggestion.Length - i - 1)..];
                        break;
                    }
                }

                sb.Append(suggestion);

                return sb.ToString();
            };

            Style textStyle = new(x => x.OfType<AutoCompleteBox>().Descendant().OfType<TextBlock>());
            textStyle.Setters.Add(new Setter(MarginProperty, new Thickness(0, 0)));
            
            Styles.Add(textStyle);
        }

        private Task<IEnumerable<object>> GetSuggestionsAsync(string input, CancellationToken token)
            => Task.FromResult(ViewModel.GetSuggestions(input, token));

        public static StarscriptTextBox Create(StarscriptHypervisor hv)
            => new() { ViewModel = new StarscriptTextBoxViewModel(hv) };
        
        public static async Task Show()
        {
            ContentDialog contentDialog = new()
            {
                PrimaryButtonText = string.Empty,
                SecondaryButtonText = string.Empty,
                CloseButtonText = LocaleManager.Instance[LocaleKeys.UserProfilesClose],
                Content = new StarscriptTextBox { ViewModel = new() }
            };

            await ContentDialogHelper.ShowAsync(contentDialog.ApplyStyles());
        }
    }
}
