using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Buffers.Binary;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CZGL.Tracing
{
    /// <summary>
    /// Trace 实用类
    /// </summary>
    public static class TracingUtil
    {
        #region TraceId、SpanId 解析与生成

        /// <summary>
        /// 从 <see cref="ByteString"/> 中生成 traceId
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        public static string GetTraceId(ByteString byteString) => GetTraceId(byteString.ToArray());

        public static long GetTraceLongId(ByteString byteString) => GetTraceIdLong(byteString.ToArray().AsSpan()).Low;

        /// <summary>
        /// 从 <see cref="byte"/>[] 中生成 traceId
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetTraceId(byte[] bytes)
        {
            if (bytes.Length != 16)
                throw new FormatException("Cannot convert to TraceId!");

            var traceId = GetTraceIdLong(bytes.AsSpan());
            // 目前 traceid 已经跟 span 相同高位部分全为0，只需要取低位部分即可
            return traceId.Low.ToString("x016");
        }

        /// <summary>
        /// 从 <see cref="ByteString"/> 中生成 spanId
        /// </summary>
        /// <param name="byteString"></param>
        /// <returns></returns>
        public static string GetSpanId(ByteString byteString) => GetSpanId(byteString.ToArray());


        /// <summary>
        /// 从 <see cref="byte"/>[] 中生成 spanId
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetSpanId(byte[] bytes)
        {
            if (bytes.Length != 8)
                throw new FormatException("Cannot convert to SpanId!");
            long value = BytesToLong(bytes.AsSpan());
            return value.ToString("x016");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static (long High, long Low) GetTraceIdLong(Span<byte> bytes)
        {
            long high = BytesToLong(bytes[0..8]);
            long low = BytesToLong(bytes[8..16]);
            return (high, low);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long BytesToLong(Span<byte> bytes)
        {
            // Jaeger 高位在前，低位在后
            if (BitConverter.IsLittleEndian)
            {
                return BinaryPrimitives.ReadInt64BigEndian(bytes);
            }
            return BinaryPrimitives.ReadInt64LittleEndian(bytes);
        }


        #endregion

        #region 时间戳转换

        public static long GetLongTime(this Google.Protobuf.WellKnownTypes.Timestamp timestamp)
        {
#warning TODO:后面试试ToDateTimeOffset
            return GetTraceTimetamp(timestamp.ToDateTime());
        }

        /// <summary>
        /// 将转为 Jaeger 中合适的微秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetTraceTimetamp(this DateTime dateTime)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (dateTime.Ticks - dt1970.Ticks)/10;
        }

        /// <summary>
        /// 将其 Trace 中 long 时间 转为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static Timestamp GetTraceTimestamp(long time)
        {
            DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var ticks = time * 10;
            var newTime = dt1970.AddTicks(ticks);
            return Timestamp.FromDateTime(newTime);
        }

        #endregion
    }
}
