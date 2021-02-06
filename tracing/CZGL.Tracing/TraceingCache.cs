using CZGL.Tracing.Models;
using Microsoft.Extensions.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CZGL.Tracing
{
    /// <summary>
    /// Collector Trace 专用缓存
    /// </summary>
    public class TraceingCache
    {

        private static readonly ConcurrentDictionary<long, EntryCache<TracingObject>> _entries = new ConcurrentDictionary<long, EntryCache<TracingObject>>();
        private static TimeSpan timeSpan = TimeSpan.FromSeconds(30);
        private static TimeSpan timeSpanSleep = TimeSpan.FromSeconds(31);

        private static Task task;
        private static object objLock = new object();


        private ITracingCacheDue _cacheDue;

        public TraceingCache(ITracingCacheDue cacheDue)
        {
            _cacheDue = cacheDue;
        }


        public bool TryGetValue(long traceId,out TracingObject tracingObject)
        {
            if(_entries.TryGetValue(traceId,out var obj))
            {
                tracingObject = obj.Data;
                return true;
            }
            tracingObject = default;
            return false;
        }

        /// <summary>
        /// 不存在相同键时，添加成功；存在相同键时，添加失败，out 获得已存在相同键的对象
        /// <para>支持并发</para>
        /// </summary>
        /// <param name="tracingObject"></param>
        /// <returns></returns>
        public bool TryAddOrGet(TracingObject tracingObject, out TracingObject oldTracingObject)
        {
            oldTracingObject = default;

            var obj = _entries.GetOrAdd(tracingObject.TraceId, new EntryCache<TracingObject>
            {
                DateTime = DateTime.Now,
                Data = tracingObject
            });

            // 新添加的
            if (obj.Data.TraceId == tracingObject.TraceId)
            {
                lock (objLock)
                {
                    if (task == null)
                    {
                        task = Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(timeSpanSleep);
                            Check();
                            task = null;
                        });
                        task = Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(timeSpanSleep * 2);
                            Check();
                            task = null;
                        });
                    }
                }
                return true;
            }

            oldTracingObject = obj.Data;
            return false;
        }

        private void Check()
        {
            DateTime dateTime = DateTime.Now;
            List<long> removes = new List<long>();
            List<TracingObject> objs = new List<TracingObject>();
            foreach (var item in _entries)
            {
                if ((dateTime - item.Value.DateTime) > timeSpan)
                {
                    removes.Add(item.Key);
                    objs.Add(item.Value.Data);
                }
            }
            _ = Task.WhenAny(objs.Select(x => _cacheDue.InvokeAsync(x)));
            foreach (var item in removes)
            {
                _ = _entries.Remove(item, out _);
            }
        }

        private class EntryCache<T>
        {
            public DateTime DateTime { get; set; }
            public T Data { get; set; }
        }
    }
}
