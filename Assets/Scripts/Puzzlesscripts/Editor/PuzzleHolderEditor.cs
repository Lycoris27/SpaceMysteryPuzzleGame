using UnityEngine;
using UnityEditor;
using System.Linq;
using Puzzles;

[CustomEditor(typeof(Puzzles.PuzzleHolder))]
public class PuzzleHolderEditor : Editor
{
    private string prefabFolder = "Assets/Objects/Puzzles"; // <-- change to your prefab folder
    private GameObject[] prefabs;
    private string[] prefabNames;
    private int selectedIndex;

    private void OnEnable()
    {
        // Find all prefabs in the folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { prefabFolder });
        prefabs = guids.Select(guid => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid))).ToArray();
        prefabNames = prefabs.Select(p => p.name).ToArray();
    }

    public override void OnInspectorGUI()
    {
        PuzzleHolder holder = (PuzzleHolder)target;

        // Find current index
        selectedIndex = Mathf.Max(0, System.Array.IndexOf(prefabs, holder.selectedPuzzle));

        // Show dropdown
        int newIndex = EditorGUILayout.Popup("Select Puzzle", selectedIndex, prefabNames);
        if (newIndex != selectedIndex)
        {
            selectedIndex = newIndex;
            holder.selectedPuzzle = prefabs[selectedIndex];
            //holder.selectedPuzzleName = prefabNames[selectedIndex];
            EditorUtility.SetDirty(holder); // mark as dirty so it saves
        }

        DrawDefaultInspector();
    }
}
