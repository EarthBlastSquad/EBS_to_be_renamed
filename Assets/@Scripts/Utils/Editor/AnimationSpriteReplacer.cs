#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class AnimationSpriteReplacer : EditorWindow
{
    private AnimationClip sourceClip;
    private string newClipName = "NewClip";

    private List<SpriteReplaceData> replaceDatas = new List<SpriteReplaceData>();

    [MenuItem("Tools/Animation Sprite Replacer")]
    static void Open()
    {
        GetWindow<AnimationSpriteReplacer>("Anim Sprite Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label("Source Animation Clip", EditorStyles.boldLabel);

        sourceClip = (AnimationClip)EditorGUILayout.ObjectField(
            sourceClip,
            typeof(AnimationClip),
            false
        );

        if (sourceClip == null)
            return;

        GUILayout.Space(10);

        newClipName = EditorGUILayout.TextField("New Clip Name", newClipName);

        GUILayout.Space(10);

        if (GUILayout.Button("Load Sprite Tracks"))
        {
            LoadSpriteTracks();
        }

        GUILayout.Space(20);

        if (replaceDatas.Count > 0)
        {
            GUILayout.Label("Sprite Replace", EditorStyles.boldLabel);

            foreach (var data in replaceDatas)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Label(data.originalSprite.name, GUILayout.Width(200));

                data.newSprite = (Sprite)EditorGUILayout.ObjectField(
                    data.newSprite,
                    typeof(Sprite),
                    false
                );

                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Create Replaced Animation"))
            {
                CreateNewClip();
            }
        }
    }

    void LoadSpriteTracks()
    {
        replaceDatas.Clear();

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(sourceClip);

        foreach (var binding in bindings)
        {
            var keyframes = AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);

            foreach (var key in keyframes)
            {
                if (key.value is Sprite sprite)
                {
                    bool alreadyExists = false;

                    foreach (var data in replaceDatas)
                    {
                        if (data.originalSprite == sprite)
                        {
                            alreadyExists = true;
                            break;
                        }
                    }

                    if (!alreadyExists)
                    {
                        replaceDatas.Add(new SpriteReplaceData()
                        {
                            originalSprite = sprite
                        });
                    }
                }
            }
        }
    }

    void CreateNewClip()
    {
        string sourcePath = AssetDatabase.GetAssetPath(sourceClip);

        string folder = System.IO.Path.GetDirectoryName(sourcePath);

        string newPath = folder + "/" + newClipName + ".anim";

        AnimationClip newClip = Instantiate(sourceClip);

        var bindings = AnimationUtility.GetObjectReferenceCurveBindings(newClip);

        foreach (var binding in bindings)
        {
            var keyframes = AnimationUtility.GetObjectReferenceCurve(newClip, binding);

            for (int i = 0; i < keyframes.Length; i++)
            {
                if (keyframes[i].value is Sprite sprite)
                {
                    foreach (var data in replaceDatas)
                    {
                        if (data.originalSprite == sprite && data.newSprite != null)
                        {
                            keyframes[i].value = data.newSprite;
                        }
                    }
                }
            }

            AnimationUtility.SetObjectReferenceCurve(
                newClip,
                binding,
                keyframes
            );
        }

        AssetDatabase.CreateAsset(newClip, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Done",
            "Created: " + newPath,
            "OK"
        );
    }

    [System.Serializable]
    public class SpriteReplaceData
    {
        public Sprite originalSprite;
        public Sprite newSprite;
    }
}
#endif