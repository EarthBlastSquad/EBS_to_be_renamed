#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Editor.KDH_IMSI
{
    public class AutoUIRename
    {
        private static int[] _buttonN = new int[5], _textN = new int[5], _imageN = new int[5], _gameObjectN = new int[5];
        [MenuItem("Tools/Auto Setup/UI_Rename")]
        public static void InitializeScene()
        {
            for (int i = 0; i < 5; i++)
            {
                _buttonN[i] = -1;
                _textN[i] = -1;
                _imageN[i] = -1;
                _gameObjectN[i] = -1;
            }
            Transform UI_Root = GameObject.Find("@UI_Root").transform;
            if (UI_Root == null)
            {
                Debug.Log("@UI_RootÀÌ ¾øÀ½");
                return;
            }
            foreach (Transform t in UI_Root)
            {
                Rename(t, 0);
                for (int i = 1; i < 5; i++)
                {
                    _buttonN[i] = -1;
                    _textN[i] = -1;
                    _imageN[i] = -1;
                    _gameObjectN[i] = -1;
                }
            }
        }
        private static void Rename(Transform node, int index, string s = null)
        {
            if (node.TryGetComponent<Button>(out _))
            {
                s += "Button";
                s += $"_{++_buttonN[index]}";
            }
            else if (node.TryGetComponent<Image>(out _))
            {
                s += "Image";
                s += $"_{++_imageN[index]}";
            }
            else if (node.TryGetComponent<TextMeshProUGUI>(out _))
            {
                s += "Text";
                s += $"_{++_textN[index]}";
            }
            else
            {
                s += "GameObject";
                s += $"_{++_gameObjectN[index]}";
            }
            node.name = s;
            foreach (Transform t in node)
            {
                Rename(t, index + 1, $"{s}_");

            }
        }

    }
}
#endif