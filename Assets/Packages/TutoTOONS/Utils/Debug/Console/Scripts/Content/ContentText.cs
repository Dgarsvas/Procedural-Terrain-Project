using UnityEngine;
using UnityEngine.UI;

namespace TutoTOONS.Utils.Debug.Console
{
    public class ContentText : MonoBehaviour
    {
        public Transform transform_content_list;
        public GameObject prefab_log_entry;
        public Color[] log_colors;
        public int rows_limit = 50;

        public void AddLog(string text, int color = 0, int filter = -1)
        {
            if(transform_content_list.childCount > 0 && transform_content_list.childCount > rows_limit)
            {
                DestroyImmediate(transform_content_list.GetChild(0).gameObject);
            }

            var go = Instantiate(prefab_log_entry, transform_content_list);
            var txt = go.GetComponent<Text>();
            txt.text = text;
            txt.color = log_colors[color];
            if(filter >= 0)
            {
                go.AddComponent<SDKDebuggingLogEntry>().Init(filter);
            }
        }
    }
}
