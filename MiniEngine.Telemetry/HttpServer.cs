using System;
using System.Net;
using System.Text;
using System.Threading;

namespace MiniEngine.Telemetry
{
    internal sealed class HttpServer
    {
        private Thread thread;

        public HttpServer(int port)
        {
            this.Port = port;
        }

        public int Port { get; }
        public bool IsActive { get; private set; }


        public void Start(Func<string> getBody)
        {            
            this.thread = new Thread(new ParameterizedThreadStart(this.Listen))
            {
                Priority = ThreadPriority.BelowNormal,
                IsBackground = true
            };
            this.IsActive = true;
            this.thread.Start(getBody);            
        }

        public void Stop()
        {
            this.IsActive = false;            
            this.thread.Join();
        }

        private void Listen(object parameter)
        {
            var getBody = (Func<string>)parameter;
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add($"http://localhost:{this.Port}/");
                listener.Start();

                while (this.IsActive)
                {
                    var context = listener.GetContext();
                    var body = Encoding.UTF8.GetBytes(getBody());

                    context.Response.ContentLength64 = body.Length;
                    using (var output = context.Response.OutputStream)
                    {
                        output.Write(body, 0, body.Length);
                    }
                }
            }
        }        
    }
}
