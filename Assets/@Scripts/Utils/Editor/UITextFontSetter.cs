using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
namespace Utils.Editor
{
    public class UITextFontSetter
    {
        public const string PATH_FONT_UITEXT_SAMLIB = "Assets/@Resources/ChosunCentennial_otf2/ChosunCentennial_otf SDF.asset";

        [MenuItem("Tools/FontChanger(현재 폰트를 교체합니다.)")]
        public static void ChageFontInUIText()
        {
            GameObject[] rootObj = GetSceneRootObjects();

            for (int i = 0; i < rootObj.Length; i++)
            {
                GameObject gbj = (GameObject)rootObj[i];
                Component[] com = gbj.GetComponentsInChildren(typeof(TextMeshProUGUI), true);
                foreach (TextMeshProUGUI txt in com)
                {
                    txt.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(PATH_FONT_UITEXT_SAMLIB);
                }
            }
        }

        private static GameObject[] GetSceneRootObjects()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            return currentScene.GetRootGameObjects();
        }
    }
}