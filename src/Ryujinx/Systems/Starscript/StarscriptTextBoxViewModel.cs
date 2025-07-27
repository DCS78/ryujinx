using CommunityToolkit.Mvvm.ComponentModel;
using Ryujinx.Ava.UI.ViewModels;
using Starscript;
using Starscript.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private string _currentScriptSource;
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
                
                OnPropertyChanged();
            }
        }
        
        public string CurrentScriptSource
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
                    HasError = false;
                    return;
                }

                try
                {
                    CurrentScript = Compiler.DirectCompile(CurrentScriptSource);
                    Exception = null;
                    HasError = false;
                }
                catch (ParseException pe)
                {
                    CurrentScript = null;
                    CurrentScriptResult = null;
                    Exception = pe;
                    HasError = true;
                }

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
                    HasError = false;
                }
                catch (StarscriptException se)
                {
                    _currentScript = null;
                    CurrentScriptResult = null;
                    Exception = se;
                    HasError = true;
                }

                OnPropertyChanged();
            }
        }

        public IEnumerable<object> GetSuggestions(string input, CancellationToken token)
        {
            CurrentScriptSource = input;
            
            _hv.GetCompletions(CurrentScriptSource, CurrentScriptSource.Length, CreateCallback(), token);
            
            OnPropertyChanged(nameof(CurrentSuggestions));

            return CurrentSuggestions;
        }

        private CompletionCallback CreateCallback()
        {
            CurrentSuggestions.Clear();
            
            return (result, isFunction) => 
                CurrentSuggestions.Add(isFunction ? $"{result}(" : result);
        }
    }
}
