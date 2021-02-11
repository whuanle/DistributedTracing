using CZGL.Tracing.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CZGL.Tracing.Caches
{
    /// <summary>
    /// 并发缓存类
    /// <para>使用 tarce id 做 Key</para>
    /// </summary>
    public class ConcurrentCache
    {
        private readonly ConcurrentDictionary<long, EntryCache<TracingObject>> _entries = new ConcurrentDictionary<long, EntryCache<TracingObject>>();
        private readonly IMongoDatabase database;
        public ConcurrentCache(MongoClient mongoClient)
        {
            database = mongoClient.GetDatabase(TracingOption.Options.DataName);
            timer = new Timer(TimingCheck, null, 3100, 500);
        }

        private static readonly TimeSpan _TimeSpan = TimeSpan.FromSeconds(30);
        private readonly Timer timer;

        /// <summary>
        /// 占位注册。不存在相同的键则注册成功，存在相同的键则注册失败，并返回这个键的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public bool TryRegister(long key,out EntryCache<TracingObject> entry)
        {
            var entryNew = new EntryCache<TracingObject>(DateTime.Now)
            {
                Data = null
            };
            entry = _entries.GetOrAdd(key, entryNew);

            return entryNew == entry;
        }


        /// <summary>
        /// 检查所有数据，是否已经过期
        /// </summary>
        private void TimingCheck(object? state)
        {
            DateTime dateTime = DateTime.Now;
            List<long> removes = new List<long>();
            List<TracingObject> objs = new List<TracingObject>();
            foreach (var item in _entries)
            {
                if ((dateTime - item.Value.DateTime) > _TimeSpan)
                {
                    removes.Add(item.Key);
                    objs.Add(item.Value.Data);
                }
            }

            _ = InvokeAsync(objs);

            foreach (var item in removes)
            {
                _ = _entries.Remove(item, out _);
            }

            timer.Change(0,500);
        }

        /// <summary>
        /// 批量推送 trace 到 MongoDB
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task InvokeAsync(IEnumerable<TracingObject> data)
        {
            var collection = database.GetCollection<BsonDocument>(TracingOption.Options.DocumentName);
            await collection.InsertManyAsync(data
                .AsParallel()
                .Select(x => x.ToBsonDocument()));
        }
    }
}
