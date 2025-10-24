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

    }
}