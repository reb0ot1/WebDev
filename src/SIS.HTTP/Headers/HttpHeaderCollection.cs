using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIS.HTTP.Headers
{
    public class HttpHeaderCollection : IHttpHeaderCollection
    {
        private Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
        {
            this.headers = new Dictionary<string, HttpHeader>();
        }

        public void Add(HttpHeader header)
        {
            headers.Add(header.Key, header);
        }

        public bool ContainsHeader(string key)
        {
            return headers.ContainsKey(key);
        }

        public HttpHeader GetHeader(string key)
        {
            if (this.ContainsHeader(key))
            {
                return headers[key];
            }

            return null;
        }

        public override string ToString()
        {
            var showHeaders = headers.Values.Select(h => h.ToString()).ToArray();

            return string.Join("\r\n", showHeaders);
        }
    }
}
