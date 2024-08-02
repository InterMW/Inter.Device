using Google.Rpc;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Device.GrpcClient;

public class ClientExceptionInterceptor : Interceptor
{
    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncClientStreamingCall(context, continuation);
        } catch (RpcException ex) {throw Exceptor(ex);}
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(ClientInterceptorContext<TRequest, TResponse> context, AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
        return base.AsyncDuplexStreamingCall(context, continuation);
        } catch (RpcException ex) {throw Exceptor(ex);}
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
            return base.AsyncUnaryCall(request, context, continuation);
        } catch (RpcException ex) {throw Exceptor(ex);}
    }

    public override TResponse BlockingUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, BlockingUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        try
        {
        return base.BlockingUnaryCall(request, context, continuation);
        } catch (RpcException ex) {throw Exceptor(ex);}
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, ServerCallContext context, ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
        return await base.ClientStreamingServerHandler(requestStream, context, continuation);
        } catch (RpcException ex) {throw Exceptor(ex);}
    }

    private Exception Exceptor(RpcException ex)
    {
        var richException = ex.GetRpcStatus()?.GetDetail<ErrorInfo>();

        if(richException != null)
        {
            Type type = Type.GetType(richException.Domain);

            if(type != null)
            {
                return (Exception)Activator.CreateInstance(type, richException.Reason) ?? new Exception(ex.Message);
            }
        }
        return new Exception("Undefined error occurred.");
    }
}
