using Flow.Core.Areas.Returns;

namespace Flow.Core.Demos.AppClient.Common.Utilities;

public static class NetworkingUtility
{
    public static Flow<bool> HasInternetConnection(bool alwaysOn = false)

        => alwaysOn == true ? true :
                              (Random.Shared.Next() % 2 == 0) ? true : new Failure.InternetConnectionFailure("No internet connection - randomly set!",null,0,true);
}
