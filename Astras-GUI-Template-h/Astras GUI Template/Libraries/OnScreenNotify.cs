using System.Collections;
using UnityEngine;

namespace Astras_GUI_Template.Libraries;

public class OnScreenNotify : MonoBehaviour
{
        public static OnScreenNotify? Instance { get; private set; }

        private List<string> notification = new();

        private static string CNotify = "";

        private GUIStyle? StyleHHHHHH;

        private void Start()
        {
            StyleHHHHHH = new GUIStyle();
            StyleHHHHHH.alignment = TextAnchor.MiddleCenter;
            StyleHHHHHH.fontSize = 35;
            StyleHHHHHH.richText = true;
            StyleHHHHHH.normal.textColor = Color.white;
        }

        public static void SendIT(string msg, float det = 2f)
        {
            if (Instance == null) return;

            Instance.SendNotifyScreen(msg, det);
        }

        public void SendNotifyScreen(string msg, float det)
        {
            notification.Add(msg);
            UpdateNotify();
            StartCoroutine(DisplayNotify(det, msg));
        }

        private IEnumerator DisplayNotify(float der, string msgr)
        {
            yield return new WaitForSeconds(der);
            notification.Remove(msgr);
            UpdateNotify();
        }

        private void UpdateNotify()
        {
            CNotify = "";

            foreach (var item in notification)
            {
                CNotify += item + "\n";
            }
        }
        private void Awake() => Instance = this;
        private void OnGUI()
        {
            if (string.IsNullOrEmpty(CNotify)) return;

            int lines = CNotify.Split('\n').Length;

            GUI.Label(
                new Rect(
                    new Vector2(
                        Screen.width / 4f,
                        Screen.height - 55 * (lines + 1)
                    ),
                    new Vector2(
                        Screen.width / 2f,
                        100f
                    )
                ),
                CNotify,
                StyleHHHHHH
            );
        }

    }
