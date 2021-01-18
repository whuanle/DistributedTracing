using Grpc.Core;
using Jaeger.ApiV2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CZGL.Tracing.Services
{
    public class TracingQuery:QueryService.QueryServiceBase
    {
        public override Task<ArchiveTraceResponse> ArchiveTrace(ArchiveTraceRequest request, ServerCallContext context)
        {
            return base.ArchiveTrace(request, context);
        }
        public override Task FindTraces(FindTracesRequest request, IServerStreamWriter<SpansResponseChunk> responseStream, ServerCallContext context)
        {
            return base.FindTraces(request, responseStream, context);
        }

        public override Task<GetDependenciesResponse> GetDependencies(GetDependenciesRequest request, ServerCallContext context)
        {
            return base.GetDependencies(request, context);
        }

        public override Task<GetOperationsResponse> GetOperations(GetOperationsRequest request, ServerCallContext context)
        {
            return base.GetOperations(request, context);
        }
        public override Task<GetServicesResponse> GetServices(GetServicesRequest request, ServerCallContext context)
        {
            return base.GetServices(request, context);
        }

        public override Task GetTrace(GetTraceRequest request, IServerStreamWriter<SpansResponseChunk> responseStream, ServerCallContext context)
        {
            return base.GetTrace(request, responseStream, context);
        }
    }
}
