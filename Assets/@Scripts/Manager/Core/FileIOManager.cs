using UnityEngine;
using System.IO;
using System;

namespace Manager.Core
{
    public static class FileIOManager
    {
        /*
        있을 기능들:
        파일이 있는지 검사
        파일 생성/삭제
        파일 읽기/쓰기
        */

        public static bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool DeleteFile(string filePath)
        {
            if (IsFileExist(filePath) == false)
            {
#if UNITY_EDITOR
                Debug.LogError("File not found");
#endif   
                return false;
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception fileException)
            {
#if UNITY_EDITOR
                Debug.LogError($"file delete failed by {fileException.Message}");
#endif   
                return false;
            }
            return true;
        }

        public static bool CreateFile(string filePath)
        {
            if (IsFileExist(filePath))
            {
#if UNITY_EDITOR
                Debug.LogError("File already exists");
#endif   
                return false;
            }

            try
            {
                using (File.Create(filePath)) { }
            }
            catch (Exception fileException)
            {
#if UNITY_EDITOR
                Debug.LogError($"file create failed by {fileException.Message}");
#endif   
                return false;
            }


            return true;
        }

        public static bool ReadFromFile(string filePath, out string contents, System.Text.Encoding encoding)
        {
            if (IsFileExist(filePath) == false)
            {
#if UNITY_EDITOR
                Debug.LogError("File not found");
#endif   
                contents = string.Empty;
                return false;
            }

            try
            {
                contents = File.ReadAllText(filePath, encoding);
            }
            catch (Exception fileException)
            {
#if UNITY_EDITOR
                Debug.LogError($"file read failed by {fileException.Message}");
#endif
                contents = string.Empty;
                return false;
            }

            return true;
        }

        public static bool WriteToFile(string filePath, string contents, bool resetFile, System.Text.Encoding encoding)
        {
            if (IsFileExist(filePath) == false)
            {
#if UNITY_EDITOR
                Debug.LogError("File not found");
#endif
                return false;
            }

            try
            {
                using (StreamWriter writer = new StreamWriter(filePath,!resetFile, encoding))
                {
                    writer.Write(contents);
                }
            }
            catch (Exception fileException)
            {
#if UNITY_EDITOR
                Debug.LogError($"file write failed by {fileException.Message}");
#endif
                return false;
            }

            return true;
        }
    }
}
//assetdatabase써? 말어?
//어차피 유니티에서 처리해주고, 에셋내부에 접근하는건 에디터 환경에서만이고, 이건 런타임에도 써야되니까, 그냥 systemio쓰죠