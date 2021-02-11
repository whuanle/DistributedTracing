using CZGL.Tracing.Models;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Jaeger;
using Jaeger.ApiV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Extensions
{
    public static class SpanReferenceExtensions
    {
        public static SpanReference ToSpanReference(this SpanRef spanRef)
        {
            return new SpanReference
            {
                TraceID = TracingUtil.GetTraceId(spanRef.TraceId),
                SpanID = TracingUtil.GetSpanId(spanRef.SpanId),
                RefType = spanRef.RefType == SpanRefType.ChildOf ? "CHILD_OF" : "Follows_From"
            };
        }

        public static RepeatedField<SpanRef> ToSpanTref(this IEnumerable<SpanReference> spans)
        {
            RepeatedField<SpanRef> refs = new RepeatedField<SpanRef>();
            foreach (var item in spans)
            {
                refs.Add(item.ToSpanRef());
            }
            return refs;
        }


        public static SpanRef ToSpanRef(this SpanReference span)
        {
            return new SpanRef
            {
                TraceId = ByteString.CopyFrom(TraceId.FromString(span.TraceID).ToByteArray()),
                SpanId = ByteString.CopyFrom(SpanId.FromString(span.SpanID).ToByteArray()),
                RefType = span.RefType == "CHILD_OF" ? SpanRefType.ChildOf : SpanRefType.FollowsFrom
            };
        }

    }
}
