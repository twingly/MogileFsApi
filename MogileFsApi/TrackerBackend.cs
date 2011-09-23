using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Primelabs.Twingly.MogileFsApi.Exceptions;

namespace Primelabs.Twingly.MogileFsApi
{
    public class TrackerBackend : ITrackerBackend
    {
        private Uri[] _trackers;
        private SocketWithReaderAndWriter _cachedSocket;
        private Dictionary<Uri, DateTime> _deadHosts;

        public TrackerBackend(Uri[] trackers)
        {
            _trackers = trackers;
            _deadHosts = new Dictionary<Uri, DateTime>();
        }

        public void Reload(Uri[] trackers)
        {
            if (trackers == null) 
                throw new ArgumentNullException("trackers");

            _trackers = trackers;
            _deadHosts = new Dictionary<Uri, DateTime>();
            SocketCloseAndDispose();
        }

        protected void SocketCloseAndDispose()
        {
            if (_cachedSocket != null) {
                _cachedSocket.Dispose();
                _cachedSocket = null;
            }
        }

        protected SocketWithReaderAndWriter GetSocketReaderAndWriter()
        {
            var random = new Random();
            var startIndex = random.Next(_trackers.Length - 1);

            for (int i = 0; i < _trackers.Length; i++) {
                var tracker = _trackers[(i + startIndex)%_trackers.Length];

                DateTime trackerLastTry;
                if (_deadHosts.TryGetValue(tracker, out trackerLastTry) && (DateTime.Now - trackerLastTry).TotalSeconds < 5)
                    continue;

                try {
                    var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                    
                    // TODO: Set timeouts from query-string.
                    socket.SendTimeout = 3000;
                    socket.ReceiveTimeout = 3000;
                    socket.Connect(tracker.Host, tracker.Port);

                    return new SocketWithReaderAndWriter(socket);
                } catch (Exception) {
                    // something went wrong. mark host as dead.. 
                    _deadHosts[tracker] = DateTime.Now;
                }
            }

            // didn't find anything! throw an exception!
            throw new NoTrackersException();
        }

        public IDictionary<string, string> DoRequest(string command, IDictionary<string, string> args)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (args == null) throw new ArgumentNullException("args");

            var argString = EncodeUrlParameters(args);
            var request = command + " " + argString + "\r\n";

            // Try first with the cached socket. If no good, we try again, if we get exception then, we throw..
            for (int i = 0; i < 2; i++) {
                if (_cachedSocket != null) {
                    try {
                        _cachedSocket.Writer.Write(request);
                        _cachedSocket.Writer.Flush();
                        break;
                    }
                    catch (Exception ex) {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        SocketCloseAndDispose();
                        if (i == 1)
                            throw new TrackerCommunicationException("problem finding a working tracker");
                    }
                }
                if (_cachedSocket == null) {
                    _cachedSocket = GetSocketReaderAndWriter();
                }
            }
            try {
                // ok - we finally got a message off to a tracker
                // now get a response
                var response = _cachedSocket.Reader.ReadLine();

                if (response == null)
                    throw new TrackerCommunicationException(
                        "Received null response from tracker at " + _cachedSocket.EndPoint);

                var ok = OK_PATTERN.Match(response);
                if (ok.Success) {
                    return DecodeUrlString(ok.Groups[ARGS_PART].Value);
                }

                var err = ERROR_PATTERN.Match(response);
                if (err.Success)
                {
                    // error response
                    LastErr = err.Groups[ERR_PART].Value;
                    LastErrStr = err.Groups[ERRSTR_PART].Value;

                    return null;
                }

                throw new TrackerCommunicationException(
                    "invalid server response from "
                    + _cachedSocket.EndPoint + ": "
                    + response);
                
            } catch (Exception ex) {
                throw new TrackerCommunicationException(
                    "problem talking to server at "
                    + _cachedSocket.EndPoint, ex);
            }
        }

        public string LastErr { get; protected set; }
        public string LastErrStr { get; protected set; }


        private static readonly Regex ERROR_PATTERN = new Regex("^ERR\\s+(\\w+)\\s*(\\S*)", RegexOptions.Compiled);
        private const int ERR_PART = 1;
        private const int ERRSTR_PART = 2;

        private static readonly Regex OK_PATTERN = new Regex("^OK\\s+\\d*\\s*(\\S*)", RegexOptions.Compiled);
        private const int ARGS_PART = 1;


        private static string EncodeUrlParameters(IDictionary<string, string> args)
        {
            var sb = new StringBuilder();
            foreach (var pair in args) {
                if (sb.Length > 0)
                    sb.Append("&");

                sb.Append(HttpUtility.UrlEncode(pair.Key, Encoding.UTF8));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(pair.Value, Encoding.UTF8));
            }
            return sb.ToString();
        }

        private static Dictionary<String, String> DecodeUrlString(string encoded)
        {
            var retval = new Dictionary<string, string>();

            var retval2 = HttpUtility.ParseQueryString(encoded, Encoding.UTF8);
            foreach (var key in retval2.AllKeys) {
                retval[key] = retval2[key];
            }
            return retval;
        }


        public bool IsConnected()
        {
            return _cachedSocket != null && _cachedSocket.Socket.Connected;
        }

        bool _isDisposed = false;
        public void Dispose()
        {
            if (! _isDisposed) {
                _isDisposed = true;
                SocketCloseAndDispose();
            }
        }
    }
}