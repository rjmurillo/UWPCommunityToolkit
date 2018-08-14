// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;
using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.FunctionalTests.LongRunningJavaScript
{
    public abstract partial class LongRunningJavaScriptTestContext
    {
        public string Content { get; private set; }

        public bool LongRunningScriptDetectedEventRaised { get; private set; }

        public bool ScriptNotifyEventRaised { get; private set; }

        protected override void Given()
        {
            Content = @"
<!doctype html>
<head>
<script>
    function loadScript(delay) {
        const startTime = new Date().getTime();
        let currentTime = new Date().getTime();

        // Create a long running script by busy waiting until 20s have elapsed
        do {
            // Note: Must allocate in the inner loop to afford Chakra an opportunity to
            // detect the long running script.
            currentTime = new Date().getTime();
        } while ((currentTime - startTime) < delay);

        window.external.notify('Script running too long');
    }
</script>
</head>
<body><h1>Long running script test page</h1></body>
</html>
";

            base.Given();

            WebView.IsJavaScriptEnabled = true;
            WebView.IsScriptNotifyAllowed = true;

            // BUG: The content causes browsers to show prompt when loaded externally without a problem
            WebView.LongRunningScriptDetected += (o, e) =>
            {
                WriteLine($"{nameof(WebView.LongRunningScriptDetected)}: {e.ExecutionTime}");
                LongRunningScriptDetectedEventRaised = true;
            };

            WebView.ScriptNotify += (o, e) =>
            {
                WriteLine($"{nameof(WebView.ScriptNotify)}: {e.Value}");
                ScriptNotifyEventRaised = true;
            };

            async void OnWebViewOnNavigationCompleted(object o, WebViewControlNavigationCompletedEventArgs e)
            {
                try
                {
                    await WebView.InvokeScriptAsync("loadScript", "2000");
                }
                catch (Exception ex)
                {
                    const uint E_ABORT = unchecked(0x80004004);

                    switch ((uint) ex.HResult)
                    {
                        case E_ABORT:
                            break;
                        default:
                            throw;
                    }
                }
                finally
                {
                    Form.Close();
                }
            }

            WebView.NavigationCompleted += OnWebViewOnNavigationCompleted;
        }

        protected override void When()
        {
            NavigateToStringAndWaitForFormClose(Content);
        }
    }

    [TestClass]
    public partial class LongRunningJavaScriptTests : LongRunningJavaScriptTestContext
    {
        [TestMethod]
        [Timeout(TestConstants.Timeouts.Longest)]
        public void LongRunningJavaScriptEventRaised()
        {
            LongRunningScriptDetectedEventRaised.ShouldBeTrue();
            ScriptNotifyEventRaised.ShouldBeTrue();
        }
    }

    [TestClass]
    public partial class CancelledLongRunningJavaScriptEvent : LongRunningJavaScriptTestContext
    {
        protected override void Given()
        {
            base.Given();

            WebView.LongRunningScriptDetected += (o, e) =>
            {
                e.StopPageScriptExecution = true;
            };
        }

        [TestMethod]
        [Timeout(TestConstants.Timeouts.Longest)]
        public void LongRunningJavaScriptEventRaised()
        {
            LongRunningScriptDetectedEventRaised.ShouldBeTrue();
            ScriptNotifyEventRaised.ShouldBeFalse();
        }
    }
}
