using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Request {
        public String Type { get; set; }
        public String Url { get; set; }
        private String Host { get; set; }
        public String Referer { get; set; }
        private Request(String type, String url, String host) {
            Type = type;
            Url = url;
            Host = host;
        }
        public static Request GetRequest(String msg) {
            if (String.IsNullOrEmpty(msg))
                return null;
            String[] tokens = msg.Split(' ');
            Console.WriteLine("Type is: {0}, url is: {1}, host is: {2}", tokens[0], tokens[1], tokens[3]);
            return new Request(tokens[0], tokens[1], tokens[3]);
        }
    }
}
