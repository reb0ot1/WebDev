using System;
using System.Collections.Generic;
using System.Text;

namespace SIS.HTTP.Sessions
{
    public class HttpSession : IHttpSession
    {
        private readonly IDictionary<string, object> parameters;

        public HttpSession(string id)
        {
            this.Id = id;
            this.parameters = new Dictionary<string, object>();
        }

        public string Id { get; }

        public void AddParameter(string name, object parameter)
        {
            //if (this.ContainsParameter(name) == false)
            //{
            //    this.parameters.Add(name, new object());
            //}

            this.parameters.Add(name ,parameter);
        }

        public void ClearParameters()
        {
            this.parameters.Clear();
        }
        
        public bool ContainsParameter(string name)
        {
            return this.parameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            if (this.parameters.ContainsKey(name))
            {
                return this.parameters[name];
            }

            return null;
        }
    }
}
