using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server {
    class Response {
        Byte[] data;
        String status;
        String mime;
        private Response(String status, String mime, Byte[] data) {
            this.data = data;
            this.status = status;
            this.mime = mime;
        }
        public static Response From(Request request) {
            if (request == null)
                return NotWork("501.html", "501 Not Implemented");
            if (request.Type == "GET") {
                String file = Environment.CurrentDirectory + HttpServer.WEB_DIR + request.Url;
                FileInfo fi = new FileInfo(file);
                if (fi.Exists && fi.Extension.Contains(".")) {
                    if (fi.Extension == ".jpg" || fi.Extension == ".gif" || fi.Extension == ".jpeg" || fi.Extension == ".png")
                        return MakePicture(fi);
                    return MakeFromFile(fi);
                } else {
                    if (request.Url.Equals("/timeout"))
                        return NotWork("408.html", "408 Request Timeout");
                    DirectoryInfo di = new DirectoryInfo(fi + "/");
                    if (!di.Exists)
                        return NotWork("404.html", "404 Page Not Found");
                    FileInfo[] files = di.GetFiles();
                    foreach (FileInfo ff in files) {
                        if (ff.Name.Contains("other.html") || ff.Name.Contains("griminov.html"))
                            return MakeFromFile(ff);
                    }
                }
            }
            else
                return NotWork("501.html", "501 Not Implemented");
            return NotWork("404.html", "404 Page Not Found");
        }

        private static Response MakePicture(FileInfo f)
        {
            FileStream stream = f.OpenRead();
            Byte[] b = new Byte[stream.Length];
            BinaryReader reader = new BinaryReader(stream);
            reader.Read(b, 0, b.Length);
            return new Response("200 OK", "image", b);
        }

        private static Response MakeFromFile(FileInfo f) {
            FileStream stream = f.OpenRead();
            Byte[] b = new Byte[stream.Length];
            BinaryReader reader = new BinaryReader(stream);
            reader.Read(b, 0, b.Length);
            return new Response("200 OK", "text/html", b);
        }

        private static Response NotWork(String fileName, String status) {
            String file = Environment.CurrentDirectory + HttpServer.MSG_DIR + fileName;
            FileInfo fi = new FileInfo(file);
            FileStream fs = fi.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] d = new Byte[fs.Length];
            reader.Read(d, 0, d.Length);
            return new Response(status, "text/html", d);
        }
        public void Post(NetworkStream stream) {
            StreamWriter writer = new StreamWriter(stream);
//            Console.WriteLine(String.Format("Response:\r\n{0} {1}\r\nServer: {2}\r\nContent-Language: ru\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\nConnection: close\r\n",
//                HttpServer.VERSION, status, HttpServer.SERVERNAME, mime, data.Length));
            writer.WriteLine(String.Format("{0} {1}\r\nServer: {2}\r\nContent-Language: ru\r\nContent-Type: {3}\r\nAccept-Ranges: bytes\r\nContent-Length: {4}\r\nConnection: close\r\nKeep-Alive: false\r\n", 
                HttpServer.VERSION, status, HttpServer.SERVERNAME, mime, data.Length));
            writer.Flush();
 //           Console.WriteLine(Encoding.UTF8.GetString(data, 0, data.Length));
            stream.Write(data, 0, data.Length);
        }
    }
}