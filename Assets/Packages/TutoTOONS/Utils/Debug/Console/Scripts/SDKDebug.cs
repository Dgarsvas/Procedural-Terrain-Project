using TutoTOONS.Utils.Debug.Console;

namespace TutoTOONS.Utils.Debug
{ 
    public class SDKDebug
    {
        public enum ServiceName
        {
            TutoAds,
            Chartboost,
            SuperAwesome,
            Kidoz,
            IronSourceMediation,
            AdMobMediation,
            StatsTracker
        }

        public static void Log(ServiceName _service_name, string _message)
        {
            ContentSDKDebugging.logQueue.Add(new SDKLog(_service_name, _message));
        }
    }
}