using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ElectricityConzolePuzzleSolver : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite defaultSprite;   // default sprite
    [SerializeField] private Sprite sprite02;       // hover sprite
    [SerializeField] private Sprite curveSprite;    // curve sprite
    [SerializeField] private Sprite straightSprite; // straight sprite

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth = 4;
    [SerializeField] private int gridHeight = 4;
    [SerializeField] private GameObject[,] gridArray;

    [Header("Locked Objects")]
    [SerializeReference] private List<List<Vector2Int>> lockedObjs = new();
    [SerializeField] private int connections;

    private Image objImage;
    private bool nodeHeld;
    private bool foundEnd = false;

    private void OnEnable()
    {
        NodeScript.PointerUpPing += OnPointerUp;
        NodeScript.PointerDownPing += OnPointerDown;
        NodeScript.PointerEnterPing += OnPointerEnter;
        NodeScript.PointerExitPing += OnPointerExit;
    }

    private void OnDisable()
    {
        NodeScript.PointerUpPing -= OnPointerUp;
        NodeScript.PointerDownPing -= OnPointerDown;
        NodeScript.PointerEnterPing -= OnPointerEnter;
        NodeScript.PointerExitPing -= OnPointerExit;
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
        if (gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode())
        {
            // loop backwards so we can safely remove
            if (lockedObjs.Count != 0)
            {
                for (int i = lockedObjs.Count - 1; i >= 0; i--)
                {
                    if (lockedObjs[i].Contains(pos))
                    {
                        foreach (Vector2Int storedPos in lockedObjs[i])
                        {
                            RevertSpriteToDefault(storedPos);
                        }

                        lockedObjs.RemoveAt(i); // remove the entire sublist
                        break; // done, exit loop
                    }
                }
            }

            // add a brand new sublist starting with pos
            lockedObjs.Add(new List<Vector2Int> { pos });
            nodeHeld = true;
            foundEnd = false;
        }
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
        Vector2Int lastPos = currentSublist[currentSublist.Count - 1];
        Vector2Int firstPos = currentSublist[0];

        // Get the GameObject at that grid position
        GameObject lastNodeObj = gridArray[lastPos.x, lastPos.y];
        GameObject firstNodeObj = gridArray[firstPos.x, firstPos.y];

        // Get the NodeScript component
        NodeScript lastNodeScript = lastNodeObj.GetComponent<NodeScript>();
        NodeScript firstNodeScript = firstNodeObj.GetComponent<NodeScript>();

        // Check if it is a major node
        bool isMajor = lastNodeScript.ReceiveIsMajorNode();
        int lastMajorID = lastNodeScript.ReceiveMajorNodeID();
        int firstMajorID = firstNodeScript.ReceiveMajorNodeID();


        //Debug.Log($"Checking last node at {lastPos} | Name: {lastNodeObj.name} | Major? {isMajor}");

        // Check if the last node in the current sublist is a major node
        if (currentSublist.Count > 0 && isMajor && lastMajorID == firstMajorID && firstPos != lastPos)
        {

            if(connections == lockedObjs.Count)
            {
                print("win!");
            }
        }
        else
        {
            print($"firstMajorID is: {firstMajorID} & lastmajorID is: {lastMajorID}, therefore Denied");

            // Revert all nodes in the current sublist
            foreach (Vector2Int storedPos in currentSublist)
            {
                RevertSpriteToDefault(storedPos); // pass the position or object
            }

            // Remove the last sublist entirely
            lockedObjs.RemoveAt(lockedObjs.Count - 1);
        }

        nodeHeld = false;
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
        var currentSublist = lockedObjs[lockedObjs.Count - 1];
        if (Vector2Int.Distance(currentSublist[currentSublist.Count-1], pos) > 1 && !currentSublist.Contains(pos))
        {
            return;
        }
        if(gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode())
        {
            if (gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveMajorNodeID() != gridArray[currentSublist[0].x, currentSublist[0].y].GetComponent<NodeScript>().ReceiveMajorNodeID())
            {
                print("returning");
                return;
            }
        }

        // Backtracking/removal
        if (currentSublist.Contains(pos))
        {
            for (int i = currentSublist.Count - 1; i >= 0; i--)
            {
                if (currentSublist[i] == pos)
                {
                    if (!gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode())
                    {
                        // this needs to be changed
                        Image posImage = gridArray[pos.x, pos.y].GetComponent<Image>();
                        posImage.sprite = sprite02;
                        float angle = FindSpriteRotation(currentSublist[i - 1], pos);
                        posImage.transform.rotation = Quaternion.Euler(0, 0, angle);

                    }
                    break;
                }

                RevertSpriteToDefault(currentSublist[i]);
                currentSublist.RemoveAt(i);
            }
            foundEnd = false;
        }
        if (!foundEnd)
        {
            // Extending the path
            if (currentSublist.Count > 0 &&
                Vector2Int.Distance(currentSublist[currentSublist.Count - 1], pos) == 1 &&
                !currentSublist.Contains(pos))
            {

                bool majorNode = gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode();

                ChangeNodeSprite(pos);
                currentSublist.Add(pos);
                print($"added pos {pos}");

                int currentID = gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveMajorNodeID();
                int firstID = gridArray[currentSublist[0].x, currentSublist[0].y].GetComponent<NodeScript>().ReceiveMajorNodeID();
                if (firstID == currentID && majorNode)
                {
                    foundEnd = true; // stop adding after hitting a major node
                }
            }
        }
        
    }
    private void ChangeNodeSprite(Vector2Int pos)
    {
        NodeScript posNodeScript = gridArray[pos.x, pos.y].GetComponent<NodeScript>();



        Vector2Int lastPos = lockedObjs[lockedObjs.Count - 1][lockedObjs[lockedObjs.Count - 1].Count - 1];
        
        float angle = FindSpriteRotation(lastPos, pos);
        Image lastImage = gridArray[pos.x, pos.y].GetComponent<Image>();
        if (!posNodeScript.ReceiveIsMajorNode()) { lastImage.sprite = sprite02; }
        gridArray[pos.x, pos.y].transform.rotation = Quaternion.Euler(0, 0, angle);

        NodeScript lastNodeScript =  gridArray[lastPos.x, lastPos.y].GetComponent<NodeScript>();

        if (!lastNodeScript.ReceiveIsMajorNode())
        {
            Image lastPosImage = gridArray[lastPos.x, lastPos.y].GetComponent<Image>();
            Image posImage = gridArray[pos.x, pos.y].GetComponent<Image>();

            if (lastPosImage.transform.rotation == posImage.transform.rotation)
            {
                lastPosImage.sprite = straightSprite;
            }
            else
            {
                lastPosImage.sprite = curveSprite;

                float currentZ = posImage.transform.eulerAngles.z;
                float prevZ = lastPosImage.transform.eulerAngles.z;

                if ((prevZ == 270 && currentZ == 0) ||
                (prevZ == 0 && currentZ == 90) ||
                (prevZ == 90 && currentZ == 180) ||
                (prevZ == 180 && currentZ == 270))
                {
                    lastPosImage.transform.rotation = Quaternion.Euler(0, 0, prevZ - 90f);
                }

                    print($"EEEEEEEEEEEE \n lastPosImage rotation = {lastPosImage.transform.eulerAngles.z} \n posImage rotation = {posImage.transform.eulerAngles.z}" );
            }
        }


        //print(angle);
    }
    private void OnPointerExit(Vector2Int pos)
    {

    }
    private void RevertSpriteToDefault(Vector2Int pos)
    {
        if (!gridArray[pos.x, pos.y].GetComponent<NodeScript>().ReceiveIsMajorNode())
        {
            Image curImage = gridArray[pos.x, pos.y].GetComponent<Image>();
            curImage.sprite = defaultSprite;
            gridArray[pos.x, pos.y].transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private float FindSpriteRotation(Vector2Int previousPosition, Vector2Int currentPosition)
    {
        Vector2Int direction = currentPosition - previousPosition;

        // Custom mapping: (1,0) = up, (0,1) = right
        float angleRad = Mathf.Atan2(-direction.y, direction.x);
        float angleDeg = angleRad * Mathf.Rad2Deg;

        // Normalize to 0–360
        if (angleDeg < 0) angleDeg += 360f;

        return angleDeg;
    }


    /*
    private void OnPointerDown(Vector2Int pos, GameObject obj)
    {
        // Reset all locked objects to default
        foreach (Vector2Int lockedObj in lockedObjs)
        {
            RevertSpriteToDefault(lockedObj);
        }
        lockedObjs.Clear();
        lockedObjs.Add(pos);
        currentPos = pos;
        heldNodeStart = obj;
        nodeHeld = true;
    }

    private void OnPointerUp(Vector2Int pos, GameObject obj)
    {

        heldNodeStart = null;
        nodeHeld = false;
    }

    private void OnPointerEnter(Vector2Int pos, GameObject obj, bool isMajor)
    {
        if (!nodeHeld) return;

        // Ignore major nodes unless specified
        if (isMajor) return;

        // Check distance from previous position
        if (Vector2Int.Distance(pos, previousPos) > 1) return;

        currentPos = pos;

        objImage = obj.GetComponent<Image>();
        objImage.sprite = sprite02;

        float angle = FindSpriteRotation(previousPos, pos);
        obj.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (isPreviousMajor) return;

        // Update the previous node's sprite/rotation
        ResolvePreviousSprite(objImage, previousObj.GetComponent<Image>());
    }

    private void OnPointerExit(Vector2Int pos, GameObject obj, bool isMajor)
    {
        previousPos = pos;
        previousObj = obj;
        isPreviousMajor = isMajor;
    }



    private void ResolvePreviousSprite(Image currentImage, Image prevImage)
    {
        float currentZ = currentImage.transform.eulerAngles.z;
        float prevZ = prevImage.transform.eulerAngles.z;

        if (Mathf.Approximately(currentZ, prevZ))
        {
            prevImage.sprite = straightSprite;
        }
        else
        {
            prevImage.sprite = curveSprite;

            // Rotate previous image if a sharp turn is detected
            if ((prevZ == 270 && currentZ == 0) ||
                (prevZ == 0 && currentZ == 90) ||
                (prevZ == 90 && currentZ == 180) ||
                (prevZ == 180 && currentZ == 270))
            {
                prevImage.transform.rotation = Quaternion.Euler(0, 0, prevZ - 90f);
            }
        }
    }

    private void RevertSpriteToDefault(Vector2Int pos)
    {
        Image image = gridArray[pos.x, pos.y].GetComponent<Image>();
        image.sprite = defaultSprite;
        image.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    */
}
