using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapServer.Common
{
    public class ServerConnectException:Exception
    {
        public ServerConnectException() { }
        public ServerConnectException(string msg):base(msg) { }
        public ServerConnectException(string msg,Exception inner) : base(msg,inner) { }
    }
}