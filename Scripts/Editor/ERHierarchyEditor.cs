using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ERHierarchy
{
#if UNITY_EDITOR

    [InitializeOnLoad]
    public static class ERHierarchyEditor
    {
        private static readonly Dictionary<EntityId, Color> objectColors = new Dictionary<EntityId, Color>();
        private static bool isEven;

        private const float EdgeWidth = 1.0f;
        private static readonly Color EdgeColor = new Color(0.46f, 0.46f, 0.46f);
        private static readonly Color HighlightedEdgeColor = new Color(0.49f, 0.678f, 0.952f);

        private enum EdgeType
        {
            MiddleChild,
            LastChild,
            Sibling
        }

        public static void ToggleEventListeners(bool activate)
        {
            if (activate)
            {
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI += AlternativeLine;
                EditorApplication.hierarchyChanged += UpdateAlternativeLine;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI += Icon;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI += DrawHierarchyTree;
            }
            else
            {
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= AlternativeLine;
                EditorApplication.hierarchyChanged -= UpdateAlternativeLine;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= Icon;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= DrawHierarchyTree;
            }
        }

        #region Alternative Line Hierarchy

        private static void AlternativeLine(EntityId entityId, Rect selectionRect)
        {
            if (EditorUtility.EntityIdToObject(entityId) is not GameObject) return;

            if (!objectColors.TryGetValue(entityId, out Color color))
            {
                color = isEven ? new Color(0.3f, 0.3f, 0.3f, 0.1f) : Color.clear;
                objectColors[entityId] = color;
                isEven = !isEven;
            }

            EditorGUI.DrawRect(selectionRect, color);
        }

        private static void UpdateAlternativeLine()
        {
            objectColors.Clear();
            isEven = false;
        }

        #endregion

        #region Icon Hierarchy

        private static void Icon(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null) return;

            Texture2D icon = GetBestComponentIcon(gameObject);
            if (icon != null)
                GUI.DrawTexture(new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16), icon);
        }

        // Retrieves the most representative icon from all components attached to the GameObject.
        // User-defined scripts (namespaces outside UnityEngine) are prioritized because they
        // best represent the GameObject's primary purpose in the Hierarchy. If none are found,
        // the icon of any built-in Unity component (such as Rigidbody, Collider, Light, UI, etc.)
        // is automatically retrieved through AssetPreview without requiring each type to be
        // registered manually.
        private static Texture2D GetBestComponentIcon(GameObject gameObject)
        {
            Component[] components = gameObject.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component is MonoBehaviour script && IsUserScript(script) && !IsHierarchyMarker(script))
                {
                    Texture2D icon = AssetPreview.GetMiniThumbnail(script);
                    if (icon != null) return icon;
                }
            }

            foreach (Component component in components)
            {
                if (component == null || component is Transform || component is MonoBehaviour) continue;

                Texture2D icon = AssetPreview.GetMiniThumbnail(component);
                if (icon != null) return icon;
            }

            return null;
        }

        private static bool IsUserScript(MonoBehaviour script)
        {
            string ns = script.GetType().Namespace;
            return ns != null && ns != "UnityEngine";
        }

        // ERHeader/ERSeparator are purely organizational markers and already occupy
        // an entire row by themselves, so their icons do not need to be displayed
        // on the right side of the Hierarchy.
        private static bool IsHierarchyMarker(MonoBehaviour script) => script is ERHeader || script is ERSeparator;

        #endregion

        #region Tree Hierarchy

        private static Color GetColor(bool highlighted) => highlighted ? HighlightedEdgeColor : EdgeColor;

        private static bool IsAncestorSelected(Transform t) =>
            Selection.activeTransform != null && t.IsChildOf(Selection.activeTransform);

        private static float CalculateRectXValue(Rect rect, int graphDistance) =>
            rect.x - 21.5f - graphDistance * (rect.height - 2);

        private static void DrawFullVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, EdgeWidth, rect.height),
                GetColor(highlighted));
        }

        private static void DrawHalfVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, EdgeWidth, rect.height / 2),
                GetColor(highlighted));
        }

        private static void DrawHorizontalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
        {
            EditorGUI.DrawRect(
                new Rect(CalculateRectXValue(rect, graphDistance), rect.y + rect.height / 2, rect.height / 2, EdgeWidth),
                GetColor(highlighted));
        }

        private static void DrawHierarchyEdge(EdgeType edgeType, bool highlighted, int graphDistance, Rect rect)
        {
            switch (edgeType)
            {
                case EdgeType.Sibling:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    break;
                case EdgeType.LastChild:
                    DrawHalfVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    break;
                case EdgeType.MiddleChild:
                    DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                    DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                    break;
            }
        }

        private static void DrawHierarchyTree(EntityId entityId, Rect selectionRect)
        {
            var go = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (go == null || go.transform.parent == null) return;

            Transform parent = go.transform.parent;
            bool isLastChild = go.transform.GetSiblingIndex() == parent.childCount - 1;
            DrawHierarchyEdge(isLastChild ? EdgeType.LastChild : EdgeType.MiddleChild,
                IsAncestorSelected(parent), 0, selectionRect);

            Transform reference = parent;
            int distance = 1;

            while (reference.parent != null)
            {
                if (reference.GetSiblingIndex() < reference.parent.childCount - 1)
                {
                    DrawHierarchyEdge(EdgeType.Sibling, IsAncestorSelected(reference.parent), distance, selectionRect);
                }

                reference = reference.parent;
                distance++;
            }
        }

        #endregion
    }

#endif
}
