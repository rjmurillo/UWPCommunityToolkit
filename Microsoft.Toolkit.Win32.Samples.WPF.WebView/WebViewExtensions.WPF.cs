// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Toolkit.Win32.UI.Controls;

namespace Microsoft.Toolkit.Win32.Samples.WebView
{
    public static partial class WebViewExtensions
    {
        // WPF Specific
        private static readonly ImageSource DefaultFavIcon;

        static WebViewExtensions()
        {
            DefaultFavIcon = ToImageSource(new MemoryStream(EmptyFavIconBytes));
        }

        public static void SetDefaultIcon(this Window window)
        {
            window.Icon = DefaultFavIcon;
        }

        public static async Task SetFavIconAsync(this IWebView webView, Window window)
        {
            foreach (var stream in await GetFavIconAsync(webView))
            {
                var bmp = stream?.ToImageSource();
                if (bmp != null)
                {
                    window.Icon = bmp;

                    return;
                }
            }

            window.SetDefaultIcon();
        }

        // Performs the conversion from a stream containing Icon information to a WPF object
        public static ImageSource ToImageSource(this Stream stream)
        {
            var image = new BitmapImage();
            image.BeginInit();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            image.StreamSource = stream;
            image.EndInit();

            return image;
        }
    }
}