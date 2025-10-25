using System;
using System.Collections.Generic;
using System.Text;
using Manager.Core;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using Utils;

namespace Tools
{
    public class CodeGeneratorTool
    {
        private StringBuilder _generatedCode;
        private bool _preservePlaceHolder = true;
        private Dictionary<string, string> _templates = new Dictionary<string, string>();//key : key, value : template

        #region header
        private const string _fileHeader = "CODE_GENERATOR_HEADER";
        private const string _templateTypeKeyword = "template_type";
        private const string _regionSeperator = "__[REG_SEP]__";
        private const string _classGenEntryPoint = "__[ENTRY_POINT]__";
        #endregion

        public CodeGeneratorTool(int initialStringCapacity, bool preservePlaceHolder)
        {
            if (initialStringCapacity < 1)
            {
                initialStringCapacity = 1;
            }
            _generatedCode = new StringBuilder(initialStringCapacity);
            _preservePlaceHolder = preservePlaceHolder;
        }

        public bool IsItCorrectTemplate(string template, string templateType)
        {
            string tester = template.Replace("\r\n", "\n");
            tester = tester.Replace('\r', '\n');

            if (string.IsNullOrEmpty(tester) ||
                string.IsNullOrEmpty(templateType) || 
                tester.Length <= _fileHeader.Length ||
                !tester.StartsWith(_fileHeader, System.StringComparison.Ordinal) ||
                tester[_fileHeader.Length] != '\n' ||
            tester.GetSubstringCount(_classGenEntryPoint) != 1)//header and entry point check
            {
                return false;
            }

            if (string.Compare(tester, _fileHeader.Length + 1, templateType, 0, templateType.Length, StringComparison.Ordinal) != 0 ||
            tester[_fileHeader.Length + templateType.Length] > ' ')//type check
            {
                return false;
            }

            return true;
        }

        public void ClearGenerator(int initialStringCapacity, bool preservePlaceHolder)
        {
            _templates.Clear();
            _generatedCode.Clear();
            _generatedCode.EnsureCapacity(initialStringCapacity);
            _preservePlaceHolder = preservePlaceHolder;
        }

        public bool LoadTemplate(string templatePath, string templateType)
        {
            if (string.IsNullOrEmpty(templatePath) || string.IsNullOrEmpty(templateType))
            {
                return false;
            }

            string template;
            if (Manager.Core.FileIOManager.ReadFromFile(templatePath, out template, Encoding.UTF8) == false)
            {
                return false;
            }

            if (IsItCorrectTemplate(template, templateType) == false)
            {
                return false;
            }


            string[] tokens = template.Split(_regionSeperator);

            if (tokens.Length % 2 == 0)
            {
#if UNITY_EDITOR
                Debug.LogError("worng token count! toekn count must be %2 == 1");
#endif
                return false;
            }

            for (int i = 1; i < tokens.Length; i += 2)
            {
                if (tokens[i + 1].Contains(_classGenEntryPoint))
                {
                    _generatedCode.Append(tokens[i].Replace(_classGenEntryPoint, ""));
                }
                else
                {
                    if (_templates.TryAdd(tokens[i], tokens[i + 1]) == false)
                    {
#if UNITY_EDITOR
                        Debug.LogError("duplicated template key warning!");
#endif
                    }
                }
            }

            return true;
        }
        
        public bool GenerateCodeFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
#if UNITY_EDITOR
                Debug.LogError("path is null or empty");
#endif
                return false;
            }
            
            if (FileIOManager.IsFileExist(filePath))
            {
#if UNITY_EDITOR
                Debug.LogError("file already exist");
#endif
                return false;
            }

            if (_generatedCode.Length <= 0)
            {
#if UNITY_EDITOR
                Debug.LogError("code is empty");
#endif
                return false;
            }

            if (FileIOManager.CreateFile(filePath) == false)
            {
#if UNITY_EDITOR
                Debug.LogError("file creation failed");
#endif
                return false;
            }

            if (FileIOManager.WriteToFile(filePath, _generatedCode.ToString(), true, Encoding.UTF8) == false)
            {
#if UNITY_EDITOR
                Debug.LogError("file write failed");
#endif
                return false;
            }
            
            return true;
        }
    }
}
/*
구조:
첫 두줄:템플릿 정보
1. 템플릿인지에 대한 여부
2. 어떤 타입 템플릿인지에 대한 정보
그 이후로는 키와 값으로 나뉨
각 문자열들을 __[REG_SEP]__을 기준으로 청크단위로 분리되며, 공백도 있는 그대로 들어간다
그래서, 각 청크 사이에는 불필요한 공백은 넣지 않는것이 좋다
*/
/*
CODE_GENERATOR_HEADER
UI
__[REG_SEP]__
button_func_template:
__[REG_SEP]__
public void On__[NAME]__Clicked(PointerEventData data)
{

}
__[REG_SEP]__
test_temp:
__[REG_SEP]__
public __[RET_TYPE]__ __[NAME]__(__[ARGS]__)
{

}
__[REG_SEP]__
entry
__[REG_SEP]__
__[ENTRY_POINT]__public class Test{}
*/