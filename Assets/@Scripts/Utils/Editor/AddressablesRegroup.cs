#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AddressablesFixer
{
    [MenuItem("Tools/Addressables/Force Reload Settings")]
    public static void ForceReloadSettings()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogError("Addressables Settings를 찾을 수 없습니다.");
            return;
        }

        AssetDatabase.Refresh();
        EditorUtility.SetDirty(settings);
        Debug.Log("Addressables Settings 강제 재로드 완료");
    }
}
#endif