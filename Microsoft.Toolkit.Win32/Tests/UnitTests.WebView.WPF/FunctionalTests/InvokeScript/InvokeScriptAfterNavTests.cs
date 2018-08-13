// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.Toolkit.Win32.UI.Controls.Test.WinForms.WebView.FunctionalTests.NavigateToLocalStreamUri;

using System;
using System.Net.Http;

using HostFormWebViewContextSpecification = Microsoft.Toolkit.Win32.UI.Controls.Test.WPF.WebView.WpfContextSpecification;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.FunctionalTests.InvokeScript
{
    public abstract partial class InvokeScriptAfterNavigateContextSpecification : HostFormWebViewContextSpecification
    {
        protected override void Given()
        {
            base.Given();

            WebView.NavigationCompleted += async (o, e) =>
            {
                if (e.Uri == TestConstants.Uris.AboutBlank)
                {
                    return;
                }

                if (o is IWebView wv)
                {
                    Title = await wv.InvokeScriptAsync("eval", "document.title");
                }

                Form.Close();
            };
        }
    }

    public partial class InvokeScriptAfterNavigateLocal
    {
        protected override void When()
        {
            NavigateToLocalAndWaitForFormClose(new Uri(File, UriKind.Relative), new TestStreamResolver());
        }
    }

    public partial class InvokeScriptAfterNavigateToString
    {
        protected override void When()
        {
            NavigateToStringAndWaitForFormClose(Content);
        }
    }

    public partial class InvokeScriptAfterNavigateUri
    {
        protected override void When()
        {
            NavigateAndWaitForFormClose(TestConstants.Uris.ExampleCom);
        }
    }

    public partial class InvokeScriptAfterNavigateWithHttpMessage
    {
        protected override void When()
        {
            NavigateAndWaitForFormClose(TestConstants.Uris.ExampleCom, HttpMethod.Get);
        }
    }
}
