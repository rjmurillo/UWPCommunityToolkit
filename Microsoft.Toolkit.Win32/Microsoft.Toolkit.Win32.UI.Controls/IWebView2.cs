﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Toolkit.Win32.UI.Controls.Interop.WinRT;

namespace Microsoft.Toolkit.Win32.UI.Controls
{
    public interface IWebView2
    {
        /// <summary>
        /// Navigates the web view with the URI with a HTTP request and HTTP headers.
        /// </summary>
        /// <param name="requestUri">The Uniform Resource Identifier (URI) to send the request.</param>
        /// <param name="httpMethod">The HTTP method of the request.</param>
        /// <param name="content">Optional content to send with the request.</param>
        /// <param name="headers">Optional headers to send with the request.</param>
        /// <remarks>
        /// This method only supports <see cref="HttpMethod.Get"/> and <see cref="HttpMethod.Post"/> for the <paramref name="httpMethod"/> parameter.
        /// </remarks>
        /// <seealso cref="Windows.Web.UI.Interop.WebViewControl.NavigateWithHttpRequestMessage"/>
        void Navigate(
            Uri requestUri,
            HttpMethod httpMethod,
            string content = null,
            IEnumerable<KeyValuePair<string, string>> headers = null);

        /// <summary>
        /// Loads local web content at the specified Uniform Resource Identifier (URI) using an <see cref="IUriToStreamResolver"/>.
        /// </summary>
        /// <param name="relativePath">A path identifying the local HTML content to load.</param>
        /// <param name="streamResolver">A <see cref="IUriToStreamResolver"/> instance that converts a Uniform Resource Identifier (URI) into a stream to load.</param>
        void NavigateToLocalStreamUri(Uri relativePath, IUriToStreamResolver streamResolver);
    }
}