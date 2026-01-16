using Data;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class StageManager
    {
        private StageData _nowStage;

        public bool Init(int idx)
        {
            if (Managers.Instance.DataManager.StageDic.TryGetValue(idx, out _nowStage))
            {
                return true;
            }

            return false;
        }

        public bool DoesReachedLeftEnd()
        {
            return _nowStage.PrevIdx == (int)ControlValue.INVALID;
        }

        public bool DoesReachedRightEnd()
        {
            return _nowStage.NextIdx == (int)ControlValue.INVALID;
        }

        public bool TryGetNextStage()
        {
            if(DoesReachedRightEnd())
            {
                return false;
            }
            _nowStage = Managers.Instance.DataManager.StageDic[_nowStage.NextIdx];
            return true;
        }

        public bool TryGetPrevStage()
        {
            if (DoesReachedLeftEnd())
            {
                return false;
            }
            _nowStage = Managers.Instance.DataManager.StageDic[_nowStage.PrevIdx];
            return true;
        }

    }
}
