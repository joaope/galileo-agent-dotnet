using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public class Timings
    {
        public double Send { get; }

        public double Wait { get;  }

        public double Receive { get; }

        public double Blocked { get; }

        public double Connect { get; }

        public Timings(double send, double wait, double receive)
            : this(send, wait, receive, -1, -1)
        {
        }

        public Timings(double send, double wait, double receive, double blocked, double connect)
        {
            if (send < 0) throw new ArgumentException($"send time cannot be lower than zero - actual value: {send}", nameof(send));
            if (wait < 0) throw new ArgumentException($"wait time cannot be lower than zero - actual value: {wait}", nameof(wait));
            if (receive < 0) throw new ArgumentException($"receive time cannot be lower than zero - actual value: {receive}", nameof(receive));

            if (blocked < -1) throw new ArgumentException($"blocked time can be -1 if unknown, otherwise equal or higher than zero. actual value: {blocked}", nameof(blocked));
            if (connect < -1) throw new ArgumentException($"connect time can be -1 is unknown, otherwise equal or higher than zero. actual value: {connect}", nameof(connect));

            Send = send;
            Wait = wait;
            Receive = receive;
            Blocked = blocked;
            Connect = connect;
        }
    }
}