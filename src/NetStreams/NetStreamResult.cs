using System;
using OneOf;

namespace NetStreams
{
    public class NetStreamResult<TOutType> : OneOfBase<TOutType, Exception, StreamCancellation>
    {
        public NetStreamResult(OneOf<TOutType, Exception, StreamCancellation> _) : base(_) { }
    }
}