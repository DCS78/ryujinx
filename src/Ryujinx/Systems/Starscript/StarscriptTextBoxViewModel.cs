using CommunityToolkit.Mvvm.ComponentModel;
using Ryujinx.Ava.UI.ViewModels;
using Starscript;
using Starscript.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Ryujinx.Ava.Systems.Starscript
{
    public partial class StarscriptTextBoxViewModel : BaseModel
    {
        private readonly StarscriptHypervisor _hv;
        
        public StarscriptTextBoxViewModel(StarscriptHypervisor hv = null)
        {
            _hv = hv ?? RyujinxStarscript.Hypervisor;
        }
        
        public ObservableCollection<string> CurrentSuggestions { get; } = [];

        [ObservableProperty] private bool _hasError;
        [ObservableProperty] private StringSegment _currentScriptResult;
        [ObservableProperty] private string _errorMessage;
        private Exception _exception;
        private ParserResult _currentScriptSource;
        private Script _currentScript;
        
        public Exception Exception
        {
            get => _exception;
            set
            {
                ErrorMessage = (_exception = value) switch
                {
                    ParseException pe => pe.Error.ToString(),
                    StarscriptException se => se.Message,
                    _ => string.Empty
                };

                HasError = value is not null;
                
                OnPropertyChanged();
            }
        }
        
        public ParserResult CurrentScriptSource
        {
            get => _currentScriptSource;
            set
            {
                _currentScriptSource = value;

                if (value is null)
                {
                    CurrentScript = null;
                    CurrentScriptResult = null;
                    Exception = null;
                    return;
                }
                
                CurrentScript = Compiler.SingleCompile(value);
                Exception = null;

                OnPropertyChanged();
            }
        }

        public Script CurrentScript
        {
            get => _currentScript;
            private set
            {
                try
                {
                    CurrentScriptResult = value?.Execute(_hv)!;
                    _currentScript = value;
                    Exception = null;
                }
                catch (StarscriptException se)
                {
                    _currentScript = null;
                    CurrentScriptResult = null;
                    Exception = se;
                }

                OnPropertyChanged();
            }
        }

        public void ReExecuteScript()
        {
            if (_currentScript is null) return;

            try
            {
                CurrentScriptResult = _currentScript.Execute(_hv)!;
            }
            catch (StarscriptException se)
            {
                CurrentScriptResult = null;
                Exception = se;
            }
        }

        public IEnumerable<object> GetSuggestions(string input, CancellationToken token)
        {
            CurrentSuggestions.Clear();
            
            CurrentScriptSource = _hv.ParseAndGetCompletions(input, input.Length, CompletionCallback, token);

            if (CurrentScriptSource.HasErrors)
            {
                Exception = new ParseException(CurrentScriptSource.Errors.First());
            }
            
            OnPropertyChanged(nameof(CurrentSuggestions));

            return CurrentSuggestions;
        }

        private void CompletionCallback(string result, bool isFunction) => CurrentSuggestions.Add(isFunction ? $"{result}(" : result);
    }
}
