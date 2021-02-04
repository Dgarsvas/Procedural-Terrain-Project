using UnityEngine;

namespace TutoTOONS.Utils.Debug.Console
{
    public class ActionButton : MonoBehaviour
    {
        public static int actions_couted {get; private set;}
        private RectTransform rect_transform;

        void Start()
        {
            actions_couted = 0;
            rect_transform = transform.GetComponent<RectTransform>();
        }

        public static void Reset()
        {
            actions_couted = 0;
        }

        void Update()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                double _x_min = transform.position.x;
                double _x_max = transform.position.x + rect_transform.sizeDelta.x;

                double _y_min = transform.position.y - rect_transform.sizeDelta.y;
                double _y_max = transform.position.y;

                if (UnityEngine.Input.mousePosition.x > _x_min && UnityEngine.Input.mousePosition.x < _x_max && UnityEngine.Input.mousePosition.y > _y_min && UnityEngine.Input.mousePosition.y < _y_max)
                {
                    actions_couted++;
                }
                else
                {
                    actions_couted = 0;
                }
            }
        }

    }
}
