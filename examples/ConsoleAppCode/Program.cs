
/*
 * 这个项目只是为了试代码方便，以及测试代码、复制代码
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Microsoft.Diagnostics.Runtime.ICorDebug;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppCode
{
    public static class Test
    {
        // It's of no use
        //public static TB ToType<TA, TB>(this TA a, IMapper mapper, Expression<Func<TA, TB>> func)
        //{
        //    //Func<TA, TB> f1 = mapper.MapExpression<Expression<Func<TA, TB>>>(func).Compile();
        //    //TB result = f1(a);

        //    return mapper.MapExpression<Expression<Func<TA, TB>>>(func).Compile()(a);
        //}

        public static IEnumerable<TB> ToType<TA, TB>(this IEnumerable<TA> list, IMapper mapper, Expression<Func<TA, TB>> func)
        {
            var one = mapper.MapExpression<Expression<Func<TA, TB>>>(func).Compile();
            List<TB> bList = new List<TB>();
            foreach (var item in list)
            {
                bList.Add(one(item));
            }
            return bList;
        }
    }

    public class TestA
    {
        public int A { get; set; }
        public string B { get; set; }
        public int C { get; set; }
        public string D { get; set; }
        public int E { get; set; }
        public string F { get; set; }
        public int G { get; set; }
        public string H { get; set; }
    }

    public class TestB
    {
        public int A { get; set; }
        public string B { get; set; }
        public int C { get; set; }
        public string D { get; set; }
        public int E { get; set; }
        public string F { get; set; }
        public int G { get; set; }
        public string H { get; set; }
    }

    public class DataDBContext : DbContext
    {
        public DbSet<TestA> TestA { get; set; }
    }

    class Program
    {
        private static readonly MapperConfiguration configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddExpressionMapping();
            cfg.CreateMap<TestA, TestB>();
        });
        private static readonly IMapper mapper = configuration.CreateMapper();

        private static DataDBContext _context = new DataDBContext();
        static async Task Main(string[] args)
        {
            List<TestA> data = new List<TestA>();
            data.Add(mapper.Map<TestA>(_a));
            data.Add(mapper.Map<TestA>(_a));
            data.Add(mapper.Map<TestA>(_a));

            _ = _context.TestA.ToArray().ToType(mapper, item => mapper.Map<TestB>(item));

            Console.Read();
        }


        private static readonly TestA _a = new TestA
        {
            A = 1,
            B = "aaa",
            C = 1,
            D = "aaa",
            E = 1,
            F = "aaa",
            G = 1,
            H = "aaa",
        };
    }
}
