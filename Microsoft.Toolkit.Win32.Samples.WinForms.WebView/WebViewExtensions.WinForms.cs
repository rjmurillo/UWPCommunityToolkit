// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Toolkit.Win32.UI.Controls;

namespace Microsoft.Toolkit.Win32.Samples.WebView
{
    public static partial class WebViewExtensions
    {
        // WinForms specific
        private static readonly Icon DefaultFavIcon;

        static WebViewExtensions()
        {
            DefaultFavIcon = ToIcon(new MemoryStream(EmptyFavIconBytes));
        }

        public static void SetDefaultIcon(this Form form)
        {
            form.Icon = DefaultFavIcon;
        }

        public static async Task SetFavIconAsync(this IWebView webView, Form form)
        {
            foreach (var stream in await GetFavIconAsync(webView))
            {
                var ico = stream?.ToIcon();
                if (ico != null)
                {
                    form.Icon = ico;
                    return;
                }
            }

            form.SetDefaultIcon();
        }

        // Performs the conversion from a stream containing Icon information to an Icon object
        public static Icon ToIcon(this Stream stream)
        {
            var bmp = (Bitmap) Image.FromStream(stream);
            if (bmp != null)
            {
                var ico = Icon.FromHandle(bmp.GetHicon());
                return ico;
            }

            return null;
        }
    }
}