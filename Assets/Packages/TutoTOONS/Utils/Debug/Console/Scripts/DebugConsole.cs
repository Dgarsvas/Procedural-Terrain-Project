using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Security.Cryptography;

namespace TutoTOONS.Utils.Debug.Console
{
    public class DebugConsole : MonoBehaviour
    {
        public static DebugConsole instance;
        private const string KEY_SAVED_STATE = "debug_console_enabled";
        private const int CLICK_THRESHOLD = 7;
        private const int INPUT_ACTIVATION_LIMIT = 15;
        private const string KEY = "1af180faed9a8717cf05d3fb75d734e11a563b70fd694001bd02513f76a4ff16117488bdae49a527a3173b803693b82d74cb96e7d5b91f8a5434a31d1c4b4cd4";

        public Input input;
        public Console console;
        public InputField input_field;
        public GameObject consoleContainer;
        public Text textLastAdInfo;

        private bool console_active;
        private bool input_active;
        private double input_timeout;

        void OnEnable()
        {
            Application.logMessageReceived += console.HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= console.HandleLog;
        }
        public void UpdateLastAdInfo(string _ad_network, string _ad_type, bool _reward_given)
        {
            var txt = "Last completed ad info:" + "\n";
            txt += "Ad network: " + _ad_network + "\n";
            txt += "Ad type: " + _ad_type + "\n";
            txt += "Reward given: " + _reward_given + "\n";
            textLastAdInfo.text = txt;
        }

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            input_field.onEndEdit.AddListener(onInputFieldChange);

            console_active = false;
            input_active = false;
            input_timeout = 0.0;
            console.gameObject.SetActive(false);
            input.SetActive(false);

#if !UNITY_EDITOR
            if (SavedData.GetInt(KEY_SAVED_STATE) > 0)
            {
                Show();
                ShowUnlockField();
            }
#endif
        }

        private void TryShow()
        {
            if (input_active)
            {
                return;
            }

            if(ActionButton.actions_couted >= CLICK_THRESHOLD)
            {
                ActionButton.Reset();
                ShowUnlockField();
                input_active = true;
                input_timeout = INPUT_ACTIVATION_LIMIT;
            }
        }

        private void Show()
        {
            if (console_active)
            {
                return;
            }
            consoleContainer.SetActive(true);
            console_active = true;
            console.gameObject.SetActive(true);
            SavedData.SetInt(KEY_SAVED_STATE, 1);
        }

        private void Close()
        {
            ActionButton.Reset();
            input_active = false;
            console_active = false;
            consoleContainer.SetActive(false);
            console.gameObject.SetActive(false);
            input.SetActive(false);
            SavedData.SetInt(KEY_SAVED_STATE, 0);
        }

        private void ShowUnlockField()
        {
            if(input_active)
            {
                return;
            }
            
            input.SetActive(true);
        }

        private void onInputFieldChange(string _entered_text)
        {
            input_field.GetComponent<InputField>().text = "";

            if (!console_active)
            {
                if (computeHash(_entered_text) == KEY)
                {
                    Show();
                }
                else
                {
                    Close();
                }
                return;
            }

            // Process commands.
            switch (_entered_text)
            {
                case "close":
                    Close();
                    break;

                default:

                    break;
            }
        }

        private string computeHash(string _data)
        {
            using (SHA512 _sha_512 = SHA512.Create())
            {
                StringBuilder _builder = new StringBuilder();
                byte[] _bytes = _sha_512.ComputeHash(Encoding.UTF8.GetBytes(_data));

                for (int i = 0; i < _bytes.Length; i++)
                {
                    _builder.Append(_bytes[i].ToString("x2"));
                }
                return _builder.ToString();
            }
        }

        void Update()
        {
            TryShow();

            if(input_active && !console_active)
            {
                input_timeout -= Time.deltaTime;

                if (input_timeout <= 0)
                {
                    Close();
                }
            }
        }

    }
}