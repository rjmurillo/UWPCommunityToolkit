// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WinForms.WebView.FunctionalTests.LongRunningJavaScript
{
    [TestClass]
    public class LongRunningJavaScriptTests : HostFormWebViewContextSpecification
    {
        private bool _slowEventRaised;
        private string _content = @"
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

        protected override void Given()
        {
            base.Given();
            WebView.IsJavaScriptEnabled = true;
            WebView.IsScriptNotifyAllowed = true;

            // BUG: The content causes browsers to show prompt when loaded externally without a problem
            WebView.LongRunningScriptDetected += (o, e) =>
            {
                WriteLine($"LongRunningScriptDetected: {e.ExecutionTime}");
                _slowEventRaised = true;
            };

            WebView.ScriptNotify += (o, e) =>
            {
                // Got to the end, didn't raise LongRunningScriptDetected
                Form.Close();
            };

            WebView.NavigationCompleted += async (o, e) =>
                {
                    try
                    {
                        await WebView.InvokeScriptAsync("loadScript", "2000");
                    }
                    catch (Exception ex)
                    {
                        const uint E_ABORT = unchecked(0x80004004);

                        switch ((uint)ex.HResult)
                        {
                            case E_ABORT:
                                break;
                            default:
                                throw;
                        }
                    }
                };
        }

        protected override void When()
        {
            NavigateToStringAndWaitForFormClose(_content);
        }

        [TestMethod]
        [Timeout(TestConstants.Timeouts.Longest)]
        //[Ignore("LongRunningScriptDetected event is not raised")]
        public void LongRunningJavaScriptEventRaised()
        {
            _slowEventRaised.ShouldBeTrue();
        }
    }
}
