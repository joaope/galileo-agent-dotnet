using System;

namespace GalileoAgentNet.ApiLogFormat
{
    public sealed class Entry
    {
        public double Time { get; }

        public DateTime StartedDateTime { get; }

        public string ClientIpAddress { get; }

        public string ServerIpAddress { get; }

        public Request Request { get; }

        public Response Response { get; }

        public Timings Timings { get; }

        public Entry(
            DateTime startedDateTime,
            string clientIpAddress,
            string serverIpAddress,
            Request request,
            Response response,
            Timings timings)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (timings == null) throw new ArgumentNullException(nameof(timings));

            StartedDateTime = startedDateTime;
            ClientIpAddress = clientIpAddress;
            ServerIpAddress = serverIpAddress;
            Request = request;
            Response = response;
            Timings = timings;

            Time = (timings.Send == -1 ? 0 : timings.Send) +
                   (timings.Blocked == -1 ? 0 : timings.Blocked) +
                   (timings.Connect == -1 ? 0 : timings.Connect) +
                   (timings.Receive == -1 ? 0 : timings.Receive) +
                   (timings.Wait == -1 ? 0 : timings.Wait);
        }
    }
}