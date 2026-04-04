using DG.Tweening;
using UnityEngine;

namespace Actor
{
    public class HeartActor : MonoBehaviour
    {
        private float _punchStrength = 0.1f;
        private float _duration = 0.1f;
        private float _interval = 2f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            DOTween.Sequence()
            // 1. 위로 움찔 (현재 위치에서 punchStrength만큼 상대적 이동)
            .Append(transform.DOLocalMoveY(_punchStrength, _duration)
                .SetRelative()
                .SetEase(Ease.OutQuad))

            // 2. 다시 제자리로 (상대적 이동을 반대로 적용)
            .Append(transform.DOLocalMoveY(-_punchStrength, _duration)
                .SetRelative()
                .SetEase(Ease.InQuad))

            // 3. 2초 대기 (시퀀스 끝에 추가하여 반복 직전에 멈춤)
            .AppendInterval(_interval)

            // 무한 반복
            .SetLoops(-1);
        }
    }
}