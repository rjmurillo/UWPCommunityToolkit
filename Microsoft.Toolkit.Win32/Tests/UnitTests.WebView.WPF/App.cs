// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WPF.WebView.FunctionalTests
{
    public class App : System.Windows.Application
    {
        public TestHostWindow Window => (TestHostWindow)MainWindow;
        public Controls.WPF.WebView WebView => Window.WebView1;

        public App()
        {
            MainWindow = new TestHostWindow();
        }
    }
}
