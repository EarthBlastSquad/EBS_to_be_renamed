using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    public static class EditorUtil
    {

        private static string GetRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            return absolutePath;
        }

        public static void ShowDirUI(ref string sourceFilePath, ref string outputDirectory)
        {
            // 파일 경로 설정
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Source File:", GUILayout.Width(100));
            sourceFilePath = EditorGUILayout.TextField(sourceFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFilePanel("Select Source File", "Assets/@Resources/Json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    sourceFilePath = GetRelativePath(path);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 출력 디렉토리 설정
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Directory:", GUILayout.Width(100));
            outputDirectory = EditorGUILayout.TextField(outputDirectory);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFilePanel("Select Output Directory", "Assets/@Resources/Json", "json");
                if (!string.IsNullOrEmpty(path))
                {
                    outputDirectory = GetRelativePath(path);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}