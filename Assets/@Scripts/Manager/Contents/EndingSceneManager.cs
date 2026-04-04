using Data;
using System.Collections.Generic;
using UnityEngine;
using Utils.Defines;

namespace Manager.Contents
{
    public class EndingSceneManager : MonoBehaviour
    {
        private List<string> _contents;
        private EndingData _data;
        private int _contentIdx = -1;

        private void Start()
        {
            StageData stage = Managers.Instance.StageManager.GetNowStageData();
            var lastStatus = Managers.Instance.GameManager.LastGameEndStatus;
            bool isCleared = lastStatus.Item2;

            if(isCleared)
            {
                _data = Managers.Instance.DataManager.EndingDic[stage.GoodEndingDataIdx];
            }
            else
            {
                _data = Managers.Instance.DataManager.EndingDic[stage.BadEndingDataIdx];
            }

            if(_data is not null)
            {
                _contents = _data.Contents;
            }
        }

        public EndingContentType GetNowContent(ref string content)
        {
            _contentIdx++;
            if(_contents is null || _contents.Count <= _contentIdx)
            {
                return EndingContentType.TYPE_INVALID;
            }

            if (_contents[_contentIdx] == "__load__")
            {
                _contentIdx++;
                if(_contents.Count <= _contentIdx)
                {
                    return EndingContentType.TYPE_INVALID;
                }
                content = _contents[_contentIdx];
                return EndingContentType.TYPE_IMAGE;
            }
            else if (_contents[_contentIdx] == "__play__")
            {
                _contentIdx++;
                if(_contents.Count <= _contentIdx)
                {
                    return EndingContentType.TYPE_INVALID;
                }
                content = _contents[_contentIdx];
                return EndingContentType.TYPE_SOUND;
            }

            content = _contents[_contentIdx];
            return EndingContentType.TYPE_TEXT;
        }

        public EndingContentType Skip()
        {
            //어느 씬으로 넘길지 아직 미정
            //Managers.Instance.SceneManagerEx.LoadScene();
            return EndingContentType.TYPE_INVALID;
        }
    }
}