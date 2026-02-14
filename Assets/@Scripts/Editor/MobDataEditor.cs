#if UNITY_EDITOR
using Data;
using Manager.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Utils.Editor;

namespace Editor
{
    public class MobDataEditor : EditorWindow
    {
        private static MobDataEditor _instance;
        private string sourceFilePath = "";
        private string outputDirectory = "";
        private List<MonsterData> _mobDatas;
        private MonsterDataLoader _loader;
        private int _dataIdx;

        private MonsterData _mobData;
        [MenuItem("Tools/DataEditor/Mob(몹의 JSON데이터를 수정합니다)")]
        public static void ShowWindow()
        {
            _instance = GetWindow<MobDataEditor>();
        }

        void LoadJson(string path)
        {
            string contents;
            FileIOManager.ReadFromFile(path, out contents, Encoding.UTF8);

            if(string.IsNullOrEmpty(contents))
            {
                Debug.Log("failed monster data");
                return;
            }

            _loader = JsonConvert.DeserializeObject<MonsterDataLoader>(contents);
            _mobDatas = _loader.monsters;
        }

        private void Write(string path)
        {
            if (_mobDatas is null)
            {
                return;
            }
            string data = JsonConvert.SerializeObject(_loader,Formatting.Indented);
            FileIOManager.WriteToFile(path,data,true,Encoding.UTF8);
        }

        private void AddToList(int idx)
        {
            if(_mobData is null)
            {
                return;
            }

            for(int i = 0; i < _mobDatas.Count;i++)
            {
                if (_mobDatas[i].MonsterId == idx)
                {
                    _mobDatas[i] = _mobData;
                    _mobDatas[i].MonsterId = idx;
                    _mobData = null;
                    return;
                }
            }

            _mobData.MonsterId = idx;
            _mobDatas.Add( _mobData );
            _mobData = null;
        }

        private void ShowData(int idx)
        {
            
            for (int i = 0; i < _mobDatas.Count; i++)
            {
                if (_mobDatas[i].MonsterId == idx)
                {
                    _mobData = new MonsterData(_mobDatas[i]);
                    return;
                }
            }

            _mobData = new MonsterData();
            _mobData.PrefabName = "";
            _mobData.MonsterImgName = "";
            _mobData.HitSound = "";
            _mobData.AnimatorControllerName="";
            _mobData.DeadParticleName = "";
        }

        private void RemoveFromList(int dataIdx)
        {
            for(int i = 0; i < _mobDatas.Count;i++)
            {
                if (_mobDatas[i].MonsterId == dataIdx)
                {
                    _mobDatas.RemoveAt(i);
                }
            }
        }

        private void OnGUI()
        {
            EditorUtil.ShowDirUI(ref sourceFilePath, ref outputDirectory);

            if (GUILayout.Button("파일 로드", GUILayout.Height(30)))
            {
                LoadJson(sourceFilePath);
            }

            _dataIdx = EditorGUILayout.IntField("데이터 인덱스",_dataIdx);

            if(GUILayout.Button("데이터를 에디터 화면으로 로드"))
            {
                ShowData(_dataIdx);
            }

            if(_mobData is not null)
            {
                _mobData.InvincibilityTime = EditorGUILayout.FloatField("InvincibilityTime", _mobData.InvincibilityTime);
                _mobData.MonsterSpeed = EditorGUILayout.FloatField("MonsterSpeed", _mobData.MonsterSpeed);
                _mobData.MonsterHP = EditorGUILayout.IntField("MonsterHP", _mobData.MonsterHP);
                _mobData.PrefabName = EditorGUILayout.TextField("PrefabName", _mobData.PrefabName);
                _mobData.MonsterImgName = EditorGUILayout.TextField("MonsterImgName", _mobData.MonsterImgName);
                _mobData.RewardCurrency = EditorGUILayout.IntField("RewardCurrency", _mobData.RewardCurrency);
                _mobData.HitSound = EditorGUILayout.TextField("HitSound", _mobData.HitSound);
                _mobData.AnimatorControllerName = EditorGUILayout.TextField("AnimatorControllerName", _mobData.AnimatorControllerName);
                _mobData.DeadParticleName = EditorGUILayout.TextField("DeadParticleName", _mobData.DeadParticleName);
                _mobData.SkillID = EditorGUILayout.IntField("SkillID", _mobData.SkillID);
            }


            if(GUILayout.Button($"데이터를 몹 리스트에 인덱스 {_dataIdx}로 반영"))
            {
                AddToList(_dataIdx);
            }

            if(GUILayout.Button($"{_dataIdx}인덱스 데이터를 삭제"))
            {
                RemoveFromList(_dataIdx);
            }

            if(GUILayout.Button("파일에 다시 저장"))
            {
                Write(outputDirectory);
            }
        }
    }
}
#endif