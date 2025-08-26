using UnityEngine;
using System.Collections.Generic;

public class ObjectHolder : MonoBehaviour
{
    [SerializeField] private List<GameObject> heldObjects = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        foreach (GameObject obj in heldObjects)
        {
            DontDestroyOnLoad(obj);
        }
    }
}
