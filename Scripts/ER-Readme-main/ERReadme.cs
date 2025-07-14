using UnityEngine;

[CreateAssetMenu(fileName = "README", menuName = "Readme/New Readme", order = 1)]
public class ERReadme : ScriptableObject
{
    [SerializeField] public string Title;

    [TextArea(10, 40)]
    [SerializeField] public string Description;
}