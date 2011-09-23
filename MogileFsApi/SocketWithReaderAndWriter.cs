using System;
using System.IO;
using System.Net.Sockets;

namespace Primelabs.Twingly.MogileFsApi
{
    public class SocketWithReaderAndWriter: IDisposable
    {
        public SocketWithReaderAndWriter(Socket s)
        {
            Socket = s;
            Stream = new NetworkStream(s);
            Reader = new StreamReader(Stream);
            Writer = new StreamWriter(Stream);
        }

        public Socket Socket { get; private set; }
        public NetworkStream Stream { get; private set; }
        public StreamReader Reader { get; private set; }
        public StreamWriter Writer { get; private set; }

        public string EndPoint
        {
            get
            {
                return Socket.RemoteEndPoint.ToString();
            }
        }

        bool _isDisposed = false;
        public void Dispose()
        {
            if (!_isDisposed) {
                _isDisposed = true;
                try {
                    Reader.Dispose();
                }
                catch (Exception) {
                }
                try {
                    Writer.Dispose();
                }
                catch (Exception) {
                }
                try {
                    Stream.Dispose();
                } 
                catch (Exception) {
                }
            }
        }
    }
}