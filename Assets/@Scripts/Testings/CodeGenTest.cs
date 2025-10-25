

using Tools;
using UnityEditor;
using UnityEngine;

public class GenTest
{
    [MenuItem("Testings/Tool")]
    public static void Fun()
    {
        CodeGeneratorTool tool = new CodeGeneratorTool(128, false);
        Debug.Log(tool.LoadTemplate(@"E:\unityproject\EBS_to_be_renamed\Assets\Testings\Test.txt", "UI"));
        Debug.Log(tool.ReplacePlaceHolderWith("testname", "__[TEST_NAME]__"));
        Debug.Log(tool.GenerateCodeFile(@"E:\unityproject\EBS_to_be_renamed\Assets\Testings\Test.cs"));

    }
}