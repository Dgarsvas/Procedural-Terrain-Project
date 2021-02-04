using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TutoTOONS.Utils.Debug.Console
{
    public enum ConsoleMenu
    {
        None,
        ContentAds,
        ContentVersions,
        ContentLogs,
        ContentStats,
        ContentSDKDebugging,
        ContentSDKDebuggingLogs
    }

    public class Console : MonoBehaviour
    {
        public ContentAds content_ads;
        public ContentText content_versions;
        public ContentText content_logs;
        public ContentStats content_stats;
        public ContentSDKDebugging content_sdk_debugging;
        public ContentText content_sdk_debugging_logs;
        public Image[] imgNavigationButtons;
        public Text[] txtNavigationButtons;
        public Text txtLogNavigationBtn;
        bool versionsLogged;
        bool logsActive = true;

        void Start()
        {
            ShowNone();
        }

        void Update()
        {
            if (!versionsLogged)
            {
                if (AdServices.ad_services_initialized)
                {
                    LogVersions();
                    versionsLogged = true;
                }
            }
        }

        void LogVersions()
        {
            var _versions = AdServices.GetVersions();
            var _len = _versions.Count;
            for (int j = 0; j < _len; j++)
            {
                LogVersion(_versions[j]);
            }

            LogVersion("TutoTOONS package: " + AppConfig.PACKAGE_VERSION);
        }

        public void LogVersion(string text)
        {
            content_versions.AddLog(text);
        }

        public void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (!logsActive) return;

            string _text = logString + "\n" + stackTrace;
            content_logs.AddLog(_text, (int)type);
        }

        public void SwitchMenu(ConsoleMenu menu)
        {
            content_ads.gameObject.SetActive(menu == ConsoleMenu.ContentAds);
            content_versions.gameObject.SetActive(menu == ConsoleMenu.ContentVersions);
            content_logs.gameObject.SetActive(menu == ConsoleMenu.ContentLogs);
            content_stats.gameObject.SetActive(menu == ConsoleMenu.ContentStats);
            content_sdk_debugging.gameObject.SetActive(menu == ConsoleMenu.ContentSDKDebugging);
            content_sdk_debugging_logs.gameObject.SetActive(menu == ConsoleMenu.ContentSDKDebuggingLogs);
        }

        public void ShowNone()
        {
            SwitchMenu(ConsoleMenu.None);

            for(int i = 0; i < imgNavigationButtons.Length; i++)
            {
                imgNavigationButtons[i].enabled = !imgNavigationButtons[i].enabled;
            }

            for (int i = 0; i < txtNavigationButtons.Length; i++)
            {
                txtNavigationButtons[i].enabled = !txtNavigationButtons[i].enabled;
            }
        }

        public void ShowAdsContent()
        {
            SwitchMenu(ConsoleMenu.ContentAds);
        }

        public void ShowStatsContent()
        {
            SwitchMenu(ConsoleMenu.ContentStats);
        }

        public void ShowVersionsContent()
        {
            SwitchMenu(ConsoleMenu.ContentVersions);
        }

        public void ShowLogsContent()
        {
            if (content_logs.gameObject.activeInHierarchy)
            {
                logsActive = !logsActive;
                txtLogNavigationBtn.text = "Logs\n(" + (logsActive ? "active" : "paused") + ")";
            }
            SwitchMenu(ConsoleMenu.ContentLogs);
        }

        public void ShowSDKDebuggingContent()
        {
            SwitchMenu(ConsoleMenu.ContentSDKDebugging);
        }

        public void ShowSDKDebuggingLogsContent()
        {
            SwitchMenu(ConsoleMenu.ContentSDKDebuggingLogs);
        }
    }
}
