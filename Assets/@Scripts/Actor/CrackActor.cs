using Manager;
using UnityEngine;

namespace Actor
{
    public class CrackActor
    {
        private SpriteRenderer _renderer;
        private string _crackNameBase="";
        public CrackActor(SpriteRenderer renderer)
        {
            _renderer = renderer;
        }
        public void Init(string crackNameBase,int initialStage)
        {
            _crackNameBase = crackNameBase;
            _renderer.sprite = Managers.Instance.ResourceManager.Load<Sprite>($"{crackNameBase}{initialStage}");
        }
        public void UpdateCrack(int stage)
        {
            _renderer.sprite = Managers.Instance.ResourceManager.Load<Sprite>($"{_crackNameBase}{stage}");
        }
    }
}