using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "README", menuName = "Readme/New Readme", order = 1)]
public class ERReadme : ScriptableObject
{
    [SerializeField] public string Title;

    [TextArea(10, 40)]
    [SerializeField] public string Description;
}

#if UNITY_EDITOR

[CustomEditor(typeof(ERReadme))]
public class ReadmeAssetEditor : Editor
{
    private bool _isEditMode = false;
    private Vector2 _scrollPos;

    public override void OnInspectorGUI()
    {
        ERReadme readme = (ERReadme)target;

        EditorGUILayout.Space();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (!_isEditMode)
        {
            if (GUILayout.Button("Edit", GUILayout.Width(60)))
                _isEditMode = true;
        }
        else
        {
            if (GUILayout.Button("Done", GUILayout.Width(60)))
            {
                _isEditMode = false;
                EditorUtility.SetDirty(readme);
                AssetDatabase.SaveAssets();
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        if (_isEditMode)
        {
            EditorGUI.BeginChangeCheck();

            readme.Title = EditorGUILayout.TextField("Title", readme.Title);
            EditorGUILayout.LabelField("Description");
            readme.Description = EditorGUILayout.TextArea(readme.Description, GUILayout.MinHeight(150));

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(readme);
            }
        }
        else
        {
            GUIStyle titleStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24
            };

            GUIStyle descriptionStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                richText = true,
                fontSize = 12
            };

            GUILayout.Label(readme.Title, titleStyle);
            GUILayout.Label(readme.Description, descriptionStyle);
        }

        EditorGUILayout.EndScrollView();
    }
}
#endif
