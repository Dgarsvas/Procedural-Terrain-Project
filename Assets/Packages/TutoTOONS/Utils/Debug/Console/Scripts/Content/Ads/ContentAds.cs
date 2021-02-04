using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TutoTOONS;

namespace TutoTOONS.Utils.Debug.Console
{
    public class ContentAds : MonoBehaviour
    {
        public GameObject ad_service_data_prefab;
        public GameObject loading_app_config_prefab;
        public GameObject scroll_panel;

        private List<GameObject> ad_service_data = new List<GameObject>();
        private GameObject loading_app_config;
        private bool waiting_for_app_config = true;

        void Start()
        {
            loading_app_config = Instantiate(loading_app_config_prefab, scroll_panel.transform);
        }

        private void GenerateAdServicesData()
        {
            if (ad_service_data_prefab != null)
            {
                for (int i = 0; i < AdServices.unique_ads.Count; i++)
                {
                    GameObject _slot = Instantiate(ad_service_data_prefab, scroll_panel.transform);
                    ad_service_data.Add(_slot);
                    _slot.GetComponent<AdServiceData>().Init(AdServices.unique_ads[i]);
                }

                if (ad_service_data.Count == 0)
                {
                    return;
                }
            }
        }

        void Update()
        {
            if (waiting_for_app_config)
            {
                switch (AppConfig.state)
                {
                    case AppConfig.STATE_LOADED:
                        if (AdServices.state == AdServices.STATE_ENABLED)
                        {
                            Destroy(loading_app_config);
                            GenerateAdServicesData();
                            waiting_for_app_config = false;
                        }
                        else
                        {
                            if (AdServices.state == AdServices.STATE_DISABLED)
                            {
                                loading_app_config.GetComponent<Text>().text = "Ad services are disabled.";
                            }
                            else
                            {
                                loading_app_config.GetComponent<Text>().text = "Loading ad services";
                            }
                        }
                        break;

                    case AppConfig.STATE_LOADING:
                        loading_app_config.GetComponent<Text>().text = "Loading AppConfig...";
                        break;

                    case AppConfig.STATE_FAILED:
                        loading_app_config.GetComponent<Text>().text = "Failed to load AppConfig.";
                        break;

                    case AppConfig.STATE_DISABLED:
                        loading_app_config.GetComponent<Text>().text = "AppConfig is disabled.";

                        #if GEN_FREETIME
                        loading_app_config.GetComponent<Text>().text = "Freetime games doesn't have ads and IAPs.";
                        #endif

                        break;



                }
            }
        }
    }
}
