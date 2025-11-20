using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UI.Scene;
using UnityEditor;
using UnityEngine;
namespace Editor
{
    public class UISetScene : EditorWindow
    {
        private Component uiScene;
        private MethodInfo[] sceneMethods;
        private Vector2 scrollPos;
        private Dictionary<GameObject, bool> originalActiveStates;

        [MenuItem("Tools/UI에서 버튼들을 끄고 켜보자")]
        public static void ShowWindow()
        {
            GetWindow<UISetScene>("UISetScene");
        }

        private void OnGUI()
        {
            GUILayout.Label("씬 UI 제어", EditorStyles.boldLabel);

            // 원본 씬 상태 저장 / 되돌리기 버튼
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("현재 UI 상태 저장"))
            {
                SaveUIActiveStates();
            }

            GUI.enabled = originalActiveStates != null;
            if (GUILayout.Button("저장된 상태로 되돌리기"))
            {
                RestoreUIActiveStates();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("UIScene 찾기 / 바인딩"))
            {
                FindUIScene();
            }

            GUILayout.Space(10);

            // UIScene protected 메서드 버튼 표시
            if (sceneMethods != null)
            {
                foreach (var method in sceneMethods)
                {
                    if (GUILayout.Button(method.Name))
                    {
                        InvokeUISceneMethod(method);
                    }
                }
            }
        }
        private void SaveUIActiveStates()
        {
            if (uiScene == null)
            {
                Debug.LogWarning("UIScene이 설정되지 않았습니다.");
                return;
            }

            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                Debug.LogWarning("@UI_Root 오브젝트를 찾을 수 없습니다.");
                return;
            }

            originalActiveStates = new Dictionary<GameObject, bool>();

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                originalActiveStates[child.gameObject] = child.gameObject.activeSelf;
            }

            Debug.Log($"UI 활성화 상태 {originalActiveStates.Count}개 저장 완료");
        }

        private void RestoreUIActiveStates()
        {
            if (originalActiveStates == null)
            {
                Debug.LogWarning("저장된 UI 상태가 없습니다.");
                return;
            }

            foreach (var kvp in originalActiveStates)
            {
                if (kvp.Key != null)
                    kvp.Key.SetActive(kvp.Value);
            }

            Debug.Log("저장된 활성화 상태로 복원 완료!");
        }

        // Init 호출 (필요하면)
        private void FindUIScene()
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                Debug.LogWarning("@UI_Root 오브젝트를 찾을 수 없습니다.");
                uiScene = null;
                sceneMethods = null;
                return;
            }

            uiScene = root.GetComponent<UIScene>();
            if (uiScene == null)
            {
                Debug.LogWarning("@UI_Root에 UIScene 컴포넌트가 없습니다.");
                sceneMethods = null;
                return;
            }

            // 필요한 바인딩만 수행
            var uiType = uiScene.GetType();

            // GameObjects Enum 바인딩
            var bindObjectMethod = uiType.GetMethod("BindObject", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bindObjectMethod != null)
            {
                var gameObjectsEnum = uiType.GetNestedType("GameObjects", BindingFlags.Public | BindingFlags.NonPublic);
                if (gameObjectsEnum != null)
                    bindObjectMethod.Invoke(uiScene, new object[] { gameObjectsEnum });
            }

            // Buttons Enum 바인딩
            var bindButtonMethod = uiType.GetMethod("BindButton", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bindButtonMethod != null)
            {
                var buttonsEnum = uiType.GetNestedType("Buttons", BindingFlags.Public | BindingFlags.NonPublic);
                if (buttonsEnum != null)
                    bindButtonMethod.Invoke(uiScene, new object[] { buttonsEnum });
            }

            // Texts Enum 바인딩
            var bindTextMethod = uiType.GetMethod("BindText", BindingFlags.NonPublic | BindingFlags.Instance);
            if (bindTextMethod != null)
            {
                var textsEnum = uiType.GetNestedType("Texts", BindingFlags.Public | BindingFlags.NonPublic);
                if (textsEnum != null)
                    bindTextMethod.Invoke(uiScene, new object[] { textsEnum });
            }


            // 여기서 protected 메서드만 가져오고 Finalize/MemberwiseClone 제외
            sceneMethods = uiScene.GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.IsFamily                  // protected
                            && m.DeclaringType == uiScene.GetType()) // 부모 클래스나 System.Object 제외
                .ToArray();



            Debug.Log($"UIScene [{uiScene.GetType().Name}] 찾음. (protected 메서드 {sceneMethods.Length}개)");
        }



        private void InvokeUISceneMethod(MethodInfo method)
        {
            try
            {
                var parameters = method.GetParameters();
                object[] args = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    // 기본형 타입에 따른 임시 값 생성
                    if (parameters[i].ParameterType == typeof(int)) args[i] = 0;
                    else if (parameters[i].ParameterType == typeof(float)) args[i] = 0f;
                    else if (parameters[i].ParameterType == typeof(string)) args[i] = "";
                    else args[i] = null;
                }

                method.Invoke(uiScene, args);
                Debug.Log($"Protected 메서드 호출됨: {method.Name}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"메서드 실행 실패: {e.Message}");
            }
        }
    }
}