using UnityEngine;
using UnityEngine.UI;

namespace TutoTOONS.Utils.Debug.Console
{
    public class LogEntry : MonoBehaviour
    {
        public UnityEngine.UI.Button button;
        public Text text;

        private string full_text;
        private string short_text;
        private bool show_full;
        private bool update_size = true;

        void Start()
        {
            full_text = text.text;
            short_text = full_text;

            var split = text.text.Split('\n');
            if (split.Length > 0)
            {
                short_text = split[0];
            }

            text.text = short_text;

            button.onClick.AddListener(delegate
            {
                show_full = !show_full;
                text.text = show_full ? full_text : short_text;
                update_size = true;
            });
        }

        void Update()
        {
            if (update_size)
            {
                update_size = false;
                text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight + 8f);
            }
        }
    }
}
