using Flow.Core.Areas.Returns;

namespace Flow.Core.Demos.AppServer.Common.Seeds;

public interface IDbExceptionHandler
{
    public Flow<T> Handle<T>(Exception ex);
}
