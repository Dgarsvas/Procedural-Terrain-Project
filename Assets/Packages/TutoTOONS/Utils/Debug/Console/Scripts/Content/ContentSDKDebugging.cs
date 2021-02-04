using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

namespace TutoTOONS.Utils.Debug.Console
{
    public class ContentSDKDebugging : MonoBehaviour
    {
        public static ContentSDKDebugging instance;
        public GameObject prefabEntry;
        public Transform tList;
        public Text textLoading;
        public ContentText content_sdk_debugging_logs;
        public static List<SDKLog> logQueue = new List<SDKLog>();

        void Start()
        {
            instance = this;
            GenerateEntries();
        }

        void GenerateEntries()
        {
            int _count = System.Enum.GetNames(typeof(SDKDebug.ServiceName)).Length;

            string[] classes = new string[]
            {
            "TutoTOONS.TutoToonsWrapper,Assembly-CSharp",
            "TutoTOONS.ChartboostWrapper,Assembly-CSharp",
            "TutoTOONS.SuperAwesomeWrapper,Assembly-CSharp",
            "TutoTOONS.KidozWrapper,Assembly-CSharp",
            "TutoTOONS.IronSourceWrapper,Assembly-CSharp",
            "TutoTOONS.AdMobWrapper,Assembly-CSharp",
            "TutoTOONS.StatsTracker,Assembly-CSharp"
            };

            for (int i = 0; i < _count; i++)
            {
                bool classExists = Type.GetType(classes[i]) != null;
                if (!classExists) continue;
                var entry = Instantiate(prefabEntry, tList).GetComponent<SDKDebuggingEntry>();
                entry.Init(i);
            }
        }

        void Update()
        {
            for (int i = logQueue.Count - 1; i >= 0; i--)
            {
                content_sdk_debugging_logs.AddLog(logQueue[i].message, 0, (int)logQueue[i].service_name);
                logQueue.RemoveAt(i);
            }
        }
    }
}