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
    public class TowerDataEditor : EditorWindow
    {
        private static TowerDataEditor _instance;
        private string sourceFilePath = "";
        private string outputDirectory = "";
        private List<TowerData> _towerDatas;
        private TowerDataLoader _loader;
        private int _dataIdx;

        private TowerData _towerData;
        [MenuItem("Tools/DataEditor/Tower(타워의 JSON데이터를 수정합니다)")]
        public static void ShowWindow()
        {
            _instance = GetWindow<TowerDataEditor>();
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

            _loader = JsonConvert.DeserializeObject<TowerDataLoader>(contents);
            _towerDatas = _loader.towers;
        }

        private void Write(string path)
        {
            if (_towerDatas is null)
            {
                return;
            }
            string data = JsonConvert.SerializeObject(_loader,Formatting.Indented);
            FileIOManager.WriteToFile(path,data,true,Encoding.UTF8);
        }

        private void AddToList(int idx)
        {
            if(_towerData is null)
            {
                return;
            }

            for(int i = 0; i < _towerDatas.Count;i++)
            {
                if (_towerDatas[i].TowerId == idx)
                {
                    _towerDatas[i] = _towerData;
                    _towerDatas[i].TowerId = idx;
                    _towerData = null;
                    return;
                }
            }

            _towerData.TowerId = idx;
            _towerDatas.Add( _towerData );
            _towerData = null;
        }

        private void ShowData(int idx)
        {
            
            for (int i = 0; i < _towerDatas.Count; i++)
            {
                if (_towerDatas[i].TowerId == idx)
                {
                    _towerData = new TowerData(_towerDatas[i]);
                    return;
                }
            }

            _towerData = new TowerData();
            _towerData.TowerName = "";
            _towerData.PrefabName = "";
            _towerData.TowerImgName = "";
            _towerData.CrackSpriteNameBase = "";
            _towerData.TowerDescription = "";
            _towerData.HitSound = "";
            _towerData.AnimatorControllerName = "";
            _towerData.DeadParticleName = "";
        }

        private void RemoveFromList(int dataIdx)
        {
            for(int i = 0; i < _towerDatas.Count;i++)
            {
                if (_towerDatas[i].TowerId == dataIdx)
                {
                    _towerDatas.RemoveAt(i);
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

            if(_towerData is not null)
            {
                _towerData.InvincibilityTime = EditorGUILayout.FloatField("InvincibilityTime", _towerData.InvincibilityTime);
                _towerData.TowerName = EditorGUILayout.TextField("TowerName", _towerData.TowerName);
                _towerData.PrefabName = EditorGUILayout.TextField("PrefabName", _towerData.PrefabName);
                _towerData.TowerImgName = EditorGUILayout.TextField("TowerImgName", _towerData.TowerImgName);
                _towerData.CrackSpriteNameBase = EditorGUILayout.TextField("CrackSpriteNameBase", _towerData.CrackSpriteNameBase);
                _towerData.TowerHP = EditorGUILayout.IntField("TowerHP", _towerData.TowerHP);
                _towerData.DemendedCurrency = EditorGUILayout.IntField("DemendedCurrency", _towerData.DemendedCurrency);
                _towerData.TowerDescription = EditorGUILayout.TextField("TowerDescription", _towerData.TowerDescription);
                _towerData.HitSound = EditorGUILayout.TextField("HitSound", _towerData.HitSound);
                _towerData.AnimatorControllerName = EditorGUILayout.TextField("AnimatorControllerName", _towerData.AnimatorControllerName);
                _towerData.DeadParticleName = EditorGUILayout.TextField("DeadParticleName", _towerData.DeadParticleName);
                _towerData.SkillId = EditorGUILayout.IntField("SkillId", _towerData.SkillId);
            }


            if(GUILayout.Button($"데이터를 타워 리스트에 인덱스 {_dataIdx}로 반영"))
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