// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using ManagedCommon;
using Wox.Plugin;

namespace Dictionary
{
    public class Dictionary : IPlugin, IContextMenu, IDisposable
    {
        public string Name => "Dictionary";

        public string Description => "Provides a simple dictionary for looking up definitions.";

        public static string PluginID => "07d77899358bcfabc3608f9d4c59966c";

        private PluginInitContext _context;
        private static string _icon_path;
        private bool _disposed;

        private InputInterpreter _inputInterpreter;

        public void Init(PluginInitContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(paramName: nameof(context));
            }

            _context = context;
            _context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(_context.API.GetCurrentTheme());

            _inputInterpreter = new InputInterpreter();
        }

        public List<Result> Query(Query query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(paramName: nameof(query));
            }

            // Parse
            List<DictionarySearchResult> resultList = _inputInterpreter.QueryDictionary(query.Search);
            if (resultList?.Any() != true)
            {
                return new List<Result>();
            }

            // Convert
            return resultList
              .Select(str => GetResult(str))
              .ToList();
        }

        private Result GetResult(DictionarySearchResult result)
        {
            return new Result
            {
                ContextData = result,
                Title = string.Format("{0}: {1}", result.KeyString, result.ResultString),
                IcoPath = _icon_path,
                Score = result.Score,
                SubTitle = string.Format("Copy {0} to clipboard", result.ResultString),
                Action = c =>
                {
                    var ret = false;
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            Clipboard.SetText(result.ResultString);
                            ret = true;
                        }
                        catch (ExternalException)
                        {
                            MessageBox.Show("Copy failed");
                        }
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    return ret;
                },
            };
        }

        private ContextMenuResult CreateContextMenuEntry(DictionarySearchResult result)
        {
            return new ContextMenuResult
            {
                PluginName = Name,
                Title = "Copy (Enter)",
                Glyph = "\xE8C8",
                FontFamily = "Segoe MDL2 Assets",
                AcceleratorKey = Key.Enter,
                Action = _ =>
                {
                    bool ret = false;
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            Clipboard.SetText(result.ResultString);
                            ret = true;
                        }
                        catch (ExternalException)
                        {
                            MessageBox.Show("Copy failed");
                        }
                    });
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                    return ret;
                },
            };
        }

        public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
        {
            if (!(selectedResult?.ContextData is DictionarySearchResult))
            {
                return new List<ContextMenuResult>();
            }

            List<ContextMenuResult> contextResults = new List<ContextMenuResult>();
            DictionarySearchResult result = selectedResult.ContextData as DictionarySearchResult;
            contextResults.Add(CreateContextMenuEntry(result));

            return contextResults;
        }

        public string GetTranslatedPluginTitle()
        {
            return "Dictionary";
        }

        public string GetTranslatedPluginDescription()
        {
            return "Provides a simple dictionary for looking up definitions.";
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private static void UpdateIconPath(Theme theme)
        {
            if (theme == Theme.Light || theme == Theme.HighContrastWhite)
            {
                _icon_path = "Images/dictionary.light.png";
            }
            else
            {
                _icon_path = "Images/dictionary.dark.png";
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_context != null && _context.API != null)
                    {
                        _context.API.ThemeChanged -= OnThemeChanged;
                    }

                    _disposed = true;
                }
            }
        }
    }
}
