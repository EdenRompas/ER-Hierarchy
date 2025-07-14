using UnityEngine;
using UnityEditor;
using ERHierarchy;

namespace ERHierarchyWindow
{
#if UNITY_EDITOR

    [InitializeOnLoad]
    public static class ERHierarchyToggleEditor
    {
        private const string PREF_KEY = "CustomHierarchyWindow_isActive";

        static ERHierarchyToggleEditor()
        {
            bool isActive = PlayerPrefs.GetInt(PREF_KEY, 0) == 1;
            ERHierarchyEditor.ToggleEventListeners(isActive);
        }

        [MenuItem("Tools/ER Hierarchy/Enable")]
        public static void EnableHierarchy()
        {
            SetState(true);
        }

        [MenuItem("Tools/ER Hierarchy/Enable", true)]
        public static bool EnableHierarchyValidate()
        {
            Menu.SetChecked("Tools/ER Hierarchy/Enable", PlayerPrefs.GetInt(PREF_KEY, 0) == 1);
            return true;
        }

        [MenuItem("Tools/ER Hierarchy/Disable")]
        public static void DisableHierarchy()
        {
            SetState(false);
        }

        [MenuItem("Tools/ER Hierarchy/Disable", true)]
        public static bool DisableHierarchyValidate()
        {
            Menu.SetChecked("Tools/ER Hierarchy/Disable", PlayerPrefs.GetInt(PREF_KEY, 0) == 0);
            return true;
        }

        private static void SetState(bool isActive)
        {
            ERHierarchyEditor.ToggleEventListeners(isActive);
            PlayerPrefs.SetInt(PREF_KEY, isActive ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log($"[ER Hierarchy] is now {(isActive ? "Enabled" : "Disabled")}");
        }
    }

#endif
}