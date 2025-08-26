using UnityEngine;

namespace Puzzles
{
    public class PuzzleHolder : MonoBehaviour
    {
        // This will be set via the dropdown
        [HideInInspector]
        public GameObject selectedPuzzle;

        // Just to see which was chosen in the inspector
        //public string selectedPuzzleName;
    }
}