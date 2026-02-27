using Manager.Contents;
using Unity.Mathematics;
using UnityEngine;

namespace UI.Popup
{
    public class UI_StepsPreview : UIPopup
    {
        private UnitManager _um;
        private GridManager _gm;
        private Canvas _canvas;
        private RectTransform _rectTransform;
        enum Texts
        {
            Text_0
        }
        public override bool Init()
        {
            if (base.Init() == false)
                return false;
            BindText(typeof(Texts));

            _um = FindAnyObjectByType<UnitManager>();
            _gm = FindAnyObjectByType<GridManager>();
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
            return true;
        }

        private void SetTargetPos()
        {
            CheckTarget(_um.GetNowShortestTarget());
        }
        private void CheckTarget(Vector2Int targetCell)
        {
            Camera cam = Camera.main;

            Vector3 worldPos = _gm.GetWorldPos(targetCell, 0);
            Vector3 screenPos = cam.WorldToScreenPoint(worldPos);
            Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);

            if (viewportPos.z > 0 && viewportPos.x >= 0f && viewportPos.x <= 1f && viewportPos.y >= 0f && viewportPos.y <= 1f)
            {
                _canvas.enabled = false;
                return;
            }

            _canvas.enabled = true;

            float marginX = 60f;
            float marginY = 50f;

            float x=marginX;
            float y = Mathf.Clamp(screenPos.y, marginY, Screen.height - marginY);

            float halfWidth = cam.orthographicSize * cam.aspect;
            float leftWorldX = cam.transform.position.x - halfWidth;
            float rightWorldX = cam.transform.position.x + halfWidth;

            float gridOriginX = -6f;
            int cellDistance = 0;

            if (screenPos.x < 0)
            {
                int leftCellX = Mathf.FloorToInt(leftWorldX - gridOriginX);
                cellDistance = targetCell.x - leftCellX;
            }
            else if (screenPos.x > Screen.width)
            {
                int rightCellX = Mathf.FloorToInt(rightWorldX - gridOriginX);
                cellDistance = rightCellX - targetCell.x;
                x = Screen.width - marginX;
            }

            _rectTransform.position = new Vector2(x, y);

            GetText((int)Texts.Text_0).text = $"{Mathf.Abs(cellDistance)}M";
        }
        private void Awake()
        {
            Init();
        }
        private void LateUpdate()
        {
            SetTargetPos();
        }
    }
}

