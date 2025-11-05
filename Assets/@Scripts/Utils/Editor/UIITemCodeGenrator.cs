using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
namespace Utils.Editor
{
    public class UIItemCodeGenerator
    {
        private static Dictionary<string, List<UnityEngine.Object>> _objects = new Dictionary<string, List<UnityEngine.Object>>();

        [MenuItem("UI_CodeGen/ItemCodeGen(_Item 코드를 제작합니다.)")]
        public static void ItemCodeGenerate()
        {
            GameObject[] rootObj = GetSceneRootObjects();
            GameObject uiRoot = null;

            // @UI_Root 찾기
            for (int i = 0; i < rootObj.Length; i++)
            {
                if (rootObj[i].name == "@UI_Root")
                {
                    uiRoot = rootObj[i];
                    break;
                }
            }

            if (uiRoot == null)
            {
                Debug.LogError("@UI_Root를 찾을 수 없습니다!");
                return;
            }

            // @UI_Root 아래의 모든 직접 자식들을 순회
            for (int i = 0; i < uiRoot.transform.childCount; i++)
            {
                GameObject child = uiRoot.transform.GetChild(i).gameObject;

                // UI_로 시작하고 _Item을 포함하는 오브젝트만 처리
                if (child.name.StartsWith("UI_") && child.name.Contains("_Item"))
                {
                    Debug.Log($"Processing Item: {child.name}");

                    _objects.Clear(); // 각 UI마다 초기화

                    string UI_Name = child.name;
                    FindAllObjects(child.transform);

                    // 파일 경로 결정
                    string filePath = GetFilePath(UI_Name);

                    // 기존 파일이 있는지 확인
                    if (File.Exists(filePath))
                    {
                        UpdateExistingCode(filePath, UI_Name);
                    }
                    else
                    {
                        CreateNewCode(filePath, UI_Name);
                    }
                }
            }

            _objects.Clear();
            AssetDatabase.Refresh();
            Debug.Log("모든 Item 코드 생성 완료!");
        }

        private static string GetFilePath(string uiName)
        {
            return $"Assets/@Scripts/UI/Item/{uiName}.cs";
        }

        private static void CreateNewCode(string filePath, string uiName)
        {
            string Enums = "";
            string enumsValue = "";
            string EnumsInit = "";
            string ButtonsEvent = "\t";

            foreach (var go in _objects)
            {
                string key = go.Key.ToString();
                string value = "";

                for (int j = 0; j < _objects[go.Key].Count; j++)
                {
                    if (j < _objects[go.Key].Count)
                    {
                        value += _objects[go.Key][j].name + "," + "\n" + "\t" + "\t";

                        switch (key)
                        {
                            case "Texts":
                                enumsValue += string.Format(ItemCodeFormat.GetEnumsObject, "Text", "Texts", _objects[go.Key][j].name) + "\t" + "\t";
                                break;
                            case "Images":
                                enumsValue += string.Format(ItemCodeFormat.GetEnumsObject, "Image", "Images", _objects[go.Key][j].name) + "\t" + "\t";
                                break;
                            case "Buttons":
                                enumsValue += string.Format(ItemCodeFormat.OnClickAddListener, "Button", "Buttons", _objects[go.Key][j].name) + "\t" + "\t";
                                ButtonsEvent += string.Format(ItemCodeFormat.OnclickEventCreater, _objects[go.Key][j].name) + "\t";
                                break;
                            case "GameObjects":
                                enumsValue += string.Format(ItemCodeFormat.GetEnumsObject, "Object", "GameObjects", _objects[go.Key][j].name) + "\t" + "\t";
                                break;
                        }
                    }
                }

                switch (key)
                {
                    case "Texts":
                        EnumsInit += string.Format(ItemCodeFormat.enumsInit, "Text", "Texts") + "\n" + "\t" + "\t";
                        break;
                    case "Images":
                        EnumsInit += string.Format(ItemCodeFormat.enumsInit, "Image", "Images") + "\n" + "\t" + "\t";
                        break;
                    case "Buttons":
                        EnumsInit += string.Format(ItemCodeFormat.enumsInit, "Button", "Buttons") + "\n" + "\t" + "\t";
                        break;
                    case "GameObjects":
                        EnumsInit += string.Format(ItemCodeFormat.enumsInit, "Object", "GameObjects") + "\n" + "\t" + "\t";
                        break;
                }

                string data = string.Format(ItemCodeFormat.enums_GameObjects, key, value);
                Enums += data + "\t";
            }

            string Init = string.Format(ItemCodeFormat.init, EnumsInit, enumsValue);
            Init += ButtonsEvent;

            string finalstring = string.Format(ItemCodeFormat.classHeader_item, uiName, Enums, Init);
            string files = ItemCodeFormat.codeHeader + "\n" + finalstring;

            // 디렉토리가 없으면 생성
            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, files);
            Debug.Log($"새 Item 코드 생성 완료: {uiName}");
        }

        private static void UpdateExistingCode(string filePath, string uiName)
        {
            string existingCode = File.ReadAllText(filePath);

            // 각 enum 타입별로 업데이트
            foreach (var go in _objects)
            {
                string enumType = go.Key;
                existingCode = UpdateEnum(existingCode, enumType, go.Value);
                existingCode = UpdateBindings(existingCode, enumType, go.Value);

                if (enumType == "Buttons")
                {
                    existingCode = UpdateButtonEvents(existingCode, go.Value);
                }
            }

            File.WriteAllText(filePath, existingCode);
            Debug.Log($"기존 Item 코드 업데이트 완료: {uiName}");
        }

        private static string UpdateEnum(string code, string enumType, List<UnityEngine.Object> objects)
        {
            // enum 블록 찾기
            string pattern = $@"enum {enumType}\s*{{\s*(.*?)\s*}}";
            Match match = Regex.Match(code, pattern, RegexOptions.Singleline);

            if (!match.Success)
            {
                // enum이 없으면 추가
                return AddNewEnum(code, enumType, objects);
            }

            // 기존 enum 항목들 파싱
            string enumContent = match.Groups[1].Value;
            HashSet<string> existingItems = new HashSet<string>();

            foreach (Match item in Regex.Matches(enumContent, @"(\w+),"))
            {
                existingItems.Add(item.Groups[1].Value.Trim());
            }

            // 새로운 항목들 추가
            List<string> newItems = new List<string>();
            foreach (var obj in objects)
            {
                if (!existingItems.Contains(obj.name))
                {
                    newItems.Add(obj.name);
                }
            }

            if (newItems.Count > 0)
            {
                string newEnumContent = enumContent.TrimEnd();
                foreach (var item in newItems)
                {
                    newEnumContent += $"\n\t\t{item},";
                }
                newEnumContent += "\n\t";

                code = Regex.Replace(code, pattern, $"enum {enumType}\n\t{{\n\t{newEnumContent}\n\t}}", RegexOptions.Singleline);
                Debug.Log($"{enumType}에 {newItems.Count}개 항목 추가됨");
            }

            return code;
        }

        private static string AddNewEnum(string code, string enumType, List<UnityEngine.Object> objects)
        {
            string enumItems = "";
            foreach (var obj in objects)
            {
                enumItems += $"\t\t{obj.name},\n";
            }

            string newEnum = $"\tenum {enumType}\n\t{{\n{enumItems}\t}}\n\t";

            // 마지막 enum 뒤에 추가
            int lastEnumIndex = code.LastIndexOf("}\n\t}");
            if (lastEnumIndex != -1)
            {
                code = code.Insert(lastEnumIndex + 4, newEnum);
            }

            return code;
        }

        private static string UpdateBindings(string code, string enumType, List<UnityEngine.Object> objects)
        {
            // Object Bind 섹션 찾기
            string bindPattern = @"#region Object Bind\s*(.*?)\s*#endregion";
            Match bindMatch = Regex.Match(code, bindPattern, RegexOptions.Singleline);

            if (!bindMatch.Success) return code;

            string bindContent = bindMatch.Groups[1].Value;

            // 이미 바인딩된 타입 확인
            string typeBindPattern = GetTypeBindPattern(enumType);
            bool hasTypeBind = Regex.IsMatch(bindContent, typeBindPattern);

            // Bind 추가
            if (!hasTypeBind)
            {
                string componentType = GetComponentType(enumType);
                string newBind = $"\n\t\tBind{componentType}(typeof({enumType}));";
                bindContent += newBind;
            }

            // Object EventBind 섹션 찾기
            string eventBindPattern = @"#region Object EventBind\s*(.*?)\s*#endregion";
            Match eventMatch = Regex.Match(code, eventBindPattern, RegexOptions.Singleline);

            if (eventMatch.Success)
            {
                string eventContent = eventMatch.Groups[1].Value;

                foreach (var obj in objects)
                {
                    string bindStatement = GetBindStatement(enumType, obj.name);
                    if (!string.IsNullOrEmpty(bindStatement) && !eventContent.Contains($"{enumType}.{obj.name}"))
                    {
                        eventContent += $"\n\t\t{bindStatement}";
                    }
                }

                code = Regex.Replace(code, eventBindPattern, $"#region Object EventBind{eventContent}\n\t\t#endregion", RegexOptions.Singleline);
            }

            code = Regex.Replace(code, bindPattern, $"#region Object Bind{bindContent}\n\t\t#endregion", RegexOptions.Singleline);
            return code;
        }

        private static string GetTypeBindPattern(string enumType)
        {
            string componentType = GetComponentType(enumType);
            return $@"Bind{componentType}\(typeof\({enumType}\)\)";
        }

        private static string GetComponentType(string enumType)
        {
            switch (enumType)
            {
                case "Texts": return "Text";
                case "Images": return "Image";
                case "Buttons": return "Button";
                case "GameObjects": return "Object";
                default: return "Object";
            }
        }

        private static string GetBindStatement(string enumType, string objectName)
        {
            string componentType = GetComponentType(enumType);

            switch (enumType)
            {
                case "Buttons":
                    return $"Get{componentType}((int){enumType}.{objectName}).gameObject.BindEvent(OnClick_{objectName});\n\t\t" +
                           $"Get{componentType}((int){enumType}.{objectName}).gameObject.GetOrAddComponent<UI_ButtonAnimation>();";
                default:
                    return "";
            }
        }

        private static string UpdateButtonEvents(string code, List<UnityEngine.Object> buttons)
        {
            // 클래스의 끝 부분 찾기 (마지막 중괄호 전)
            int lastBraceIndex = code.LastIndexOf("}");

            foreach (var button in buttons)
            {
                string eventMethod = $"OnClick_{button.name}";
                string eventPattern = $@"void {eventMethod}\(\)";

                // 이미 존재하는 이벤트 메서드인지 확인
                if (!Regex.IsMatch(code, eventPattern))
                {
                    string newEvent = $"\n\tvoid {eventMethod}()\n\t{{\n\n\t}}\n";
                    code = code.Insert(lastBraceIndex, newEvent);
                    lastBraceIndex = code.LastIndexOf("}");
                    Debug.Log($"버튼 이벤트 추가됨: {eventMethod}");
                }
            }

            return code;
        }

        private static void FindAllObjects(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Debug.Log(child.name);

                if (child.name.Contains("Text") && child.GetComponent<TMP_Text>())
                {
                    AddToObjectDictionary("Texts", child);
                }
                else if (child.name.Contains("Button") && child.GetComponent<Button>())
                {
                    AddToObjectDictionary("Buttons", child);
                }
                else if (child.name.Contains("Image") && child.GetComponent<Image>())
                {
                    AddToObjectDictionary("Images", child);
                }
                else
                {
                    AddToObjectDictionary("GameObjects", child);
                }

                FindAllObjects(child);
            }
        }

        private static void AddToObjectDictionary(string key, Transform obj)
        {
            if (_objects.TryGetValue(key, out var value))
            {
                _objects[key].Add(obj);
            }
            else
            {
                List<UnityEngine.Object> objects = new List<UnityEngine.Object>();
                _objects.Add(key, objects);
                _objects[key].Add(obj);
            }
        }

        private static GameObject[] GetSceneRootObjects()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            return currentScene.GetRootGameObjects();
        }
    }

    class ItemCodeFormat
    {
        public static string codeHeader =
    @"using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using static Define;

";
        public static string classHeader_item =
    @"public class {0} : UI_Base
{{
    {1}

    {2}
}}
";
        #region Enums
        public static string enums_GameObjects =
    @"enum {0}
    {{  
    
        {1}

    }}
";
        public static string enumsInit =
    @"Bind{0}(typeof({1}));";
        #endregion
        public static string init =
    @"public override bool Init()
    {{

        if (base.Init() == false)
            return false;

#region Object Bind

        {0}

#endregion

#region Object EventBind

        {1}

#endregion

        return true;
    }}
";
        public static string GetEnumsObject =
    @"Get{0}((int){1}.{2});
";

        public static string OnclickEventCreater =
    @"void OnClick_{0}()
    {{
    
    }}
";
        public static string OnClickAddListener =
    @"
        Get{0}((int){1}.{2}).gameObject.BindEvent(OnClick_{2});
        Get{0}((int){1}.{2}).gameObject.GetOrAddComponent<UI_ButtonAnimation>();
";
    }
}