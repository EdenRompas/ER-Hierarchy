using UnityEngine;
using UnityEditor;
using EREditor.Hierarchy;

namespace EREditor.HierarchyWindow
{

#if UNITY_EDITOR

    [InitializeOnLoad]
    public class CustomHierarchyWindow : EditorWindow
    {
        private static bool isActive = false;

        static CustomHierarchyWindow()
        {
            isActive = PlayerPrefs.GetInt("CustomHierarchyWindow_isActive") == 1;
            CustomHierarchy.ToggleEventListeners(isActive);
        }

        [MenuItem("Tools/ER Hierarchy")]
        static void OpenWindow()
        {
            CustomHierarchyWindow window = GetWindow<CustomHierarchyWindow>("ER Hierarchy");

            window.minSize = new Vector2(100, 200);
            window.maxSize = new Vector2(200, 300);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            bool newActiveState = EditorGUILayout.ToggleLeft("Enable", isActive);

            if (newActiveState != isActive)
            {
                isActive = newActiveState;
                CustomHierarchy.ToggleEventListeners(isActive);

                PlayerPrefs.SetInt("CustomHierarchyWindow_isActive", isActive ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }

#endif
}
