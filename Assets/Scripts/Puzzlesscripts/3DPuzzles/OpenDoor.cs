using UnityEngine;
using System.Collections;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] private float openHeight = 3f;   // How far the door moves upward
    [SerializeField] private float speed = 2f;
    [SerializeField] private GameObject door;

    private Coroutine moveRoutine;
    private Vector3 closedPosition;

    void Start()
    {
        // Remember the door's starting (closed) position
        closedPosition = door.transform.localPosition;
    }

    public void OnDoorOpen()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        // Target is the closed position + upward offset
        Vector3 target = closedPosition + new Vector3(0, openHeight, 0);
        moveRoutine = StartCoroutine(MoveDoor(target));
    }

    public void OnDoorClose()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        // Return to the closed position
        moveRoutine = StartCoroutine(MoveDoor(closedPosition));
    }

    private IEnumerator MoveDoor(Vector3 destination)
    {
        while (Vector3.Distance(door.transform.localPosition, destination) > 0.01f)
        {
            door.transform.localPosition = Vector3.MoveTowards(
                door.transform.localPosition,
                destination,
                speed * Time.deltaTime
            );
            yield return null;
        }

        // Snap to final position
        door.transform.localPosition = destination;
        moveRoutine = null;
    }
}
