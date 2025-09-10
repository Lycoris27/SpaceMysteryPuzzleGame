using UnityEngine;
using UnityEngine.EventSystems;
using System;


public class NodeScript : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Vector2Int position;
    [SerializeField] private bool isMajorNode;
    [SerializeField] private int majorNodeID;
    public bool isHighlighted;
    public bool isPressed;

    public static event Action<Vector2Int> PointerUpPing;
    public static event Action<Vector2Int> PointerDownPing;
    public static event Action<Vector2Int> PointerEnterPing;
    public static event Action<Vector2Int> PointerExitPing;

    public void OnPointerEnter(PointerEventData eventdata) { PointerEnterPing?.Invoke(position); }
    public void OnPointerDown(PointerEventData eventdata) { PointerDownPing?.Invoke(position); }
    public void OnPointerUp(PointerEventData eventdata) { PointerUpPing?.Invoke(position); }
    public void SetPosition(Vector2Int newPosition) { position = newPosition; }
    public bool ReceiveIsMajorNode() { return isMajorNode; }
    public int ReceiveMajorNodeID() { return majorNodeID; }
}
