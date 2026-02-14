#if UNITY_EDITOR
using Data;
using Manager.Core;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Utils.Editor;
using UnityEditorInternal;

namespace Editor
{
    public class SkillDataEditor : EditorWindow
    {
        private static SkillDataEditor _instance;
        private string sourceFilePath = "";
        private string outputDirectory = "";
        private List<SkillData> _skillDatas;
        private SkillDataLoader _loader;
        private int _dataIdx;
        private bool[,] _ranges = new bool[7,7];

        private bool _canAttackAir;
        private bool _canAttackGround;
        private bool _canAttackFloor;
        private bool _canAttackTower;

        private int _airLayer=6;
        private int _groundLayer=7;
        private int _floorLayer=9;
        private int _towerLayer=8;

        private SkillData _skillData;
        [MenuItem("Tools/DataEditor/Skill(스킬의 JSON데이터를 수정합니다)")]
        public static void ShowWindow()
        {
            _instance = GetWindow<SkillDataEditor>();
        }


        private void ConvertBoolToVec2Int(List<Vector2Int> vec)
        {
            vec.Clear();

            for(int x = 0;  x < 7; x++)
            {
                for(int y = 0; y < 7; y++)
                {
                    if(_ranges[x, y])
                    {
                        vec.Add(new Vector2Int(x-3, y-3));
                    }
                }
            }
        }

        private void ClearBool()
        {
            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    _ranges[x, y] = false;
                }
            }
        }
        private void ConvertVec2IntToBool(List<Vector2Int> vec)
        {
            ClearBool();

            for (int i = 0; i < vec.Count; i++)
            {
                _ranges[vec[i].x+3, vec[i].y+3] = true;
            }
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

            _loader = JsonConvert.DeserializeObject<SkillDataLoader>(contents);
            _skillDatas = _loader.skills;
        }

        private void Write(string path)
        {
            if (_skillDatas is null)
            {
                return;
            }
            string data = JsonConvert.SerializeObject(_loader,Formatting.Indented);
            FileIOManager.WriteToFile(path,data,true,Encoding.UTF8);
        }

        private void AddToList(int idx)
        {
            if(_skillData is null)
            {
                return;
            }

            ConvertBoolToVec2Int(_skillData.AttackPos);
            _skillData.AttackableLayers.Clear();

            if(_canAttackAir)
            {
                _skillData.AttackableLayers.Add(_airLayer);
            }

            if (_canAttackGround)
            {
                _skillData.AttackableLayers.Add(_groundLayer);
            }

            if (_canAttackFloor)
            {
                _skillData.AttackableLayers.Add(_floorLayer);
            }

            if (_canAttackTower)
            {
                _skillData.AttackableLayers.Add(_towerLayer);
            }

            for (int i = 0; i < _skillDatas.Count;i++)
            {
                if (_skillDatas[i].SkillID == idx)
                {
                    _skillDatas[i] = _skillData;
                    _skillDatas[i].SkillID = idx;
                    _skillData = null;
                    return;
                }
            }

            _skillData.SkillID = idx;
            _skillDatas.Add( _skillData );
            _skillData = null;
        }

        private void ShowData(int idx)
        {
            _canAttackAir = false;
            _canAttackGround = false;
            _canAttackFloor = false;
            _canAttackTower = false;

            for (int i = 0; i < _skillDatas.Count; i++)
            {
                if (_skillDatas[i].SkillID == idx)
                {
                    _skillData = new SkillData(_skillDatas[i]);
                    ConvertVec2IntToBool(_skillData.AttackPos);

                    for(int q = 0; q < _skillData.AttackableLayers.Count; q++)
                    {
                        if (_skillData.AttackableLayers[q] == _airLayer)
                        {
                            _canAttackAir = true;
                        }
                        else if (_skillData.AttackableLayers[q] == _groundLayer)
                        {
                            _canAttackGround = true;
                        }
                        else if (_skillData.AttackableLayers[q] == _floorLayer)
                        {
                            _canAttackFloor = true;
                        }
                        else if (_skillData.AttackableLayers[q] == _towerLayer)
                        {
                            _canAttackTower = true;
                        }
                    }

                    return;
                }
            }

            _skillData = new SkillData();
            //_skillData.AttackPos = new List<Vector2Int>();
            //_skillData.AttackableLayers = new List<int>();
            ClearBool();
            _skillData.FiringSFX = "";
            _skillData.SFXName = "";
            _skillData.SkillAnimationControllerName = "";
            _skillData.AttackObjectImgName = "";
            _skillData.PrefabName = "";
            _skillData.SkillBOOMParticleName = "";
            _skillData.SkillLaunchParticleName = "";
            _skillData.SkillDescription = "";

        }

        private void RemoveFromList(int dataIdx)
        {
            for(int i = 0; i < _skillDatas.Count;i++)
            {
                if (_skillDatas[i].SkillID == dataIdx)
                {
                    _skillDatas.RemoveAt(i);
                }
            }
        }



        private void OnGUI()
        {
            EditorUtil.ShowDirUI(ref sourceFilePath, ref outputDirectory);
            _airLayer = EditorGUILayout.IntField("공중몹 레이어 번호",_airLayer);
            _groundLayer = EditorGUILayout.IntField("지상몹 레이어 번호", _groundLayer);
            _towerLayer = EditorGUILayout.IntField("타워 레이어 번호", _towerLayer);
            _floorLayer = EditorGUILayout.IntField("밟을 수 있는 기물 레이어 번호", _floorLayer);

            if (GUILayout.Button("파일 로드", GUILayout.Height(30)))
            {
                LoadJson(sourceFilePath);
            }

            _dataIdx = EditorGUILayout.IntField("데이터 인덱스",_dataIdx);

            if(GUILayout.Button("데이터를 에디터 화면으로 로드"))
            {
                ShowData(_dataIdx);
            }

            if(_skillData is not null)
            {
                //List계열 작업 어떻게 해야될지 생각하기

                for(int y = 0; y < 7; y++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for(int x = 0; x < 7; x++)
                    {
                        _ranges[x,y] = EditorGUILayout.Toggle(_ranges[x, y], GUILayout.Width(16));
                    }
                    EditorGUILayout.EndHorizontal();
                }

                _skillData.Damage = EditorGUILayout.IntField("Damage", _skillData.Damage);
                _skillData.Cooldown = EditorGUILayout.FloatField("Cooldown", _skillData.Cooldown);
                _skillData.CanAttackMultiple = EditorGUILayout.Toggle("CanAttackMultiple", _skillData.CanAttackMultiple);
                _skillData.SpeedPerCell = EditorGUILayout.FloatField("SpeedPerCell", _skillData.SpeedPerCell);
                _skillData.FiringSFX = EditorGUILayout.TextField("FiringSFX", _skillData.FiringSFX);
                _skillData.SFXName = EditorGUILayout.TextField("SFXName", _skillData.SFXName);
                _skillData.SkillAnimationControllerName = EditorGUILayout.TextField("SkillAnimationControllerName", _skillData.SkillAnimationControllerName);
                _skillData.AttackObjectImgName = EditorGUILayout.TextField("AttackObjectImgName", _skillData.AttackObjectImgName);
                _skillData.PrefabName = EditorGUILayout.TextField("PrefabName", _skillData.PrefabName);
                _skillData.SkillBOOMParticleName = EditorGUILayout.TextField("SkillBOOMParticleName", _skillData.SkillBOOMParticleName);
                _skillData.SkillLaunchParticleName = EditorGUILayout.TextField("SkillLaunchParticleName", _skillData.SkillLaunchParticleName);
                _skillData.SkillDescription = EditorGUILayout.TextField("SkillDescription", _skillData.SkillDescription);

                _canAttackAir = EditorGUILayout.Toggle("공중몹 공격 가능?",_canAttackAir);
                _canAttackGround = EditorGUILayout.Toggle("지상몹 공격 가능?", _canAttackGround);
                _canAttackTower = EditorGUILayout.Toggle("타워 공격 가능?", _canAttackTower);
                _canAttackFloor = EditorGUILayout.Toggle("밟을 수 있는 기물 공격 가능?", _canAttackFloor);

            }


            if(GUILayout.Button($"데이터를 스킬 리스트에 인덱스 {_dataIdx}로 반영"))
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