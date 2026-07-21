using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ERHierarchy
{
    [DisallowMultipleComponent]
    public class ERHeader : MonoBehaviour
    {
        public enum HeaderColorPreset
        {
            Gray,
            Red,
            Green,
            Blue,
            Yellow
        }

        [SerializeField] private HeaderColorPreset colorPreset = HeaderColorPreset.Gray;

        public Color TintColor => GetColorForPreset(colorPreset);

        public void SetColorPreset(HeaderColorPreset preset) => colorPreset = preset;

        private static Color GetColorForPreset(HeaderColorPreset preset)
        {
            switch (preset)
            {
                case HeaderColorPreset.Gray: return new Color(0.3f, 0.3f, 0.3f, 1f);
                case HeaderColorPreset.Red: return new Color(0.55f, 0.22f, 0.22f, 1f);
                case HeaderColorPreset.Green: return new Color(0.24f, 0.45f, 0.24f, 1f);
                case HeaderColorPreset.Blue: return new Color(0.22f, 0.35f, 0.55f, 1f);
                case HeaderColorPreset.Yellow: return new Color(0.55f, 0.5f, 0.2f, 1f);
                default: return new Color(0.3f, 0.3f, 0.3f, 1f);
            }
        }
    }

#if UNITY_EDITOR
    [InitializeOnLoad]
    public static class ERHeaderHierarchy
    {
        private const int GradientTextureWidth = 64;
        private const float GradientMidPoint = 1f;
        private const float GradientMidOpacity = 0.1f;
        private const float NameOffsetX = 17f;

        private static Texture2D gradientTexture;

        static ERHeaderHierarchy()
        {
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += DrawHeader;
        }

        private static void DrawHeader(EntityId entityId, Rect selectionRect)
        {
            var gameObject = EditorUtility.EntityIdToObject(entityId) as GameObject;
            if (gameObject == null) return;

            var header = gameObject.GetComponent<ERHeader>();
            if (header == null) return;

            Color previousColor = GUI.color;
            GUI.color = header.TintColor;
            GUI.DrawTexture(selectionRect, GetGradientTexture());
            GUI.color = previousColor;

            var nameRect = new Rect(selectionRect.x + NameOffsetX, selectionRect.y, selectionRect.width - NameOffsetX, selectionRect.height);
            EditorGUI.LabelField(nameRect, gameObject.name, EditorStyles.label);
        }

        private static Texture2D GetGradientTexture()
        {
            if (gradientTexture != null) return gradientTexture;

            gradientTexture = new Texture2D(GradientTextureWidth, 1, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            for (int i = 0; i < GradientTextureWidth; i++)
            {
                float t = i / (float)(GradientTextureWidth - 1);
                float alpha = EvaluateGradientAlpha(t);
                gradientTexture.SetPixel(i, 0, new Color(1f, 1f, 1f, alpha));
            }

            gradientTexture.Apply();
            return gradientTexture;
        }

        private static float EvaluateGradientAlpha(float t)
        {
            return t <= GradientMidPoint
                ? Mathf.Lerp(1f, GradientMidOpacity, t / GradientMidPoint)
                : Mathf.Lerp(GradientMidOpacity, 1f, (t - GradientMidPoint) / (1f - GradientMidPoint));
        }

        #region Context Menu

        [MenuItem("GameObject/ER Hierarchy/Add Header/Gray", false, 10)]
        private static void AddHeaderGray(MenuCommand menuCommand) => AddHeader(menuCommand, ERHeader.HeaderColorPreset.Gray);

        [MenuItem("GameObject/ER Hierarchy/Add Header/Red", false, 11)]
        private static void AddHeaderRed(MenuCommand menuCommand) => AddHeader(menuCommand, ERHeader.HeaderColorPreset.Red);

        [MenuItem("GameObject/ER Hierarchy/Add Header/Green", false, 12)]
        private static void AddHeaderGreen(MenuCommand menuCommand) => AddHeader(menuCommand, ERHeader.HeaderColorPreset.Green);

        [MenuItem("GameObject/ER Hierarchy/Add Header/Blue", false, 13)]
        private static void AddHeaderBlue(MenuCommand menuCommand) => AddHeader(menuCommand, ERHeader.HeaderColorPreset.Blue);

        [MenuItem("GameObject/ER Hierarchy/Add Header/Yellow", false, 14)]
        private static void AddHeaderYellow(MenuCommand menuCommand) => AddHeader(menuCommand, ERHeader.HeaderColorPreset.Yellow);

        private static void AddHeader(MenuCommand menuCommand, ERHeader.HeaderColorPreset preset)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            if (target == null || target.GetComponent<ERHeader>() != null) return;

            ERHeader header = Undo.AddComponent<ERHeader>(target);
            header.SetColorPreset(preset);
        }

        [MenuItem("GameObject/ER Hierarchy/Add Header/Gray", true)]
        [MenuItem("GameObject/ER Hierarchy/Add Header/Red", true)]
        [MenuItem("GameObject/ER Hierarchy/Add Header/Green", true)]
        [MenuItem("GameObject/ER Hierarchy/Add Header/Blue", true)]
        [MenuItem("GameObject/ER Hierarchy/Add Header/Yellow", true)]
        private static bool ValidateAddHeader(MenuCommand menuCommand)
        {
            GameObject target = menuCommand.context as GameObject ?? Selection.activeGameObject;
            return target != null && target.GetComponent<ERHeader>() == null;
        }

        #endregion
    }
#endif
}
