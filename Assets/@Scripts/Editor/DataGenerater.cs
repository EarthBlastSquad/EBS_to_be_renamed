using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
namespace Editor
{
    public class DataLoaderGeneratorWindow : EditorWindow
    {
        private string sourceFilePath = "Assets/@Scripts/Data/Datas.cs";
        private string outputDirectory = "Assets/@Scripts/Data/Loaders";
        private string dataManagerOutputPath = "Assets/@Scripts/Manager/Core/DataManager.cs";
        private Vector2 scrollPosition;
        private List<ClassInfo> detectedClasses = new List<ClassInfo>();
        private string previewCode = "";
        private bool generateDataManager = true;

        [MenuItem("Tools/데이터 파싱(DataClasses수정)/Data Loader Generator(데이터를 파싱합니다.)")]
        public static void ShowWindow()
        {
            GetWindow<DataLoaderGeneratorWindow>("Data Loader Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Unity Data Loader Generator", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 파일 경로 설정
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Source File:", GUILayout.Width(100));
            sourceFilePath = EditorGUILayout.TextField(sourceFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.OpenFilePanel("Select Source File", "Assets", "cs");
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
                string path = EditorUtility.OpenFolderPanel("Select Output Directory", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    outputDirectory = GetRelativePath(path);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // DataManager 생성 옵션
            generateDataManager = EditorGUILayout.Toggle("Generate DataManager Code", generateDataManager);

            if (generateDataManager)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("DataManager Path:", GUILayout.Width(120));
                dataManagerOutputPath = EditorGUILayout.TextField(dataManagerOutputPath);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            // 분석 버튼
            if (GUILayout.Button("Analyze Source File", GUILayout.Height(30)))
            {
                AnalyzeSourceFile();
            }

            EditorGUILayout.Space();

            // 검출된 클래스 목록
            if (detectedClasses.Count > 0)
            {
                GUILayout.Label($"Detected Classes ({detectedClasses.Count}):", EditorStyles.boldLabel);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
                foreach (var classInfo in detectedClasses)
                {
                    EditorGUILayout.BeginHorizontal();
                    classInfo.isSelected = EditorGUILayout.Toggle(classInfo.isSelected, GUILayout.Width(20));
                    EditorGUILayout.LabelField(classInfo.className, GUILayout.Width(150));

                    // Key 필드 선택
                    EditorGUILayout.LabelField("Key Field:", GUILayout.Width(70));
                    if (classInfo.fields.Count > 0)
                    {
                        int currentIndex = classInfo.fields.IndexOf(classInfo.keyField);
                        if (currentIndex < 0) currentIndex = 0;

                        string[] fieldNames = classInfo.fields.ToArray();
                        int newIndex = EditorGUILayout.Popup(currentIndex, fieldNames, GUILayout.Width(120));
                        classInfo.keyField = fieldNames[newIndex];
                    }

                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndScrollView();

                EditorGUILayout.Space();

                // 전체 선택/해제
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    detectedClasses.ForEach(c => c.isSelected = true);
                }
                if (GUILayout.Button("Deselect All"))
                {
                    detectedClasses.ForEach(c => c.isSelected = false);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                // 미리보기
                if (GUILayout.Button("Preview Code"))
                {
                    GeneratePreview();
                }

                if (!string.IsNullOrEmpty(previewCode))
                {
                    EditorGUILayout.LabelField("Preview:", EditorStyles.boldLabel);

                    // 스크롤뷰로 미리보기 표시
                    scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(250));
                    EditorGUILayout.TextArea(previewCode, GUILayout.ExpandHeight(true));
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.Space();

                // 생성 버튼
                if (GUILayout.Button("Generate Loader Classes", GUILayout.Height(40)))
                {
                    GenerateLoaderClasses();
                }
            }
        }

        private void AnalyzeSourceFile()
        {
            detectedClasses.Clear();
            previewCode = "";

            if (!File.Exists(sourceFilePath))
            {
                EditorUtility.DisplayDialog("Error", $"File not found: {sourceFilePath}", "OK");
                return;
            }

            string content = File.ReadAllText(sourceFilePath);

            // public class ClassName { ... } 패턴 매칭
            string classPattern = @"public\s+class\s+(\w+Data)\s*\{([^}]*(?:\{[^}]*\}[^}]*)*)\}";
            MatchCollection classMatches = Regex.Matches(content, classPattern, RegexOptions.Singleline);

            foreach (Match match in classMatches)
            {
                string className = match.Groups[1].Value;
                string classBody = match.Groups[2].Value;

                // 클래스 내부의 public 필드 추출
                List<string> fields = new List<string>();
                Dictionary<string, string> fieldTypes = new Dictionary<string, string>();

                string fieldPattern = @"public\s+(\w+(?:<\w+>)?)\s+(\w+)";
                MatchCollection fieldMatches = Regex.Matches(classBody, fieldPattern);

                foreach (Match fieldMatch in fieldMatches)
                {
                    string fieldType = fieldMatch.Groups[1].Value;
                    string fieldName = fieldMatch.Groups[2].Value;

                    // 모든 필드 타입 저장
                    fieldTypes[fieldName] = fieldType;

                    // List<T> 타입이 아닌 필드만 키로 사용 가능
                    if (!fieldType.StartsWith("List"))
                    {
                        fields.Add(fieldName);
                    }
                }

                if (fields.Count > 0)
                {
                    ClassInfo classInfo = new ClassInfo
                    {
                        className = className,
                        fields = fields,
                        fieldTypes = fieldTypes,
                        keyField = fields[0], // 기본값: 첫 번째 필드
                        isSelected = true
                    };

                    detectedClasses.Add(classInfo);
                }
            }

            if (detectedClasses.Count == 0)
            {
                EditorUtility.DisplayDialog("Info", "No Data classes detected.\nMake sure your classes are named like 'XXXData'.", "OK");
            }
            else
            {
                Debug.Log($"Detected {detectedClasses.Count} classes");
            }
        }

        private void GeneratePreview()
        {
            if (detectedClasses.Count == 0) return;

            StringBuilder preview = new StringBuilder();

            var firstSelected = detectedClasses.Find(c => c.isSelected);
            if (firstSelected != null)
            {
                preview.AppendLine("=== Loader Class Preview ===");
                preview.AppendLine(GenerateLoaderCode(firstSelected));
            }

            if (generateDataManager)
            {
                preview.AppendLine();
                preview.AppendLine("=== DataManager Preview ===");
                preview.AppendLine(GenerateDataManagerPreview());
            }

            previewCode = preview.ToString();
        }

        private void GenerateLoaderClasses()
        {
            var selectedClasses = detectedClasses.FindAll(c => c.isSelected);

            if (selectedClasses.Count == 0)
            {
                EditorUtility.DisplayDialog("Warning", "No classes selected!", "OK");
                return;
            }

            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            // 모든 Loader 클래스를 하나의 파일에 생성
            StringBuilder allCode = new StringBuilder();

            allCode.AppendLine("using System;");
            allCode.AppendLine("using System.Collections.Generic;");
            allCode.AppendLine("using Data;");
            allCode.AppendLine();

            foreach (var classInfo in selectedClasses)
            {
                string loaderCode = GenerateLoaderCodeWithoutUsings(classInfo);
                allCode.AppendLine(loaderCode);
                allCode.AppendLine();
            }

            string filePath = Path.Combine(outputDirectory, "DataLoaders.cs");
            File.WriteAllText(filePath, allCode.ToString());

            // DataManager 생성
            if (generateDataManager)
            {
                GenerateDataManagerCode(selectedClasses);
            }

            AssetDatabase.Refresh();

            string message = $"Generated {selectedClasses.Count} loader class(es) in:\n{filePath}";
            if (generateDataManager)
            {
                message += $"\n\nDataManager code generated in:\n{dataManagerOutputPath}";
            }

            EditorUtility.DisplayDialog("Success", message, "OK");
        }

        private string GenerateLoaderCode(ClassInfo classInfo)
        {
            StringBuilder sb = new StringBuilder();

            string keyType = classInfo.fieldTypes[classInfo.keyField];
            string loaderClassName = classInfo.className.Replace("Data", "") + "DataLoader";
            string listVariableName = GetListVariableName(classInfo.className);

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using Data;");
            sb.AppendLine();
            sb.AppendLine("[Serializable]");
            sb.AppendLine($"public class {loaderClassName} : ILoader<{keyType}, {classInfo.className}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public List<{classInfo.className}> {listVariableName} = new List<{classInfo.className}>();");
            sb.AppendLine();
            sb.AppendLine($"    public Dictionary<{keyType}, {classInfo.className}> MakeDict()");
            sb.AppendLine("    {");
            sb.AppendLine($"        Dictionary<{keyType}, {classInfo.className}> dict = new Dictionary<{keyType}, {classInfo.className}>();");
            sb.AppendLine($"        foreach ({classInfo.className} data in {listVariableName})");
            sb.AppendLine("        {");
            sb.AppendLine($"            dict.Add(data.{classInfo.keyField}, data);");
            sb.AppendLine("        }");
            sb.AppendLine("        return dict;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string GenerateLoaderCodeWithoutUsings(ClassInfo classInfo)
        {
            StringBuilder sb = new StringBuilder();

            string keyType = classInfo.fieldTypes[classInfo.keyField];
            string loaderClassName = classInfo.className.Replace("Data", "") + "DataLoader";
            string listVariableName = GetListVariableName(classInfo.className);

            sb.AppendLine("[Serializable]");
            sb.AppendLine($"public class {loaderClassName} : ILoader<{keyType}, {classInfo.className}>");
            sb.AppendLine("{");
            sb.AppendLine($"    public List<{classInfo.className}> {listVariableName} = new List<{classInfo.className}>();");
            sb.AppendLine();
            sb.AppendLine($"    public Dictionary<{keyType}, {classInfo.className}> MakeDict()");
            sb.AppendLine("    {");
            sb.AppendLine($"        Dictionary<{keyType}, {classInfo.className}> dict = new Dictionary<{keyType}, {classInfo.className}>();");
            sb.AppendLine($"        foreach ({classInfo.className} data in {listVariableName})");
            sb.AppendLine("        {");
            sb.AppendLine($"            dict.Add(data.{classInfo.keyField}, data);");
            sb.AppendLine("        }");
            sb.AppendLine("        return dict;");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GenerateDataManagerCode(List<ClassInfo> selectedClasses)
        {
            string existingContent = "";
            bool fileExists = File.Exists(dataManagerOutputPath);

            if (fileExists)
            {
                existingContent = File.ReadAllText(dataManagerOutputPath);
            }

            StringBuilder newDictionaries = new StringBuilder();
            StringBuilder newInitCalls = new StringBuilder();

            // 새로 추가할 Dictionary 선언과 Init 호출 생성
            foreach (var classInfo in selectedClasses)
            {
                string keyType = classInfo.fieldTypes[classInfo.keyField];
                string dicName = classInfo.className.Replace("Data", "") + "Dic";
                string loaderName = classInfo.className.Replace("Data", "") + "DataLoader";
                string dataName = classInfo.className.Replace("Data", "") + "Data";

                // 이미 존재하는지 확인
                if (!existingContent.Contains($"{dicName} {{"))
                {
                    newDictionaries.AppendLine($"       public Dictionary<{keyType}, {classInfo.className}> {dicName} {{ get; private set; }} = new Dictionary<{keyType}, {classInfo.className}>();");
                }

                string initCall = $"    {dicName} = LoadJson<{loaderName}, {keyType}, {classInfo.className}>(\"{dataName}\").MakeDict();";
                if (!existingContent.Contains(initCall))
                {
                    newInitCalls.AppendLine($"        {initCall}");
                }
            }

            string finalContent;

            if (fileExists)
            {
                // 기존 파일 업데이트
                finalContent = existingContent;

                // Dictionary 추가
                if (newDictionaries.Length > 0)
                {
                    // public class DataManager 뒤에 추가
                    string classPattern = @"(public\s+class\s+DataManager\s*\{)";
                    Match match = Regex.Match(finalContent, classPattern);
                    if (match.Success)
                    {
                        int insertPos = match.Index + match.Length;
                        finalContent = finalContent.Insert(insertPos, "\n" + newDictionaries.ToString());
                    }
                }

                // Init 메서드에 추가
                if (newInitCalls.Length > 0)
                {
                    string initPattern = @"(public\s+void\s+Init\s*\(\s*\)\s*\{)";
                    Match match = Regex.Match(finalContent, initPattern);
                    if (match.Success)
                    {
                        int insertPos = match.Index + match.Length;
                        finalContent = finalContent.Insert(insertPos, "\n" + newInitCalls.ToString());
                    }
                    else
                    {
                        // Init 메서드가 없으면 클래스 끝에 추가
                        int lastBrace = finalContent.LastIndexOf('}');
                        if (lastBrace > 0)
                        {
                            string initMethod = "\n    public void Init()\n    {\n" + newInitCalls.ToString() + "    }\n";
                            finalContent = finalContent.Insert(lastBrace, initMethod);
                        }
                    }
                }
            }
            else
            {
                // 새 파일 생성
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("using System.Collections.Generic;");
                sb.AppendLine("using Data;");
                sb.AppendLine();
                sb.AppendLine("public class DataManager");
                sb.AppendLine("{");
                sb.Append(newDictionaries.ToString());
                sb.AppendLine();
                sb.AppendLine("    public void Init()");
                sb.AppendLine("    {");
                sb.Append(newInitCalls.ToString());
                sb.AppendLine("    }");
                sb.AppendLine("}");

                finalContent = sb.ToString();
            }

            string directoryPath = Path.GetDirectoryName(dataManagerOutputPath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            File.WriteAllText(dataManagerOutputPath, finalContent);
        }

        private string GenerateDataManagerPreview()
        {
            var selectedClasses = detectedClasses.FindAll(c => c.isSelected);
            if (selectedClasses.Count == 0) return "";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("// Dictionary declarations:");
            foreach (var classInfo in selectedClasses.Take(3))
            {
                string keyType = classInfo.fieldTypes[classInfo.keyField];
                string dicName = classInfo.className.Replace("Data", "") + "Dic";
                sb.AppendLine($"public Dictionary<{keyType}, {classInfo.className}> {dicName} {{ get; private set; }} = new Dictionary<{keyType}, {classInfo.className}>();");
            }

            if (selectedClasses.Count > 3)
                sb.AppendLine("// ... more dictionaries");

            sb.AppendLine();
            sb.AppendLine("// Init method:");
            foreach (var classInfo in selectedClasses.Take(3))
            {
                string keyType = classInfo.fieldTypes[classInfo.keyField];
                string dicName = classInfo.className.Replace("Data", "") + "Dic";
                string loaderName = classInfo.className.Replace("Data", "") + "DataLoader";
                string dataName = classInfo.className.Replace("Data", "") + "Data";

                sb.AppendLine($"{dicName} = LoadJson<{loaderName}, {keyType}, {classInfo.className}>(\"{dataName}\").MakeDict();");
            }

            if (selectedClasses.Count > 3)
                sb.AppendLine("// ... more init calls");

            return sb.ToString();
        }

        private string GetListVariableName(string className)
        {
            // WaveData -> waves
            // LevelData -> levels
            // ItemData -> items
            string baseName = className.Replace("Data", "").ToLower();

            if (baseName.EndsWith("s") || baseName.EndsWith("x") || baseName.EndsWith("ch") || baseName.EndsWith("sh"))
                return baseName + "es";
            else if (baseName.EndsWith("y"))
                return baseName.Substring(0, baseName.Length - 1) + "ies";
            else
                return baseName + "s";
        }

        private string GetRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            return absolutePath;
        }

        [Serializable]
        private class ClassInfo
        {
            public string className;
            public List<string> fields = new List<string>();
            public Dictionary<string, string> fieldTypes = new Dictionary<string, string>();
            public string keyField;
            public bool isSelected;
        }
    }
}