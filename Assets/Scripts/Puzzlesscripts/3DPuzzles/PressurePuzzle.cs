using UnityEngine;
using UnityEngine.Events;

public class PressurePuzzle : MonoBehaviour
{
    [SerializeField] private int itemsToSolve = 1;
    private int itemsOnPressurePad = 0;
    [SerializeField] private UnityEvent puzzleSolved;
    [SerializeField] private UnityEvent puzzleUnsolved;


    private void OnTriggerEnter(Collider other)
    {
        print($"items on pressure pad = {itemsOnPressurePad}");
        if(other.CompareTag("PressureItem"))
        {
            itemsOnPressurePad += 1;
        }
        if (itemsOnPressurePad == itemsToSolve)
        {
            SolvePressurePuzzle();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PressureItem"))
        {
            itemsOnPressurePad -= 1;
        }
        if (itemsOnPressurePad == itemsToSolve - 1)
        {
            UnsolvePressurePuzzle();
        }
    }
    private void Update()
    {
        if (itemsOnPressurePad > 1)
        {
            print("ERROR");
        }
    }

    private void SolvePressurePuzzle() { puzzleSolved?.Invoke(); }
    private void UnsolvePressurePuzzle() { puzzleUnsolved?.Invoke(); }
}
