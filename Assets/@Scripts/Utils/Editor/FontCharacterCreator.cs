#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public class TMPDynamicFontAdder : EditorWindow
    {
        private TMP_FontAsset targetFontAsset;
        private string jsonKey = "";
        private string jsonFolderPath = "Assets/@Resources/Json";

        [MenuItem("Tools/TMP Dynamic Font Adder")]
        public static void ShowWindow()
        {
            GetWindow<TMPDynamicFontAdder>("TMP Dynamic Font Adder");
        }

        private void OnGUI()
        {
            GUILayout.Label("Dynamic TMP 글자 추가 (JSON 기반)", EditorStyles.boldLabel);

            targetFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("Font Asset", targetFontAsset, typeof(TMP_FontAsset), false);
            jsonKey = EditorGUILayout.TextField("JSON 키", jsonKey);

            if (GUILayout.Button("JSON에서 글자 추가"))
            {
                if (targetFontAsset == null || string.IsNullOrEmpty(jsonKey))
                {
                    EditorUtility.DisplayDialog("오류", "폰트 애셋과 JSON 키를 입력하세요.", "확인");
                    return;
                }

                AddCharactersFromJSON(targetFontAsset, jsonKey);
            }
        }

        private void AddCharactersFromJSON(TMP_FontAsset fontAsset, string key)
        {
            if (!Directory.Exists(jsonFolderPath))
            {
                EditorUtility.DisplayDialog("오류", $"폴더 없음: {jsonFolderPath}", "확인");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json", SearchOption.AllDirectories);
            HashSet<char> charsToAdd = new HashSet<char>();

            foreach (string file in jsonFiles)
            {
                string jsonText = File.ReadAllText(file);
                JObject obj = JObject.Parse(jsonText);
                if (obj.ContainsKey(key))
                {
                    string value = obj[key]?.ToString();
                    if (string.IsNullOrEmpty(value)) continue;

                    foreach (char c in value)
                    {
                        if (!char.IsControl(c) && !char.IsWhiteSpace(c) && !fontAsset.HasCharacter(c))
                        {
                            charsToAdd.Add(c);
                        }
                    }
                }
            }

            if (charsToAdd.Count == 0)
            {
                EditorUtility.DisplayDialog("알림", "추가할 글자가 없습니다.", "확인");
                return;
            }

            int addedCount = 0;
            List<char> failedChars = new List<char>();

            foreach (char c in charsToAdd)
            {
                string s = c.ToString();
                string missing;
                bool success = fontAsset.TryAddCharacters(s, out missing);

                if (success)
                {
                    addedCount++;
                }
                else
                {
                    failedChars.Add(c);
                }
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string msg = $"Dynamic 폰트 글자 추가 완료: {addedCount}개 성공";
            if (failedChars.Count > 0)
            {
                msg += $", {failedChars.Count}개 실패 ({string.Join("", failedChars)})";
            }

            Debug.Log(msg);
            EditorUtility.DisplayDialog("완료", msg, "확인");
        }
    }
}
#endif