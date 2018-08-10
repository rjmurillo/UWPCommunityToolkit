// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WPF.WebView
{
    [TestClass]
    public class NavigateStringUri : WpfContextSpecification
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                if (e.IsSuccess && e.Uri == TestConstants.Uris.ExampleOrg)
                {
                    _navigationCompleted = true;
                    Form.Close();
                }
            };
        }

        protected override void When()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WebView.Navigate(TestConstants.Uris.ExampleOrg.ToString());
            });
        }

        [TestMethod]
        public void NavigationCompleted()
        {
            _navigationCompleted.ShouldBeTrue();
        }
    }
}
