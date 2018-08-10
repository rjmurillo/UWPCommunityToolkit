// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared
{
    [DebuggerStepThrough]
    public abstract class WebViewContextSpecification : ContextSpecification
    {
        private static readonly Dictionary<uint, string> TestPids = new Dictionary<uint, string>();

        internal static string GetTestNameForProcessId(uint pid)
        {
            TestPids.TryGetValue(pid, out string test);
            return test;
        }

        private static bool _alreadyInBlock = false;
        protected virtual IWebView WebView { get; set; }

        protected virtual void NavigateAndWaitForFormClose(Uri uri)
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WriteLine($"Navigating WebView with URI: {uri}");
                WebView.Navigate(uri);
            });
        }

        protected virtual void NavigateAndWaitForFormClose(
            Uri requestUri,
            HttpMethod httpMethod,
            string content = null,
            IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            PerformActionAndWaitForFormClose(() =>
            {
                string Convert(IEnumerable<KeyValuePair<string, string>> kvp)
                {
                    if (kvp == null)
                    {
                        kvp = Enumerable.Empty<KeyValuePair<string, string>>();
                    }

                    var sb = new StringBuilder();
                    foreach (var k in kvp)
                    {
                        sb.AppendLine($"\r\n    {k.Key}={k.Value}");
                    }

                    return sb.ToString();
                }

                WriteLine(
@"Navigating WebView with
  URI:     {0}
  METHOD:  {1}
  CONTENT: {2}
  HEADERS: {3}",
                    requestUri, httpMethod, content ?? string.Empty, Convert(headers));
                WebView.Navigate(requestUri, httpMethod, content, headers);
            });
        }

        protected virtual void NavigateToLocalAndWaitForFormClose(string relativePath)
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WriteLine("Navigating WebView:");
                WebView.NavigateToLocal(relativePath);
            });
        }

        protected virtual void NavigateToLocalAndWaitForFormClose(Uri relativePath, IUriToStreamResolver streamResolver)
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WriteLine("Navigating WebView");
                WebView.NavigateToLocalStreamUri(relativePath, streamResolver);
            });
        }

        protected virtual void NavigateToStringAndWaitForFormClose(string content)
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WriteLine("Navigating WebView with content:");
                WriteLine(content);
                WebView.NavigateToString(content);
            });
        }

        protected virtual void PerformActionAndWaitForFormClose(Action callback)
        {
            callback();
        }

        protected override void Cleanup()
        {
            PrintStartEnd(
                TestContext.TestName,
                nameof(Cleanup),
                () =>
                {
                    UnsubscribeWebViewEvents();
                    TryAction(() =>
                    {
                        if (WebView != null)
                        {
                            if (WebView is WinForms.WebView wfwv)
                            {
                                if (!wfwv.IsDisposed)
                                {
                                    WriteLine("WebView is not null and has not been disposed. Calling Dispose()");
                                    wfwv.Dispose();
                                }
                            }
                            else if (WebView is IDisposable dwv)
                            {
                                WriteLine("WebView is not null. Calling Dispose()");
                                dwv.Dispose();
                            }

                            WebView = null;
                        }
                    });
                });

            base.Cleanup();
        }

        protected abstract void CreateWebView();

        protected override void Given()
        {
            if (!WinForms.WebView.IsSupported || !WPF.WebView.IsSupported)
            {
                // Test cannot execute because we're on the wrong OS
                Assert.Inconclusive(DesignerUI.E_NOTSUPPORTED_OS_RS4);
            }

            CreateWebView();
            WebView.ShouldNotBeNull();
            WriteLine($"Created WebView: {WebView.Version}");
            WireUpDiagnosticWebViewEvents();
            base.Given();
        }
        protected void PrintStartEnd(string className, string methodName, Action a)
        {
            try
            {
                if (!_alreadyInBlock)
                {
                    WriteLine($"\r\n=== Starting {className}.{methodName} ===");
                    _alreadyInBlock = true;
                }

                a();
            }
            finally
            {
                if (_alreadyInBlock)
                {
                    WriteLine($"=== Ending {className}.{methodName} ===\r\n");
                    _alreadyInBlock = false;
                }
            }
        }

        protected void TryAction(Action a)
        {
            try
            {
                a();
            }
            catch (Exception e)
            {
                if (e.IsSecurityOrCriticalException()) throw;
            }
        }

        protected void UnsubscribeWebViewEvents()
        {
            if (WebView == null) return;

            WebView.NavigationStarting -= OnNavigationStarting;
            WebView.ContentLoading -= OnContentLoading;
            WebView.DOMContentLoaded -= OnDomContentLoaded;
            WebView.NavigationCompleted -= OnNavigationCompleted;

            if (WebView is WinForms.WebView wfwv)
            {
                wfwv.Disposed -= OnDisposed;
                wfwv.GotFocus -= OnGotFocus;
                wfwv.LostFocus -= OnLostFocus;
            }

            if (WebView.Process != null)
            {
                WebView.Process.ProcessExited -= OnWebViewProcessExited;
            }
        }

        protected void WireUpDiagnosticWebViewEvents()
        {
            if (WebView == null) return;

            WebView.NavigationStarting += OnNavigationStarting;
            WebView.ContentLoading += OnContentLoading;
            WebView.DOMContentLoaded += OnDomContentLoaded;
            WebView.NavigationCompleted += OnNavigationCompleted;

            if (WebView is WinForms.WebView wfwv)
            {
                wfwv.Disposed += OnDisposed;
                wfwv.GotFocus += OnGotFocus;
                wfwv.LostFocus += OnLostFocus;
            }

            if (WebView.Process != null)
            {
                WebView.Process.ProcessExited += OnWebViewProcessExited;

                // NOTE: There are two PIDs we really care about. One for WWAHost.exe and one for the sandbox Win32WebViewHost.exe
                // The following is only for WWAHost.exe
                WriteLine($"{nameof(WebView)} created with PID {WebView.Process.ProcessId}\r\n");
                TestPids[WebView.Process.ProcessId] = TestContext.TestName;
            }
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WinForms.WebView.LostFocus)}");
        }

        private void OnGotFocus(object sender, EventArgs e)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WinForms.WebView.GotFocus)}");
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WinForms.WebView.Disposed)}");
        }

        private void OnContentLoading(object o, WebViewControlContentLoadingEventArgs a)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WebView.ContentLoading)}: {a.Uri?.ToString() ?? string.Empty}");
            Application.DoEvents();
        }

        private void OnDomContentLoaded(object o, WebViewControlDOMContentLoadedEventArgs a)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WebView.DOMContentLoaded)}: {a.Uri?.ToString() ?? string.Empty}");
            Application.DoEvents();
        }

        private void OnNavigationCompleted(object o, WebViewControlNavigationCompletedEventArgs a)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WebView.NavigationCompleted)}: Uri: {a.Uri?.ToString() ?? string.Empty}, Success: {a.IsSuccess}, Error: {a.WebErrorStatus}");
            Application.DoEvents();
        }
        private void OnNavigationStarting(object o, WebViewControlNavigationStartingEventArgs a)
        {
            WriteLine($"{WebView.GetType().Name}.{nameof(WebView.NavigationStarting)}: {a.Uri?.ToString() ?? string.Empty}");
            Application.DoEvents();
        }
        private void OnWebViewProcessExited(object o, object e)
        {
            WriteLine($"{nameof(WebView)} {nameof(WebViewControlProcess.ProcessExited)}!");
        }
    }
}