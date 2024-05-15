using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

[InitializeOnLoad]
public static class CustomHierarchy
{
    private static Dictionary<int, Color> objectColors = new Dictionary<int, Color>();
    private static bool isEven = false;

    private static float edgeWidth = 1.0f;
    private static Color edgeColor = new Color(0.46f, 0.46f, 0.46f);
    private static Color highlightedEdgeColor = new Color(0.49f, 0.678f, 0.952f);

    private enum EdgeType
    {
        middleChild,
        lastChild,
        sibling
    }

    static CustomHierarchy()
    {
        /*EditorApplication.hierarchyWindowItemOnGUI += Separator;
        EditorApplication.hierarchyWindowItemOnGUI += AlternativeLine;
        EditorApplication.hierarchyChanged += UpdateAlternativeLine;
        EditorApplication.hierarchyWindowItemOnGUI += Icon;
        EditorApplication.hierarchyWindowItemOnGUI += Toggle;
        EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyTree;
        */
    }

    public static void ToggleEventListeners(bool activate)
    {
        if (activate)
        {
            EditorApplication.hierarchyWindowItemOnGUI += Separator;
            EditorApplication.hierarchyWindowItemOnGUI += AlternativeLine;
            EditorApplication.hierarchyChanged += UpdateAlternativeLine;
            EditorApplication.hierarchyWindowItemOnGUI += Icon;
            EditorApplication.hierarchyWindowItemOnGUI += DrawHierarchyTree;
        }
        else
        {
            EditorApplication.hierarchyWindowItemOnGUI -= Separator;
            EditorApplication.hierarchyWindowItemOnGUI -= AlternativeLine;
            EditorApplication.hierarchyChanged -= UpdateAlternativeLine;
            EditorApplication.hierarchyWindowItemOnGUI -= Icon;
            EditorApplication.hierarchyWindowItemOnGUI -= DrawHierarchyTree;
        }
    }

    #region Alternative Line Hierarchy

    private static void AlternativeLine(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null)
        {
            if (!objectColors.ContainsKey(instanceID))
            {
                objectColors[instanceID] = isEven ? new Color(0.3f, 0.3f, 0.3f, 0.1f) : Color.clear;
                isEven = !isEven;
            }

            Color alternatingColor = objectColors[instanceID];
            EditorGUI.DrawRect(selectionRect, alternatingColor);
        }
    }

    private static void UpdateAlternativeLine()
    {
        objectColors.Clear();
        isEven = false;
    }

    #endregion

    #region Icon Hierarchy

    static void Icon(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null)
        {
            Texture2D icon = null;

            MonoBehaviour[] scripts = gameObject.GetComponents<MonoBehaviour>();
            if (scripts.Length > 0)
            {
                foreach (MonoBehaviour script in scripts)
                {
                    if (script != null && !IsScript(script))
                    {
                        icon = AssetPreview.GetMiniThumbnail(MonoScript.FromMonoBehaviour(script));
                        if (icon != null)
                            break;
                    }
                    else if (gameObject.GetComponent<Button>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(Button)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<Slider>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(Slider)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<GridLayoutGroup>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(GridLayoutGroup)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<HorizontalLayoutGroup>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(HorizontalLayoutGroup)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<VerticalLayoutGroup>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(VerticalLayoutGroup)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<Image>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(Image)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<TextMeshProUGUI>() != null)
                    {
                        icon = AssetPreview.GetMiniThumbnail(gameObject.GetComponent<TextMeshProUGUI>());
                    }
                    else if (gameObject.GetComponent<Text>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(Text)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<Canvas>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(Canvas)).image as Texture2D;
                    }
                    else if (gameObject.GetComponent<EventSystem>() != null)
                    {
                        icon = EditorGUIUtility.ObjectContent(null, typeof(EventSystem)).image as Texture2D;
                    }
                }
            }
            else
            {
                if (gameObject.GetComponent<Rigidbody>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(Rigidbody)).image as Texture2D;
                }
                if (gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(Rigidbody2D)).image as Texture2D;
                }
                else if (gameObject.GetComponent<Light>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(Light)).image as Texture2D;
                }
                else if (gameObject.GetComponent<MeshFilter>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(MeshFilter)).image as Texture2D;
                }
                else if (gameObject.GetComponent<BoxCollider>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(BoxCollider)).image as Texture2D;
                }
                else if (gameObject.GetComponent<CapsuleCollider>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(CapsuleCollider)).image as Texture2D;
                }
                else if (gameObject.GetComponent<SphereCollider>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(SphereCollider)).image as Texture2D;
                }
                else if (gameObject.GetComponent<MeshCollider>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(MeshCollider)).image as Texture2D;
                }
                else if (gameObject.GetComponent<SpriteRenderer>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(SpriteRenderer)).image as Texture2D;
                }
                else if (gameObject.GetComponent<BoxCollider2D>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(BoxCollider2D)).image as Texture2D;
                }
                else if (gameObject.GetComponent<CapsuleCollider2D>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(CapsuleCollider2D)).image as Texture2D;
                }
                else if (gameObject.GetComponent<CircleCollider2D>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(CircleCollider2D)).image as Texture2D;
                }
                else if (gameObject.GetComponent<Camera>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(Camera)).image as Texture2D;
                }
                else if (gameObject.GetComponent<ParticleSystem>() != null)
                {
                    icon = EditorGUIUtility.ObjectContent(null, typeof(ParticleSystem)).image as Texture2D;
                }
                else if (gameObject.GetComponents<Component>().Length == 1)
                {
                    icon = null;
                }
            }

            if (icon != null)
            {
                GUI.DrawTexture(new Rect(selectionRect.xMax - 16, selectionRect.yMin, 16, 16), icon);
            }
        }
    }

    static bool IsScript(MonoBehaviour script)
    {
        return script.GetType().Namespace != null && script.GetType().Namespace != "UnityEngine";
    }

    #endregion

    #region Separator Hierarchy

    static void Separator(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.name.StartsWith("---", System.StringComparison.Ordinal))
        {
            /*Rect backgroundRect = new Rect(selectionRect)
            {
                x = 0,
                width = Screen.width
            };*/

            EditorGUI.DrawRect(selectionRect, new Color(0.3f, 0.3f, 0.3f, 1));

            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;

            EditorGUI.LabelField(selectionRect, gameObject.name.Replace("-", "").ToUpperInvariant(), style);
        }
    }

    #endregion

    #region Tree Hierarchy

    private static Color GetColor(bool highlighted)
    {
        if (highlighted)
        {
            return highlightedEdgeColor;
        }
        else
        {
            return edgeColor;
        }
    }

    private static float CalculateRectXValue(Rect rect, int graphDistance)
    {
        return rect.x - 21.5f - graphDistance * (rect.height - 2);
    }

    private static void DrawFullVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
    {
        EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, edgeWidth, rect.height),
            GetColor(highlighted));
    }

    private static void DrawHalfVerticalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
    {
        EditorGUI.DrawRect(new Rect(CalculateRectXValue(rect, graphDistance), rect.y, edgeWidth, rect.height / 2),
            GetColor(highlighted));
    }

    private static void DrawHorizontalEdgeSegment(Rect rect, int graphDistance, bool highlighted)
    {
        EditorGUI.DrawRect(
            new Rect(CalculateRectXValue(rect, graphDistance), rect.y + rect.height / 2, rect.height / 2,
                edgeWidth),
            GetColor(highlighted));
    }

    private static void DrawHierarchyEdge(EdgeType edgeType, bool highlighted, int graphDistance, Rect rect)
    {
        switch (edgeType)
        {
            case EdgeType.sibling:
                DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                break;
            case EdgeType.lastChild:
                DrawHalfVerticalEdgeSegment(rect, graphDistance, highlighted);
                DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                break;
            case EdgeType.middleChild:
                DrawFullVerticalEdgeSegment(rect, graphDistance, highlighted);
                DrawHorizontalEdgeSegment(rect, graphDistance, highlighted);
                break;
        }
    }

    private static void DrawHierarchyTree(int instanceID, Rect selectionRect)
    {
        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (go != null)
        {
            // Draw children (
            if (go.transform.parent != null)
            {
                if (go.transform.GetSiblingIndex() < go.transform.parent.childCount - 1)
                {
                    if (Selection.activeTransform != null &&
                        go.transform.parent.IsChildOf(Selection.activeTransform))
                    {
                        DrawHierarchyEdge(EdgeType.middleChild, true, 0, selectionRect);
                    }
                    else
                    {
                        DrawHierarchyEdge(EdgeType.middleChild, false, 0, selectionRect);
                    }
                }
                else
                {
                    if (Selection.activeTransform != null &&
                        go.transform.parent.IsChildOf(Selection.activeTransform))
                    {
                        DrawHierarchyEdge(EdgeType.lastChild, true, 0, selectionRect);
                    }
                    else
                    {
                        DrawHierarchyEdge(EdgeType.lastChild, false, 0, selectionRect);
                    }
                }

                var referenceTransform = go.transform.parent;
                var currentDistance = 1;

                // Draw ancestors with open sibling relations
                while (referenceTransform.parent != null)
                {
                    if (referenceTransform.GetSiblingIndex() < referenceTransform.parent.childCount - 1)
                    {
                        if (Selection.activeTransform != null &&
                            referenceTransform.parent.IsChildOf(Selection.activeTransform))
                        {
                            DrawHierarchyEdge(EdgeType.sibling, true, currentDistance, selectionRect);
                        }
                        else
                        {
                            DrawHierarchyEdge(EdgeType.sibling, false, currentDistance, selectionRect);
                        }
                    }

                    referenceTransform = referenceTransform.parent;
                    currentDistance++;
                }
            }
        }
    }

    #endregion
}

#endif