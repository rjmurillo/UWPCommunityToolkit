// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Markup;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WPF.WebView.FunctionalTests
{
    public class TestHostWindow : Window, IComponentConnector
    {
        internal Microsoft.Toolkit.Win32.UI.Controls.WPF.WebView WebView1;

        private bool _contentLoaded;

        public TestHostWindow() => InitializeComponent();

        public void Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                default:
                    _contentLoaded = true;
                    break;
            }
        }

        public void InitializeComponent()
        {
            if (!_contentLoaded)
            {
                _contentLoaded = true;

                // TODO: LoadComponents
                WebView1 = new Controls.WPF.WebView
                {
                    Name = "WebView1",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Height = Height,
                    Width = Width,
                    MinHeight = 200,
                    MinWidth = 200
                };
                WebView1.BeginInit();
                WebView1.EndInit();

                Content = WebView1;
            }
        }
    }
}
