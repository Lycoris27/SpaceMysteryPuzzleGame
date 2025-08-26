using UnityEngine;

public class PipeSection : MonoBehaviour
{
    [SerializeField] public Camera playerCamera;
    [SerializeField] public GameObject activeObj;
    [SerializeField] public bool isStart = false;
    [SerializeField] public bool isEnd = false;
    [SerializeField] public int numConn = 0;
    [SerializeField] public float rot = 0.0f;
    [SerializeField] public float interactionDist = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initState();
    }

    public void initState()
    {
        foreach(Transform child in transform)
        {
            GameObject childObject = child.gameObject;
            childObject.SetActive(false);
        }

        GameObject BasePlate = transform.GetChild(0).gameObject;
        BasePlate.SetActive(true);

        activeObj = transform.GetChild(numConn).gameObject;
        activeObj.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (Transform obj in activeObj.GetComponentInChildren<Transform>())
            {
                //Do Raycast to check main camera staring at object
                RaycastHit hit;
                if (Physics.Raycast(playerCamera.transform.position, (obj.position - playerCamera.transform.position).normalized, out hit, interactionDist))
                {

                }
            }
            
        }
    }
}
