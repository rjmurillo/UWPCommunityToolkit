// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Should;

using System.IO;
using System.Text;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.FunctionalTests.InvokeScript
{
    [TestCategory(TestConstants.Categories.Nav)]
    public abstract partial class InvokeScriptAfterNavigateContextSpecification
    {
        public string Title { get; private set; }

        [TestMethod]
        public void DocumentTitleIsNotNull()
        {
            Title.ShouldNotBeNull();
            Title.ShouldNotBeEmpty();
        }
    }

    [TestClass]
    public partial class InvokeScriptAfterNavigateLocal : InvokeScriptAfterNavigateContextSpecification
    {
        private const string Content = @"<!DOCTYPE html><head><title>Document Title</title></head><body></body></html>";

        protected string File { get; private set; }

        protected override void Given()
        {
            base.Given();

            File = "invokeScript.htm";

            // Write out content to disk
            var path = Path.GetDirectoryName(typeof(InvokeScriptAfterNavigateLocal).Assembly.Location);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            System.IO.File.WriteAllText(Path.Combine(path, File), Content, Encoding.UTF8);
        }
    }

    [TestClass]
    public partial class InvokeScriptAfterNavigateToString : InvokeScriptAfterNavigateContextSpecification
    {
        protected const string Content = @"<!DOCTYPE html><head><title>Document Title</title></head><body></body></html>";
    }

    [TestClass]
    public partial class InvokeScriptAfterNavigateUri : InvokeScriptAfterNavigateContextSpecification
    {
    }

    [TestClass]
    public partial class InvokeScriptAfterNavigateWithHttpMessage : InvokeScriptAfterNavigateContextSpecification
    {
    }
}
