#if UNITY_EDITOR
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using static PlasticPipe.PlasticProtocol.Messages.Serialization.ItemHandlerMessagesSerialization;

namespace Editor.KDH_IMSI
{
    public class AutoUIRename
    {
        //private static int[] _buttonN = new int[5], _textN = new int[5], _imageN = new int[5], _gameObjectN = new int[5];
        [MenuItem("Tools/Auto Setup/UI_Rename")]
        public static void InitializeScene()
        {
            //for (int i = 0; i < 5; i++)
            //{
            //    _buttonN[i] = -1;
            //    _textN[i] = -1;
            //    _imageN[i] = -1;
            //    _gameObjectN[i] = -1;
            //}
            Transform UI_Root = GameObject.Find("@UI_Root").transform;
            if (UI_Root == null)
            {
                Debug.Log("@UI_RootÀÌ ¾øÀ½");
                return;
            }
            foreach (Transform t in UI_Root)
            {
                Rename(t);
                //for (int i = 1; i < 5; i++)
                //{
                //    _buttonN[i] = -1;
                //    _textN[i] = -1;
                //    _imageN[i] = -1;
                //    _gameObjectN[i] = -1;
                //}
            }
        }
        private static void Rename(Transform node, int index=0, string s = null)
        {
            int i = 0;
            if (s==null)
            {
                s = $"{node.name}";
                for (int j= 0; j < 3; j++)
                {
                    Match match = Regex.Match(node.name, @" ?\((\d+)\)$");
                    Match match2 = Regex.Match(node.name, @" ?_(\d+)$");
                    if (match.Success == true && int.TryParse(match.Groups[1].Value, out int result))
                    {
                        i += result;
                        s = Regex.Replace(s, @" ?\((\d+)\)$", "");
                        node.name= Regex.Replace(node.name, @" ?\((\d+)\)$", "");
                    }
                    else if (match2.Success == true && int.TryParse(match2.Groups[1].Value, out result))
                    {
                        i += result;
                        s = Regex.Replace(s, @" ?_(\d+)", "");
                        node.name= Regex.Replace(node.name, @" ?_(\d+)", "");
                    }
                    else
                    {
                        break;
                    }
                }
                node.name = $"{s}_{index + i}";
                //if (check == true)
                //{
                //    s = node.name;
                //}

            }
            node.name = $"{s}_{index+i}";
            s = node.name;
            //if (node.TryGetComponent<Button>(out _))
            //{
            //    s += 
            //}
            //else if (node.TryGetComponent<Image>(out _))
            //{
            //    s += "Image";
            //}
            //else if (node.TryGetComponent<TextMeshProUGUI>(out _))
            //{
            //    s += "Text";
            //}
            //else
            //{
            //    s += "GameObject";
            //}
            int k = 0;
            foreach (Transform t in node)
            {
                Rename(t, k, $"{s}");
                k++;
            }
        }
    }
}
#endif