using Google.Protobuf.WellKnownTypes;
using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Device.Grpc;

public class ServerExceptionInterceptor : Interceptor
{
    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncClientStreamingCall(context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncDuplexStreamingCall(context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncServerStreamingCall(request, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncUnaryCall(request, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.BlockingUnaryCall(request, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.ClientStreamingServerHandler(requestStream, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }


    public override Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.ServerStreamingServerHandler(request, responseStream, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }


    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return base.UnaryServerHandler(request, context, continuation);
        }
        catch (System.Exception ex) { throw Exceptor(ex); }
    }

    private Exception Exceptor(Exception ex)
    {
        var status = new Google.Rpc.Status
        {
            Code = (int)Code.Aborted,
            Message = "An error occurred",
            Details =
            {
                Any.Pack( new ErrorInfo
                        {
                            Domain = ex.GetType().FullName,
                            Reason = ex.Message
                        }
                )
            }

        };

        return status.ToRpcException();
    }
}