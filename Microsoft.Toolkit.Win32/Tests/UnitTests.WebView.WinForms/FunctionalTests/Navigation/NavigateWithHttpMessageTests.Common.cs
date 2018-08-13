// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Should;

namespace Microsoft.Toolkit.Win32.UI.Controls.Test.WebView.FunctionalTests.Navigation
{
    [TestClass]
    [TestCategory(TestConstants.Categories.Nav)]
    public partial class HTTP_GET
    {
        private bool _success;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _success = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {
            NavigateAndWaitForFormClose(TestConstants.Uris.ExampleCom, HttpMethod.Get);
        }

        [TestMethod]
        [Timeout(TestConstants.Timeouts.Longest)]
        public void NavigationShouldComplete()
        {
            _success.ShouldBeTrue();
        }
    }

    [TestClass]
    [TestCategory(TestConstants.Categories.Nav)]
    public partial class HTTP_POST
    {
        private bool _success;
        private Uri _uri = new Uri(TestConstants.Uris.HttpBin, "/post");

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _success = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {

            NavigateAndWaitForFormClose(
                _uri,
                HttpMethod.Post,
                "{\"prop\":\"content\"}",
                new []{new KeyValuePair<string, string>("accept", "application/json"), });
        }

        [TestMethod]
        [Timeout(TestConstants.Timeouts.Longest)]
        public void NavigationShouldComplete()
        {
            _success.ShouldBeTrue();
        }
    }

    [TestClass]
    public partial class Navigate2Tests
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _navigationCompleted = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WebView.Navigate(TestConstants.Uris.HttpBin, HttpMethod.Get);
            });
        }

        [TestMethod]
        public void Explict_HTTP_GET_succeeds()
        {
            _navigationCompleted.ShouldBeTrue();
        }
    }

    [TestClass]
    public partial class NavigateGetWithHeaders
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _navigationCompleted = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                WebView.Navigate(
                    TestConstants.Uris.HttpBin,
                    HttpMethod.Get,
                    null,
                    new[] { new KeyValuePair<string, string>("pragma", "no-cache") });
            });
        }

        [TestMethod]
        public void Explict_HTTP_GET_with_HEADERS_succeeds()
        {
            _navigationCompleted.ShouldBeTrue();
        }
    }

    [TestClass]
    public partial class NavigateGetWithBasicAuth
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _navigationCompleted = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                const string user = "usr";
                const string password = "pwd";
                const string header = "Authorization";

                var authInfo = Convert.ToBase64String(Encoding.Default.GetBytes($"{user}:{password}"));

                WebView.Navigate(
                    new Uri(TestConstants.Uris.HttpBin, new Uri($"/basic-auth/{user}/{password}", UriKind.Relative)),
                    HttpMethod.Get,
                    null,
                    new[] { new KeyValuePair<string, string>(header, $"Basic {authInfo}") });
            });
        }

        [TestMethod]
        public void Explict_HTTP_GET_with_AUTH_BASIC_succeeds()
        {
            _navigationCompleted.ShouldBeTrue();
        }
    }

    [TestClass]
    public partial class NavigateOption
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _navigationCompleted = e.IsSuccess;
                Form.Close();
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        [Ignore("Pops UI that stalls test")]
        public void Explict_HTTP_OPTION_fails()
        {
            // This is here instead of in When
            // An exception is thrown which isn't caught property by the test framework
            PerformActionAndWaitForFormClose(() =>
            {
                WebView.Navigate(
                    TestConstants.Uris.ExampleCom,
                    HttpMethod.Options
                );
            });

            _navigationCompleted.ShouldBeFalse();
        }
    }

    [TestClass]
    public partial class NavigatePostWithContent
    {
        private bool _navigationCompleted;

        protected override void Given()
        {
            base.Given();
            WebView.NavigationCompleted += (o, e) =>
            {
                _navigationCompleted = e.IsSuccess;
                Form.Close();
            };
        }

        protected override void When()
        {
            PerformActionAndWaitForFormClose(() =>
            {
                string Foo()
                {
                    var c = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Foo", "Bar"), });
                    return c.ReadAsStringAsync().Result;
                }

                WebView.Navigate(
                    new Uri(TestConstants.Uris.HttpBin, "/post"),
                    HttpMethod.Post,
                    Foo()
                );
            });
        }

        [TestMethod]
        public void Explict_HTTP_POST_with_data_succeeds()
        {
            _navigationCompleted.ShouldBeTrue();
        }
    }
}
