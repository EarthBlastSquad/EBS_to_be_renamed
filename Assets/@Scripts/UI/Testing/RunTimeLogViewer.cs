using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
namespace UI.Testing
{

    public class RuntimeLogViewer : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform content;
        [SerializeField] private RectTransform viewport;

        [Header("Settings")]
        [SerializeField] private int poolSize = 20;
        [SerializeField] private int maxLogs = 666775;
        [SerializeField] private float lineHeight = 50f;
        [SerializeField] private int fontSize = 30;

        private readonly LinkedList<LogEntry> _deque = new LinkedList<LogEntry>();
        private TextMeshProUGUI[] _pool;

        private int _topIndex = 0;
        private bool _dirty = false;

        private struct LogEntry
        {
            public string msg;
            public LogType type;
            public string stack;
            public DateTime time;
        }

        void Awake()
        {
            if (scrollRect == null || content == null || viewport == null)
            {
                Debug.LogError("[RuntimeLogViewer] ScrollRect/content/viewport ¼¼ÆÃ ¾ÈµÊ");
                enabled = false;
                return;
            }

            BuildPool();
            UpdateContentHeight();
            RefreshVisible();
            Application.logMessageReceived -= OnLogReceived;
            Application.logMessageReceived += OnLogReceived;
            Debug.Log("TestLog");
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogReceived;
        }
        void OnEnable()
        {

            scrollRect.onValueChanged.AddListener(OnScroll);
        }

        void OnDisable()
        {

            scrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        private void BuildPool()
        {
            _pool = new TextMeshProUGUI[poolSize];

            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject go = new GameObject($"LogLine_{i}");
                go.transform.SetParent(content, false);

                var rt = go.AddComponent<RectTransform>();
                rt.anchorMin = new Vector2(0, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.pivot = new Vector2(0.5f, 1);

                rt.anchoredPosition = new Vector2(0, -i * lineHeight);
                rt.sizeDelta = new Vector2(0, lineHeight);

                var text = go.AddComponent<TextMeshProUGUI>();
                text.raycastTarget = false;
                text.enableWordWrapping = false;
                text.overflowMode = TextOverflowModes.Truncate;
                text.fontSize = fontSize;
                text.alignment = TextAlignmentOptions.Left;
                text.richText = true;
                text.text = "";

                _pool[i] = text;
            }
        }

        private void OnLogReceived(string condition, string stackTrace, LogType type)
        {
            var entry = new LogEntry()
            {
                msg = condition,
                type = type,
                stack = stackTrace,
                time = DateTime.Now
            };

            _deque.AddLast(entry);

            if (_deque.Count > maxLogs)
            {
                _deque.RemoveFirst();

                if (_topIndex > 0)
                    _topIndex--;
            }

            UpdateContentHeight();

            if (IsAtBottom())
                ScrollToBottom();

            _dirty = true;
        }

        void Update()
        {
            if (_dirty)
            {
                _dirty = false;
                RefreshVisible();
            }
        }

        private void OnScroll(Vector2 _)
        {
            UpdateTopIndexFromScroll();
            RefreshVisible();
        }

        private void UpdateTopIndexFromScroll()
        {
            float contentY = content.anchoredPosition.y;
            int idx = Mathf.FloorToInt(contentY / lineHeight);

            int maxTop = Mathf.Max(0, _deque.Count - poolSize);
            _topIndex = Mathf.Clamp(idx, 0, maxTop);
        }

        private void RefreshVisible()
        {
            int count = _deque.Count;

            var arr = new LogEntry[count];
            int i = 0;
            foreach (var e in _deque)
                arr[i++] = e;

            for (int p = 0; p < poolSize; p++)
            {
                int logIndex = _topIndex + p;

                if (logIndex >= count)
                {
                    _pool[p].text = "";
                    continue;
                }

                var entry = arr[logIndex];

                string color = entry.type switch
                {
                    LogType.Error => "#ff4b4b",
                    LogType.Exception => "#ff00ff",
                    LogType.Warning => "#ffcc00",
                    _ => "#ffffff"
                };

                string time = entry.time.ToString("HH:mm:ss");

                _pool[p].text =
                    $"<color=#888888>[{time}]</color> <color={color}>[{entry.type}]</color> {entry.msg}";
            }

            for (int p = 0; p < poolSize; p++)
            {
                _pool[p].rectTransform.anchoredPosition =
                    new Vector2(0, -(_topIndex + p) * lineHeight);
            }
        }

        private void UpdateContentHeight()
        {
            float height = Mathf.Max(_deque.Count * lineHeight, viewport.rect.height);
            content.sizeDelta = new Vector2(content.sizeDelta.x, height);
        }

        private bool IsAtBottom()
        {
            return scrollRect.verticalNormalizedPosition <= 0.001f;
        }

        public void ScrollToBottom()
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            UpdateTopIndexFromScroll();
            RefreshVisible();
        }

        public void ClearLogs()
        {
            _deque.Clear();
            _topIndex = 0;
            UpdateContentHeight();
            RefreshVisible();
        }
    }
}