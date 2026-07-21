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
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI += ActiveToggle;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI += DrawHierarchyTree;
            }
            else
            {
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= AlternativeLine;
                EditorApplication.hierarchyChanged -= UpdateAlternativeLine;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= Icon;
                EditorApplication.hierarchyWindowItemByEntityIdOnGUI -= ActiveToggle;
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

        #region Hierarchy Marker Helpers

        private static bool HasHierarchyMarker(GameObject gameObject) =>
            gameObject.GetComponent<ERSeparator>() != null || gameObject.GetComponent<ERHeader>() != null;

        #endregion

        #region Icon Hierarchy

        private const float IconSize = 14f;
        private const float IconGap = 1f;
        private const float IconGapLeft = 4f;
        private const float IconGapWithName = 12f;
        private const float IconOpacity = 0.6f;
        private const float IconTopGap = 1f;

        private static readonly List<Component> componentBuffer = new List<Component>(8);
        private static readonly List<Texture2D> iconBuffer = new List<Texture2D>(8);
        private static readonly GUIContent nameContent = new GUIContent();

        private static void Icon(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null || HasHierarchyMarker(gameObject)) return;

            CollectComponentIcons(gameObject, iconBuffer);
            if (iconBuffer.Count == 0) return;

            nameContent.text = gameObject.name;
            float nameWidth = EditorStyles.label.CalcSize(nameContent).x;

            float rightEdge = selectionRect.xMax - ToggleWidth - IconGapLeft;
            float leftBound = selectionRect.x + nameWidth + IconGapLeft + IconGapWithName;
            float availableWidth = rightEdge - leftBound;

            int maxIcons = Mathf.Max(0, Mathf.FloorToInt((availableWidth + IconGap) / (IconSize + IconGap)));
            int count = Mathf.Min(iconBuffer.Count, maxIcons);

            Color previousColor = GUI.color;
            GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, IconOpacity);

            float x = rightEdge - count * (IconSize + IconGap);
            for (int i = 0; i < count; i++)
            {
                GUI.DrawTexture(new Rect(x, selectionRect.yMin + IconTopGap, IconSize, IconSize), iconBuffer[i]);
                x += IconSize + IconGap;
            }

            GUI.color = previousColor;
        }

        private static void CollectComponentIcons(GameObject gameObject, List<Texture2D> result)
        {
            result.Clear();
            gameObject.GetComponents(componentBuffer);

            foreach (Component component in componentBuffer)
            {
                if (component is MonoBehaviour script && IsUserScript(script) && !IsHierarchyMarker(script))
                {
                    Texture2D icon = GetScriptIcon(script);
                    if (icon != null) result.Add(icon);
                }
            }

            foreach (Component component in componentBuffer)
            {
                if (component == null || component is Transform) continue;

                if (component is MonoBehaviour behaviour && (IsUserScript(behaviour) || IsHierarchyMarker(behaviour)))
                    continue;

                Texture2D icon = AssetPreview.GetMiniThumbnail(component);
                if (icon != null) result.Add(icon);
            }
        }

        private static Texture2D GetScriptIcon(MonoBehaviour script)
        {
            MonoScript monoScript = MonoScript.FromMonoBehaviour(script);
            return monoScript != null ? AssetPreview.GetMiniThumbnail(monoScript) : null;
        }

        private static bool IsUserScript(MonoBehaviour script)
        {
            string assemblyName = script.GetType().Assembly.GetName().Name;
            return !assemblyName.StartsWith("Unity");
        }

        private static bool IsHierarchyMarker(MonoBehaviour script) => script is ERHeader || script is ERSeparator;

        #endregion

        #region Active Toggle Hierarchy

        private const float ToggleWidth = 16f;
        private const float ToggleOpacity = 0.8f;
        private const float ToggleNameAndIconGap = 16f;

        private static void ActiveToggle(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null || HasHierarchyMarker(gameObject)) return;

            nameContent.text = gameObject.name;
            float nameWidth = EditorStyles.label.CalcSize(nameContent).x;
            float leftBound = selectionRect.x + nameWidth + ToggleNameAndIconGap;
            float toggleLeft = selectionRect.xMax - ToggleWidth;

            if (toggleLeft < leftBound) return;

            Color previousColor = GUI.color;
            GUI.color = new Color(previousColor.r, previousColor.g, previousColor.b, ToggleOpacity);

            var toggleRect = new Rect(toggleLeft, selectionRect.yMin, ToggleWidth, 16);
            bool isActive = EditorGUI.Toggle(toggleRect, gameObject.activeSelf);

            if (isActive != gameObject.activeSelf)
            {
                Undo.RecordObject(gameObject, "Toggle Active State");
                gameObject.SetActive(isActive);
            }

            GUI.color = previousColor;
        }

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
