// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.FunctionalTests.Navigation
{
    [TestClass]
    public partial class NavigateRelativeUri
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NavigationFailedWithArgumentException()
        {
            WebView.Navigate(new Uri("/someresource", UriKind.Relative));
        }
    }

    [TestClass]
    public partial class NavigateFilePath
    {
        private string path;

        protected override void Given()
        {
            var fileName = Guid.NewGuid().ToString("N") + ".txt";
            path = Path.Combine(TestContext.TestRunResultsDirectory, fileName);

            File.WriteAllText(
                path,
                @"
<!DOCTYPE html>
<head><title>HTML on Disk</title></head>
<body><h1>HTML on Disk</h1></body>
</html>
");

            base.Given();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "E_ABORT expected")]
        [Ignore]
        public void Navigate()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WebView.Navigate(new Uri(path));
            });
        }
    }
}
