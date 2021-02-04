﻿#if GEN_FIREBASE_ANALYTICS
using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
#endif
using UnityEngine;

namespace TutoTOONS
{
    public class FirebaseWrapper : MonoBehaviour
    {
#if GEN_FIREBASE_ANALYTICS
        Firebase.DependencyStatus dependency_status = Firebase.DependencyStatus.UnavailableOther;
        Firebase.FirebaseApp app;
#endif
        private static bool firebase_initialized = false;

        void Start()
        {
            DontDestroyOnLoad(this);
#if GEN_FIREBASE_ANALYTICS
            Debug.Log("Initializing Firebase Analytics");
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependency_status = task.Result;
                if (dependency_status == DependencyStatus.Available)
                {
                    app = Firebase.FirebaseApp.DefaultInstance;
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependency_status);
                }
            });
#endif
        }

        public static void TrackGenericEvent(string _name)
        {
#if GEN_FIREBASE_ANALYTICS
            if (!firebase_initialized)
            {
                return;
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(_name, new Parameter[] { });
#endif
        }

        private void InitializeFirebase()
        {
            if (!firebase_initialized)
            {
                firebase_initialized = true;
                #if GEN_FIREBASE_ANALYTICS
                Firebase.Analytics.FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                Debug.Log("Firebase Analytics initialized");
                #endif
            }
            
        }

#if GEN_FIREBASE_ANALYTICS
    public static void TrackAdImpressionEvent(string _ad_network, string _ad_type, string _count)
        {
            if (!firebase_initialized)
            {
                return;
            }

            Parameter[] _parameters = new Parameter[] {
                new Parameter("ad_network", _ad_network),
                new Parameter("ad_type", _ad_type),
                new Parameter("finished_count", _count),
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_finished", _parameters);

            TrackAdImpressionMilestoneEvents(_count);
        }

        public static void TrackAdImpressionMilestoneEvents(string _count)
        {
            if (!firebase_initialized)
            {
                return;
            }

            int _impression_count = int.Parse(_count);

            if (_impression_count == 10 || _impression_count == 20 || _impression_count == 30 || _impression_count == 40 || _impression_count == 50)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_finished_" + _count, new Parameter[] { });
            }
        }

        public static void TrackAdImpressionDay(string _day)
        {
            if (!firebase_initialized)
            {
                return;
            }

            Parameter[] _parameters = new Parameter[] {
                new Parameter("day", _day)
            };

            Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression_day", _parameters);
        }

        public static void TrackAdFirst24hImpressions(int _days_since_install, string _ad_impression_count)
        {
            if (!firebase_initialized || _days_since_install > 0)
            {
                return;
            }

            int _impression_count = int.Parse(_ad_impression_count);

            if (_impression_count == 2 || _impression_count == 4 || _impression_count == 6 || _impression_count == 8 || _impression_count == 10 || _impression_count == 15 || _impression_count == 20 || _impression_count == 25)
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_finished_24h_" + _ad_impression_count, new Parameter[] { });
            }
        }
#endif

        void Update()
        {

        }
    }
}
