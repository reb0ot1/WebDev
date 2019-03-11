using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SIS.HTTP.Cookies
{
    public class HttpCookieCollection : IHttpCookieCollection
    {
        private Dictionary<string, HttpCookie> httpCookieCollection;

        public HttpCookieCollection()
        {
            this.httpCookieCollection = new Dictionary<string, HttpCookie>();
        }

        public void Add(HttpCookie cookie)
        {
            if (!this.ContainsCookie(cookie.Key))
            {
                this.httpCookieCollection.Add(cookie.Key, cookie);
            }
        }

        public bool ContainsCookie(string key)
        {
            return this.httpCookieCollection.ContainsKey(key);
        }

        public HttpCookie GetCookie(string key)
        {
            if (this.ContainsCookie(key))
            {
                return this.httpCookieCollection[key];
            }

            return null;
        }

        public bool HasCookies()
        {
            return this.httpCookieCollection.Count > 0;
        }

        public IEnumerator<HttpCookie> GetEnumerator()
        {
            foreach (var cookie in this.httpCookieCollection)
            {
                yield return cookie.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join("; ", this.httpCookieCollection.Values);
        }
    }
}
