
using Grpc.Core;
using Jaeger.ApiV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CZGL.DT.Services
{
    public class TestClass:CollectorService.CollectorServiceBase
    {
        public override async Task<PostSpansResponse> PostSpans(PostSpansRequest request, ServerCallContext context)
        {
            Console.WriteLine("暂时不处理内容，所以报错");
            return new Jaeger.ApiV2.PostSpansResponse
            {

            };
        }
    }
}
