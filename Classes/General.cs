using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace JDP {
    public static class General {
        public static string Version {
            get {
                Version ver = Assembly.GetExecutingAssembly().GetName().Version;
                return ver.Major + "." + ver.Minor + "." + ver.Revision;
            }
        }

        public static string ReleaseDate {
            get { return "2013-Jun-15"; }
        }

        public static string ApplicationName {
            get { return "ST4 Image Dumper"; }
        }

        public static string ApplicationURL {
            get { return "https://github.com/SuperGouge/ST4ImageDumper/releases"; }
        }

        public static Image ImageFromURL(string url) {
            return Image.FromStream(MemoryStreamFromURL(url));
        }

        public static string TextFromURL(string url, Encoding encoding) {
            using (MemoryStream memoryStream = MemoryStreamFromURL(url)) {
                return encoding.GetString(memoryStream.ToArray());
            }
        }

        public static string PostForm(string url, string referer, CookieContainer cookies, FormData formData, Encoding responseEncoding) {
            ServicePointManager.Expect100Continue = false;
            MemoryStream memoryStream = MemoryStreamFromURL(url,
                (request) => {
                    request.Method = "POST";
                    request.Referer = referer;
                    request.CookieContainer = cookies;
                    request.ContentType = formData.ContentType;
                    request.ContentLength = formData.GetContentLength();
                    using (Stream requestStream = request.GetRequestStream()) {
                        formData.WriteContent(requestStream);
                    }
                },
                (response) => {
                    if (cookies != null) {
                        cookies.Add(response.Cookies);
                    }
                });
            using (memoryStream) {
                return responseEncoding.GetString(memoryStream.ToArray());
            }
        }

        public static MemoryStream MemoryStreamFromURL(string url) {
            return MemoryStreamFromURL(url, null, null);
        }

        public static MemoryStream MemoryStreamFromURL(string url, Action<HttpWebRequest> withRequest, Action<HttpWebResponse> withResponse) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
            if (withRequest != null) {
                withRequest(request);
            }
            MemoryStream memoryStream = new MemoryStream();
            try {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream()) {
                    CopyStream(responseStream, memoryStream);
                    if (withResponse != null) {
                        withResponse(response);
                    }
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream;
            }
            catch {
                request.Abort();
                memoryStream.Dispose();
                throw;
            }
        }

        public static IEnumerable<FormField> GetFormFields(HTMLParser pageParser, HTMLTagRange formTagRange) {
            foreach (HTMLTag tag in pageParser.FindStartTags(formTagRange, "input", "textarea")) {
                string fieldType = tag.GetAttributeValueOrEmpty("type");
                string fieldName = tag.GetAttributeValueOrEmpty("name");
                string fieldValue = tag.GetAttributeValueOrEmpty("value");
                bool isText = tag.NameEquals("input") && new[] { "hidden", "text", "password" }.Contains(fieldType, StringComparer.OrdinalIgnoreCase);
                bool isTextArea = tag.NameEquals("textarea");
                bool isBoolean = tag.NameEquals("input") && new[] { "checkbox", "radio" }.Contains(fieldType, StringComparer.OrdinalIgnoreCase);
                bool isFile = tag.NameEquals("input") && fieldType.Equals("file", StringComparison.OrdinalIgnoreCase);

                FormField field;
                if (isText) {
                    field = new TextFormField(fieldName) { Text = fieldValue };
                }
                else if (isTextArea) {
                    field = new TextFormField(fieldName) { Text = pageParser.GetInnerHTML(pageParser.CreateTagRange(tag)) };
                }
                else if (isFile) {
                    field = new FileFormField(fieldName);
                }
                else if (isBoolean) {
                    field = new BooleanFormField(fieldName) { Value = fieldValue, Checked = tag.GetAttribute("checked") != null };
                }
                else {
                    continue;
                }
                field.Disabled = tag.GetAttribute("disabled") != null;
                yield return field;
            }
        }

        public static void CopyStream(Stream srcStream, params Stream[] dstStreams) {
            byte[] data = new byte[8192];
            while (true) {
                int dataLen = srcStream.Read(data, 0, data.Length);
                if (dataLen == 0) break;
                foreach (Stream dstStream in dstStreams) {
                    if (dstStream != null) {
                        dstStream.Write(data, 0, dataLen);
                    }
                }
            }
        }

        public static string GetAvailableFilePath(string directory, string baseFileName) {
            string filePath;
            int iSuffix = 1;
            do {
                filePath = Path.Combine(directory, Path.GetFileNameWithoutExtension(baseFileName) +
                                                   (iSuffix == 1 ? String.Empty : ("_" + iSuffix)) + Path.GetExtension(baseFileName));
                iSuffix++;
            } while (File.Exists(filePath));
            return filePath;
        }

        public static bool TryMoveFile(string pathSrc, string pathDst, int maxTries, TimeSpan retryDelay) {
            for (int i = 1; i <= maxTries; i++) {
                try {
                    File.Move(pathSrc, pathDst);
                    return true;
                }
                catch {
                    Thread.Sleep(retryDelay);
                }
            }
            return false;
        }

        public static int CompareNatural(string strA, string strB) {
            // Strings are compared in sections, where each section is a group of
            // non-digits or digits.  When comparing a group of non-digits to a group of
            // digits, the group of digits is considered less than the group of
            // non-digits, except if occurring at the beginning of the string, in which
            // case regular string comparison is used.  When comparing two groups of
            // non-digits, use regular string comparison.  When comparing two groups of
            // digits, compare by numeric value without regard to leading zeros.  If no
            // difference has been found by the end of the comparison, the first section of
            // digits with different zero padding (if present) determines the difference,
            // with the longer section being considered less than the shorter section.
            int iA = 0;
            int iB = 0;
            int softResult = 0;
            while (iA < strA.Length && iB < strB.Length) {
                bool isDigitA = IsDigit(strA[iA]);
                bool isDigitB = IsDigit(strB[iB]);
                if (isDigitA != isDigitB) {
                    if (iA == 0 && iB == 0) {
                        return String.Compare(strA, strB, StringComparison.CurrentCultureIgnoreCase);
                    }
                    else {
                        return isDigitA ? -1 : 1;
                    }
                }
                else if (!isDigitA && !isDigitB) {
                    int jA = iA + 1;
                    int jB = iB + 1;
                    while (jA < strA.Length && !IsDigit(strA[jA])) {
                        jA++;
                    }
                    while (jB < strB.Length && !IsDigit(strB[jB])) {
                        jB++;
                    }
                    int cmpResult = String.Compare(
                        strA.Substring(iA, jA - iA),
                        strB.Substring(iB, jB - iB),
                        StringComparison.CurrentCultureIgnoreCase);
                    if (cmpResult != 0) {
                        return cmpResult;
                    }
                    iA = jA;
                    iB = jB;
                }
                else {
                    bool foundNonZeroA = false;
                    bool foundNonZeroB = false;
                    do {
                        if (strA[iA] != '0') foundNonZeroA = true;
                        else iA++;
                    } while (!foundNonZeroA && iA < strA.Length && IsDigit(strA[iA]));
                    do {
                        if (strB[iB] != '0') foundNonZeroB = true;
                        else iB++;
                    } while (!foundNonZeroB && iB < strB.Length && IsDigit(strB[iB]));
                    if (foundNonZeroA != foundNonZeroB) {
                        return foundNonZeroA ? 1 : -1;
                    }
                    else if (foundNonZeroA && foundNonZeroB) {
                        int sameLenResult = 0;
                        while ((isDigitA = iA < strA.Length && IsDigit(strA[iA])) &
                               (isDigitB = iB < strB.Length && IsDigit(strB[iB])))
                               {
                            if (strA[iA] != strB[iB] && sameLenResult == 0) {
                                sameLenResult = strA[iA] < strB[iB] ? -1 : 1;
                            }
                            iA++;
                            iB++;
                        }
                        if (isDigitA != isDigitB) {
                            return isDigitA ? 1 : -1;
                        }
                        else if (sameLenResult != 0) {
                            return sameLenResult;
                        }
                    }
                    if (iA != iB && softResult == 0) {
                        softResult = iA > iB ? -1 : 1;
                    }
                }
            }
            if (iA < strA.Length || iB < strB.Length) {
                return iA < strA.Length ? 1 : -1;
            }
            if (softResult != 0) {
                return softResult;
            }
            return 0;
        }

        private static bool IsDigit(char c) {
            return c >= '0' && c <= '9';
        }

        public static int? TryParseInt32(string s) {
            int result;
            return Int32.TryParse(s, out result) ? result : (int?)null;
        }

        public static Uri GetAbsoluteURI(string baseURL, string relativeURL) {
            try {
                Uri uri;
                if (!Uri.TryCreate(new Uri(baseURL), relativeURL, out uri)) {
                    return null;
                }
                return uri;
            }
            catch {
                return null;
            }
        }

        public static string GetAbsoluteURL(string baseURL, string relativeURL) {
            Uri uri = GetAbsoluteURI(baseURL, relativeURL);
            if (uri == null) return null;
            // AbsoluteUri can throw undocumented Exception (e.g. for "mailto:+")
            try { return uri.AbsoluteUri; }
            catch {
                return null;
            }
        }

        public static string StripFragmentFromURL(string url) {
            int pos = url.IndexOf('#');
            return pos != -1 ? url.Substring(0, pos) : url;
        }

        public static string CleanPageURL(string url) {
            url = url.Trim();
            url = StripFragmentFromURL(url);
            if (url.Length == 0) return null;
            if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) 
            {
                url = "http://" + url;
            }
            if (url.IndexOf('/', url.IndexOf("//", StringComparison.Ordinal) + 2) == -1) return null;
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) return null;
            return uri.AbsoluteUri;
        }

        public static string Generate4chanPassword() {
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Range(0, 8).Select(n => chars[random.Next(0, chars.Length - 1)]).ToArray());
        }

        public static int StrLen(byte[] bytes) {
            for (int i = 0; i < bytes.Length; i++) {
                if (bytes[i] == 0) return i;
            }
            return bytes.Length;
        }

        public static int StrLenW(byte[] bytes) {
            for (int i = 0; i < bytes.Length - 1; i += 2) {
                if (bytes[i] == 0 && bytes[i + 1] == 0) return i / 2;
            }
            return bytes.Length / 2;
        }
    }
}