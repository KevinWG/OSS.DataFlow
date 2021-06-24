using System;
using System.Collections.Generic;
using System.Text;

namespace OSS.DataFlow.Inter.Queue
{
    public class InterSubscriberMultiHandler
    {

        private static Dictionary<string, ISubscriberWrap> subs = new Dictionary<string, ISubscriberWrap>();
    }
}
