using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PipePuzzle : MonoBehaviour
{
    public int gridx = 10;
    public int gridz = 10;
    public GameObject loadedPrefab; 
    public GameObject[] grid;

    private void Start()
    { 
        //Generate the pipe puzzle originating from this objects position
        Vector3 origin = transform.position;

        grid = new GameObject[gridx * gridz];

        for(int i = 0; i < gridz; i++)
        {
            for(int j = 0; j < gridx; j++)
            { 
                int idx = i * gridz + j;
                Instantiate(loadedPrefab);
                grid[idx] = loadedPrefab;
                Renderer rend = grid[idx].GetComponent<Renderer>();

                //Set position
                grid[idx].transform.position = origin + new Vector3(rend.bounds.size.x*j, 0, rend.bounds.size.z*i);
            }
        }
    }

    private void Update()
    {
        
    }
}
