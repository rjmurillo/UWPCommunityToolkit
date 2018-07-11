using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.Win32.Samples.WinForms.WebView
{
    public static class Common
    {
        public static string ToTrimmedUri(this Uri uri)
        {
            if (uri == null)
            {
                return string.Empty;
            }

            // The trimmed URI should consist of the host without the "www." prefix
            var host = uri.Host;
            const string wwwHostPrefix = "www.";
            var i = host.IndexOf(wwwHostPrefix, StringComparison.OrdinalIgnoreCase);

            if (i == 0)
            {
                host = host.Substring(i + wwwHostPrefix.Length);
            }

            return host;
        }
    }
}
