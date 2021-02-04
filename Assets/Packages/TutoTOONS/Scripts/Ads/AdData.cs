using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TutoTOONS
{
    public class AdData
    {
        public const string TYPE_INTERSTITIAL = "interstitial";
        public const string TYPE_INTERSTITIAL_VIDEO = "interstitial_video";
        public const string TYPE_STATIC = "static";
        public const string TYPE_VIDEO = "video";
        public const string TYPE_BANNER = "banner";
        public const string TYPE_MORE_APPS = "more_apps";
        public const string TYPE_PANEL = "panel";
        public const string TYPE_PLAYABLE = "playable";

        public const int STATE_EMPTY = 0;
        public const int STATE_LOADING = 1;
        public const int STATE_LOADED = 2;
        public int state;
        public int ad_network_id;
        public AdLocation ad_location;
        public string key;
        public string key_no_comp;
        public string type;
        public double preload_timeout;
        public AdService ad_service;

        public AdData()
        {
            state = STATE_EMPTY;
            preload_timeout = 0.0;
        }

        public void SetStartedLoading()
        {
            state = STATE_LOADING;
            preload_timeout = 10.0;
        }

        public void SetLoaded()
        {
            state = STATE_LOADED;
        }

        public void SetEmpty()
        {
            state = STATE_EMPTY;
            preload_timeout = 0.0;
        }

        public void SetFailedToLoad()
        {
            state = STATE_EMPTY;
        }
    }
}