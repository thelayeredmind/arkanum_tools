// SPDX-License-Identifier: MIT
// Shared world-space label that any quest tool can push a transient message to.
// Single canvas instance shared across all tools so feedback never stacks.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArkanumQuestTools
{
    public class QuestToolFeedbackDisplay : MonoBehaviour
    {
        static QuestToolFeedbackDisplay s_Instance;

        public static QuestToolFeedbackDisplay Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var go = new GameObject("QuestToolFeedbackDisplay");
                    DontDestroyOnLoad(go);
                    s_Instance = go.AddComponent<QuestToolFeedbackDisplay>();
                }
                return s_Instance;
            }
        }

        [Tooltip("Distance in metres in front of the camera.")]
        public float distance = 1.0f;

        [Tooltip("Offset from screen-centre in world units (x = right, y = up).")]
        public Vector2 offset = new Vector2(-0.4f, 0.3f);

        [Tooltip("World-space width of the label panel.")]
        public float panelWidth = 0.35f;

        [Tooltip("Font size in points (world-space TMP).")]
        public float fontSize = 0.03f;

        [Tooltip("Seconds the label stays visible after a message is shown.")]
        public float displayDuration = 2f;

        Camera m_Camera;
        Canvas m_Canvas;
        RectTransform m_CanvasRT;
        TextMeshProUGUI m_Text;
        float m_HideAt = -1f;

        void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            s_Instance = this;
        }

        void Start()
        {
            m_Camera = Camera.main;
            BuildLabel();
            m_Canvas.gameObject.SetActive(false);
        }

        void OnDestroy()
        {
            if (s_Instance == this)
                s_Instance = null;
        }

        public void Show(string message)
        {
            if (m_Canvas == null) return;

            m_Text.text = message;
            m_Canvas.gameObject.SetActive(true);
            m_HideAt = Time.time + displayDuration;
        }

        void LateUpdate()
        {
            if (m_Canvas == null || !m_Canvas.gameObject.activeSelf) return;

            if (m_HideAt >= 0f && Time.time >= m_HideAt)
            {
                m_Canvas.gameObject.SetActive(false);
                return;
            }

            if (m_Camera == null) m_Camera = Camera.main;
            if (m_Camera == null) return;

            Transform cam = m_Camera.transform;
            Vector3 pos = cam.position
                + cam.forward * distance
                + cam.right   * offset.x
                + cam.up      * offset.y;

            m_CanvasRT.position = pos;
            m_CanvasRT.rotation = Quaternion.LookRotation(cam.forward, cam.up);
        }

        void BuildLabel()
        {
            var root = new GameObject("FeedbackLabel");
            root.transform.SetParent(transform, false);

            m_Canvas = root.AddComponent<Canvas>();
            m_Canvas.renderMode = RenderMode.WorldSpace;
            m_Canvas.worldCamera = m_Camera;

            m_CanvasRT = m_Canvas.GetComponent<RectTransform>();
            m_CanvasRT.sizeDelta = new Vector2(panelWidth, panelWidth * 0.35f);
            m_CanvasRT.localScale = Vector3.one;

            var bg = new GameObject("BG");
            bg.transform.SetParent(root.transform, false);
            var bgImage = bg.AddComponent<Image>();
            bgImage.color = new Color(0f, 0f, 0f, 0.6f);
            var bgRT = bgImage.rectTransform;
            bgRT.anchorMin = Vector2.zero;
            bgRT.anchorMax = Vector2.one;
            bgRT.offsetMin = bgRT.offsetMax = Vector2.zero;

            var textGO = new GameObject("Label");
            textGO.transform.SetParent(root.transform, false);
            m_Text = textGO.AddComponent<TextMeshProUGUI>();
            m_Text.fontSize = fontSize;
            m_Text.color = Color.white;
            m_Text.richText = true;
            m_Text.alignment = TextAlignmentOptions.Center;

            var textRT = m_Text.rectTransform;
            textRT.anchorMin = Vector2.zero;
            textRT.anchorMax = Vector2.one;
            textRT.offsetMin = new Vector2(0.01f, 0.005f);
            textRT.offsetMax = new Vector2(-0.01f, -0.005f);
        }
    }
}
