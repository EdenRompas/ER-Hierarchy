using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ERHierarchy
{
    // Tempelkan komponen ini pada GameObject supaya barisnya digambar sebagai Header
    // (baris solid + judul teks tebal) di Hierarchy window. Tidak ada logic runtime,
    // murni penanda untuk keperluan organisasi di Editor.
    [DisallowMultipleComponent]
    public class ERHeader : MonoBehaviour
    {
    }

#if UNITY_EDITOR

    [InitializeOnLoad]
    public static class ERHeaderHierarchy
    {
        private static readonly Color BackgroundColor = new Color(0.3f, 0.3f, 0.3f, 1f);

        static ERHeaderHierarchy()
        {
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += DrawHeader;
        }

        private static void DrawHeader(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null || gameObject.GetComponent<ERHeader>() == null) return;

            EditorGUI.DrawRect(selectionRect, BackgroundColor);

            var style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            EditorGUI.LabelField(selectionRect, gameObject.name.ToUpperInvariant(), style);
        }

        #region Context Menu

        [MenuItem("GameObject/ER Hierarchy/Add Header", false, 10)]
        private static void AddHeader(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            if (target == null || target.GetComponent<ERHeader>() != null) return;

            Undo.AddComponent<ERHeader>(target);
        }

        [MenuItem("GameObject/ER Hierarchy/Add Header", true)]
        private static bool ValidateAddHeader(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            return target != null && target.GetComponent<ERHeader>() == null;
        }

        #endregion
    }

#endif
}
