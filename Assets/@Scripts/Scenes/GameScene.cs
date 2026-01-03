using Data;
using System;
using UnityEngine;

namespace Scenes
{
    public class GameScene : BaseScene
    {
        public Action<WaveData> OnWaveChanged;
        protected override void Init()
        {
            base.Init();
            SceneType = Utils.Defines.SceneNames.GameScene;
        }
    }
}