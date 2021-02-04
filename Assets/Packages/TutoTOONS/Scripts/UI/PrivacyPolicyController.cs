using UnityEngine;
using UnityEngine.SceneManagement;

namespace TutoTOONS
{
    public class PrivacyPolicyController
    {
        public GameObject pop_up;



        public static void Show()
        {
            InAppBrowser.DisplayOptions _display_options = new InAppBrowser.DisplayOptions();
            _display_options.insets = GetWebViewPadding();
            _display_options.hidesHistoryButtons = true;
            
            ///TODOif (Application.internetReachability != NetworkReachability.NotReachable)
            if(true)
            {
                // Device is conected to the Internet
                InAppBrowser.OpenURL(GetPrivacyPolicyURL(), _display_options);
            }
            else
            {
                // Device is not conected to the Internet
                GameObject _pop_up_no_internet = Resources.Load("Prefabs/PrivacyPolicyPopup") as GameObject;
                Object.Instantiate(_pop_up_no_internet);
                GameObject tutotoons = GameObject.Find("TutoTOONS");
                _pop_up_no_internet.transform.parent = tutotoons.transform.parent;
            }
        }

        private static string GetPrivacyPolicyURL()
        {
            string _privacy_policy_url = null;

            if(AppConfig.settings != null)
            {
                _privacy_policy_url = AppConfig.settings.privacy_policy_link;
            }
            
            if (string.IsNullOrEmpty(_privacy_policy_url))
            {
                if (AppConfig.account == AppConfig.ACCOUNT_TUTOTOONS)
                {
                    _privacy_policy_url = "https://tutotoons.com/privacy_policy/embeded";
                }
                else if (AppConfig.account == AppConfig.ACCOUNT_CUTE_AND_TINY)
                {
                    _privacy_policy_url = "https://cutetinygames.com/privacy_policy/embedded";
                }
                else if (AppConfig.account == AppConfig.ACCOUNT_SPINMASTER)
                {
                    _privacy_policy_url = "https://spinmaster.helpshift.com/a/mightyexpress/?p=web&s=privacy-policy&f=spin-master-privacy-policy&l=en";
                }
                else
                {
                    // TODO: Figure out which privacy policy should be shown here
                    _privacy_policy_url = "https://tutotoons.com/privacy_policy/embeded";
                }
            }
            return _privacy_policy_url;
        }
        
        private static InAppBrowser.EdgeInsets GetWebViewPadding()
        {
            int _padding_top = 0;
            int _padding_bottom = 0;
            int _padding_left = 0;
            int _padding_right = 0;

            return new InAppBrowser.EdgeInsets(_padding_top, _padding_bottom, _padding_left, _padding_right);
        }
    }
}
