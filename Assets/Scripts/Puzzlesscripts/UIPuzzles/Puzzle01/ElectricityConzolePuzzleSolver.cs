using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ElectricityConzolePuzzleSolver : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;   // default sprite
    [SerializeField] private Sprite hoverSprite;     // hover sprite
    [SerializeField] private Sprite curveSprite;     // curve sprite
    [SerializeField] private Sprite straightSprite;  // straight sprite

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;
    [SerializeField] private GameObject[,] gridArray;

    [Header("Locked Objects")]
    [SerializeReference] private List<List<Vector2Int>> lockedObjs = new();
    [SerializeField] private int connections;
    [SerializeField] private bool nodeHeld;
    [SerializeField] private bool foundEnd = false;

    private void OnEnable()
    {
        NodeScript.PointerUpPing += OnPointerUp;
        NodeScript.PointerDownPing += OnPointerDown;
        NodeScript.PointerEnterPing += OnPointerEnter;
    }

    private void OnDisable()
    {
        NodeScript.PointerUpPing -= OnPointerUp;
        NodeScript.PointerDownPing -= OnPointerDown;
        NodeScript.PointerEnterPing -= OnPointerEnter;
    }

    private void Start()
    {
        gridArray = new GameObject[gridWidth, gridHeight];

        int childIndex = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (childIndex >= transform.childCount) break;

                Transform child = transform.GetChild(childIndex);
                gridArray[x, y] = child.gameObject;
                child.GetComponent<NodeScript>().SetPosition(new Vector2Int(x, y));
                //Debug.Log($"Placed {child.name} at [{x},{y}]");
                childIndex++;
            }
        }
    }

    private void OnPointerDown(Vector2Int pos)
    {
        var node = gridArray[pos.x, pos.y].GetComponent<NodeScript>();
        //If you press on a minor node, don't continue
        if (!node.ReceiveIsMajorNode())
            return;
        // loop backwards so we can safely remove
        // 🔹 Look for an existing path containing this position
        for (int i = lockedObjs.Count - 1; i >= 0; i--)
        {
            if (!lockedObjs[i].Contains(pos)) continue;

            // revert sprites for this path
            foreach (var storedPos in lockedObjs[i])
                RevertSpriteToDefault(storedPos);

            lockedObjs.RemoveAt(i);
            break;
        }
        // add a brand new sublist starting with pos
        lockedObjs.Add(new List<Vector2Int> { pos });
        nodeHeld = true;
        foundEnd = false;
    }
    private void OnPointerUp(Vector2Int pos)
    {
        // Make sure there's at least one sublist
        if (lockedObjs.Count == 0)
        {
            nodeHeld = false;
            return;
        }

        // Get the last sublist
        var currentSublist = lockedObjs[lockedObjs.Count - 1];

        // Get the last position in the current path
        Vector2Int lastPos = currentSublist[^1];
        Vector2Int firstPos = currentSublist[0];

        // Get the NodeScript component
        NodeScript lastNodeScript = gridArray[lastPos.x, lastPos.y].GetComponent<NodeScript>();
        NodeScript firstNodeScript = gridArray[firstPos.x, firstPos.y].GetComponent<NodeScript>();

        // Check if it is a major node
        bool isMajor = lastNodeScript.ReceiveIsMajorNode();
        int lastMajorID = lastNodeScript.ReceiveMajorNodeID();
        int firstMajorID = firstNodeScript.ReceiveMajorNodeID();

        // Check if the last node in the current sublist is a major node
        if (currentSublist.Count > 0 && isMajor && lastMajorID == firstMajorID && firstPos != lastPos)
        {
            if (connections == lockedObjs.Count)
            {
                print("win!");
            }
        }
        else
        {
            // Revert all nodes in the current sublist
            RevertPath(currentSublist);
            // Remove the last sublist entirely
            lockedObjs.RemoveAt(lockedObjs.Count - 1);
        }
        nodeHeld = false;
    }
    private void RevertPath(List<Vector2Int> path)
    {
        foreach (var pos in path)
            RevertSpriteToDefault(pos);
    }
    private void OnPointerEnter(Vector2Int pos)
    { 
        // 🔹 Nothing to do if there are no sublists
        if (lockedObjs.Count == 0 || !nodeHeld)
            return;

        // 🔹 Check every sublist except the last one
        for (int i = 0; i < lockedObjs.Count - 1; i++)
        {
            if (lockedObjs[i].Contains(pos))
            {
                return; // pos belongs to an older sublist, ignore
            }
        }
        // 🔹 Active path: operate on the last sublist
        var currentSublist = lockedObjs[^1];
        var lastPos = currentSublist[^1];
        var NodeScript = gridArray[pos.x, pos.y].GetComponent<NodeScript>();

        if (Vector2Int.Distance(lastPos, pos) > 1 && !currentSublist.Contains(pos))
            return;
        
        if(NodeScript.ReceiveIsMajorNode())
        {
            var firstNodeScript = gridArray[currentSublist[0].x, currentSublist[0].y].GetComponent<NodeScript>();
            if (NodeScript.ReceiveMajorNodeID() != firstNodeScript.ReceiveMajorNodeID())
                return;
        }

        // Backtracking/removal
        if (currentSublist.Contains(pos))
        {
            for (int i = currentSublist.Count - 1; i >= 0; i--)
            {
                if (currentSublist[i] == pos)
                {
                    if (!NodeScript.ReceiveIsMajorNode())
                    {
                        // this needs to be changed
                        Image posImage = gridArray[pos.x, pos.y].GetComponent<Image>();
                        posImage.sprite = hoverSprite;
                        float angle = FindSpriteRotation(currentSublist[i - 1], pos);
                        posImage.transform.rotation = Quaternion.Euler(0, 0, angle);

                    }
                    break;
                }

                RevertSpriteToDefault(currentSublist[i]);
                currentSublist.RemoveAt(i);
            }
            foundEnd = false;
            return;
        }
        if (!foundEnd && Vector2Int.Distance(lastPos, pos) == 1 && !currentSublist.Contains(pos))
        {
            // Extending the path
            bool majorNode = gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode();

            ChangeNodeSprite(pos);
            currentSublist.Add(pos);

            int firstID = gridArray[currentSublist[0].x, currentSublist[0].y].GetComponent<NodeScript>().ReceiveMajorNodeID();
            if (firstID == NodeScript.ReceiveMajorNodeID() && majorNode)
            {
                foundEnd = true; // stop adding after hitting a major node
            }

        }
        
    }
    private void ChangeNodeSprite(Vector2Int pos)
    {
        var currentNode = gridArray[pos.x, pos.y];
        var currentNodeScript = currentNode.GetComponent<NodeScript>();
        var currentImage = currentNode.GetComponent<Image>();

        var lastSublist = lockedObjs[^1];
        Vector2Int lastPos = lastSublist[^1];
        var lastNode = gridArray[lastPos.x, lastPos.y];
        var lastNodeScript = lastNode.GetComponent<NodeScript>();
        var lastImage = lastNode.GetComponent<Image>();

        float angle = FindSpriteRotation(lastPos, pos);
        
        if (!currentNodeScript.ReceiveIsMajorNode()) { currentImage.sprite = hoverSprite; }
        currentNode.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (!lastNodeScript.ReceiveIsMajorNode())
        {
            if (lastImage.transform.rotation == currentImage.transform.rotation)
            {
                lastImage.sprite = straightSprite;
            }
            else
            {
                lastImage.sprite = curveSprite;

                float currentZ = currentImage.transform.eulerAngles.z;
                float prevZ = lastImage.transform.eulerAngles.z;

                if ((prevZ == 270 && currentZ == 0) ||
                (prevZ == 0 && currentZ == 90) ||
                (prevZ == 90 && currentZ == 180) ||
                (prevZ == 180 && currentZ == 270))
                {
                    lastImage.transform.rotation = Quaternion.Euler(0, 0, prevZ - 90f);
                }
            }
        }
    }
    private void RevertSpriteToDefault(Vector2Int pos)
    {
        var node = gridArray[pos.x, pos.y];
        var nodeScript = node.GetComponent<NodeScript>();

        if (nodeScript.ReceiveIsMajorNode())
            return;

        var image = node.GetComponent<Image>();
        image.sprite = defaultSprite;
        node.transform.rotation = Quaternion.identity;
    }
    private float FindSpriteRotation(Vector2Int previousPosition, Vector2Int currentPosition)
    {
        Vector2Int direction = currentPosition - previousPosition;
        float angle = Mathf.Atan2(-direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }
}
