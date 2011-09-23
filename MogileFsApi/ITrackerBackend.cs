using System;
using System.Collections.Generic;

namespace Primelabs.Twingly.MogileFsApi
{
    public interface ITrackerBackend : IDisposable
    {
        void Reload(Uri[] trackers);
        IDictionary<string, string> DoRequest(string command, IDictionary<string, string> args);
        bool IsConnected();
        string LastErr { get; }
        string LastErrStr { get; }
    }
}