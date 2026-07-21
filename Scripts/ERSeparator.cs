using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ERHierarchy
{
    // Tempelkan komponen ini pada GameObject supaya barisnya digambar sebagai Separator
    // (garis horizontal tipis, tanpa teks) di Hierarchy window. Tidak ada logic runtime,
    // murni penanda untuk keperluan organisasi di Editor.
    [DisallowMultipleComponent]
    public class ERSeparator : MonoBehaviour
    {
    }

#if UNITY_EDITOR

    [InitializeOnLoad]
    public static class ERSeparatorHierarchy
    {
        private const float LineThickness = 1f;
        private static readonly Color BackgroundColorDark = new Color(0.2196f, 0.2196f, 0.2196f, 1f);
        private static readonly Color BackgroundColorLight = new Color(0.7882f, 0.7882f, 0.7882f, 1f);
        private static readonly Color LineColor = new Color(0.46f, 0.46f, 0.46f);

        static ERSeparatorHierarchy()
        {
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += DrawSeparator;
        }

        private static void DrawSeparator(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null || gameObject.GetComponent<ERSeparator>() == null) return;

            // Tutupi nama asli GameObject supaya baris terlihat bersih
            EditorGUI.DrawRect(selectionRect, EditorGUIUtility.isProSkin ? BackgroundColorDark : BackgroundColorLight);

            // Garis tipis di tengah baris, memanjang penuh secara horizontal
            var lineRect = new Rect(
                selectionRect.x,
                selectionRect.y + selectionRect.height / 2f - LineThickness / 2f,
                selectionRect.width,
                LineThickness);
            EditorGUI.DrawRect(lineRect, LineColor);
        }

        #region Context Menu

        [MenuItem("GameObject/ER Hierarchy/Add Separator", false, 11)]
        private static void AddSeparator(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            if (target == null || target.GetComponent<ERSeparator>() != null) return;

            Undo.AddComponent<ERSeparator>(target);
        }

        [MenuItem("GameObject/ER Hierarchy/Add Separator", true)]
        private static bool ValidateAddSeparator(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            return target != null && target.GetComponent<ERSeparator>() == null;
        }

        #endregion
    }

#endif
}
