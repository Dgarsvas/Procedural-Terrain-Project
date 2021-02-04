﻿using System;
using UnityEngine;
using UnityEngine.UI;

namespace TutoTOONS.Utils.Debug.Console
{
    public class AdServiceData : MonoBehaviour
    {
        private GameObject button_obj;
        private GameObject title_obj;

        private AdData ad_data;
        private Image button_image;
        private Text button_text;

        void Start()
        {

        }

        public void Init(AdData _ad_data)
        {
            ad_data = _ad_data;
            button_obj = transform.Find("button_obj").gameObject;
            title_obj = transform.Find("title_obj").gameObject;
            title_obj.GetComponent<Text>().text = ad_data.ad_service.title + " (" + ad_data.type + ") key: " + ad_data.key;
            button_image = button_obj.GetComponent<Image>();
            button_text = button_obj.GetComponentInChildren<Text>();
        }

        public void ShowAd()
        {
            if (ad_data != null)
            {
                if (ad_data.state == AdData.STATE_LOADED)
                {
                    if(ad_data.ad_service.ShowAd(ad_data))
                    {
                        AdServices.active_service = ad_data.ad_service;
                        AdServices.active_service.location_showing = ad_data.ad_location;// GenerateDebugAdLocation(ad_data);
                    }
                }
            }
        }

        private AdLocation GenerateDebugAdLocation(AdData _ad)
        {
            AdLocation _ad_location = new AdLocation();
            _ad_location.keyword = new AdLocationKeyword("debug_console");
            _ad_location.location_id = 0;
            _ad_location.campaign_id = 0;
            _ad_location.ad_id = 0;
            _ad_location.fill_rate = 0;
            _ad_location.priority = 0;
            _ad_location.ad = _ad;
            return _ad_location;
        }
        
        void Update()
        {
            if (ad_data != null && button_text != null && button_image != null)
            {
                switch (ad_data.state)
                {
                    case AdData.STATE_EMPTY:
                        button_text.text = "Empty";
                        button_image.color = Color.white;
                        break;

                    case AdData.STATE_LOADED:
                        button_text.text = "Loaded";
                        button_image.color = Color.green;
                        break;

                    case AdData.STATE_LOADING:
                        button_text.text = "Loading";
                        button_image.color = Color.yellow;
                        break;
                }
            }
        }

    }
}
