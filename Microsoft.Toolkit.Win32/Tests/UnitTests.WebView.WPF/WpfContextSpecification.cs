// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WPF.WebView
{

    [TestCategory(TestConstants.Categories.Wpf)]
    public abstract class WpfContextSpecification : BlockTestStartEndContextSpecification
    {
        protected WpfContextSpecification()
        {
            Form = new TestHostWindow();

            Form.MouseEnter += (o, e) => { WriteLine($"Window.MouseEnter"); };
            Form.MouseWheel += (o, e) => { WriteLine($"Window.MouseWheel"); };
            Form.GotFocus += (o, e) => { WriteLine("Window.GotFocus"); };
            Form.LostFocus += (o, e) => { WriteLine("Window.LostFocus"); };
            //Form.KeyPress += (o, e) => { WriteLine($"Window.KeyPress: {e.KeyChar}"); };
            Form.Closing += (o, e) => { WriteLine("Window.Closing"); };
            Form.Closed += (o, e) => { WriteLine("Window.Closed"); };
        }

        protected new Controls.WPF.WebView WebView
        {
            get => (Controls.WPF.WebView)base.WebView;
            set => base.WebView = value;
        }

        // Helps with code portability with WinForms
        protected TestHostWindow Form { get; }

        protected override void Cleanup()
        {
            PrintStartEnd(
                TestContext.TestName,
                nameof(Cleanup),
                () =>
                {
                    try
                    {
                        if (Form != null)
                        {
                            // The Form is supposed to be closed when the test is completed (to signal it is done)
                            // If it has not been closed and disposed, go ahead and do that so we can unhook
                            WriteLine("Closing the Window instance...");
                            Form.Close();
                        }
                    }
                    finally
                    {
                        base.Cleanup();
                    }
                });
        }

        protected override void CreateWebView()
        {
            WebView = Form.WebView1;
        }
        protected override void Given()
        {
            Form.Title = TestContext.TestName;
            CreateWebView();

            WebView.ShouldNotBeNull();

            WebView.NavigationStarting += (o, e) => { Form.Title = $"{TestContext.TestName}: {e.Uri}" ?? string.Empty; };
            WebView.NavigationCompleted += (o, e) =>
            {
                var focused = WebView.Focus();
                WriteLine($"WebView.Focused: {focused}");
            };
        }

        protected override void PerformActionAndWaitForFormClose(Action callback)
        {
            void OnFormLoad(object sender, RoutedEventArgs e)
            {
                // This will call CreateWebView again, but at a time when the actual WebView is closer to loading
                // Since we're just assigning the instance, this is safe
                base.Given();

                // Need a version of DoEvents here?

                callback();
            }

            WebView.ShouldNotBeNull();
            Form.Loaded += OnFormLoad;
            Form.ShowDialog();
        }
    }
}
