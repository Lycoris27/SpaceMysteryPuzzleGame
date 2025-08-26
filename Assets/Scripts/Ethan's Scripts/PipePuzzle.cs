using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEditor.Animations;
using UnityEngine;

public class PipePuzzle : MonoBehaviour
{
    [SerializeField] public Camera playerCamera; 
    [SerializeField] public int gridx = 10;
    [SerializeField] public int gridz = 10;
    [SerializeField] public float rx = 1.0f;
    [SerializeField] public float rz = 1.0f;
    [SerializeField] public GameObject loadedPrefab; 
    [SerializeField] public GameObject[] grid;

    private void Start()
    {
        playerCamera = GameObject.Find("PlayerCameraRoot").GetComponent<Camera>();

        //Generate the pipe puzzle originating from this objects position
        Vector3 origin = transform.position;

        grid = new GameObject[gridx * gridz];

        for(int i = 0; i < gridz; i++)
        {
            for(int j = 0; j < gridx; j++)
            { 
                int idx = i * gridz + j;
                GameObject newObj = GameObject.Instantiate(loadedPrefab);
                grid[idx] = newObj;

                //Set position
                grid[idx].transform.position = origin + new Vector3(rx*j, 0, rz*i);
                PipeSection sec = grid[idx].GetComponent<PipeSection>();
                sec.playerCamera = playerCamera;
            }
        }

        PipeSection firstgrid = grid[0].GetComponent<PipeSection>();
        firstgrid.isStart = true;

        PipeSection lastgrid = grid[gridx * gridz - 1].GetComponent<PipeSection>();
        lastgrid.isEnd = true;  
    }

    //Depth-First search of the arrays
    public void DFS()
    {

    }

    private void Update()
    {
        
    }
}
