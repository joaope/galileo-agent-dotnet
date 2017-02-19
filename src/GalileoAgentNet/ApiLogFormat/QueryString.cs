using System.Collections.Generic;

namespace GalileoAgentNet.ApiLogFormat
{
    public class QueryString : List<QueryStringNameValuePair>
    {
        public QueryString()
        {
        }

        public QueryString(IEnumerable<QueryStringNameValuePair> collection) 
            : base(collection)
        {
        }

        public QueryString(int capacity) 
            : base(capacity)
        {
        }
    }
}