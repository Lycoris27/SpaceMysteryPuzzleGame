using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using Unity.Services.Core;
using Unity.VisualScripting;
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
    [SerializeField] public float interactionDist;

    public void updateState(PipeSection sec)
    {
        if (sec.activeObj != null)
        {
            sec.activeObj.SetActive(false);
            sec.transform.GetChild(0).gameObject.SetActive(true);
        }

        sec.activeObj = sec.transform.GetChild(sec.numConn).gameObject;
        sec.activeObj.SetActive(true);

        //Reset rotation
        sec.rot = 0.0f;
        updateRot(sec);
    }

    public void updateRot(PipeSection sec)
    {
        sec.activeObj.transform.eulerAngles = new Vector3(sec.activeObj.transform.rotation.x, sec.rot, sec.activeObj.transform.rotation.z);

        //Update connections list
        sec.conn.Clear();

        /*              0
         *            3 x 1  Connection directions
         *              2
         *              
         *              sec.numConn
         *              - 0 = empty
         *              - 1 = line
         *              - 2 = bend
         *              - 3 = T section
         *              - 4 = all
         *              
         */

        int conn1,conn2,conn3;
        switch(sec.numConn)
        {
            case 0: // Empty
                break;
            case 1: // Straight
                conn1 = (0 + (int)(sec.rot / 90.0f)) % 4;
                conn2 = (2 + (int)(sec.rot / 90.0f)) % 4;
                sec.conn.Add(conn1);
                sec.conn.Add(conn2);
                break;
            case 2: //Bend
                conn1 = (1 + (int)(sec.rot / 90.0f)) % 4;
                conn2 = (2 + (int)(sec.rot / 90.0f)) % 4;
                sec.conn.Add(conn1);
                sec.conn.Add(conn2);
                break;
            case 3:
                conn1 = (0 + (int)(sec.rot / 90.0f)) % 4;
                conn2 = (1 + (int)(sec.rot / 90.0f)) % 4;
                conn3 = (2 + (int)(sec.rot / 90.0f)) % 4;
                sec.conn.Add(conn1);
                sec.conn.Add(conn2);
                sec.conn.Add(conn3);
                break;
            case 4:
                sec.conn.Add(0);
                sec.conn.Add(1);
                sec.conn.Add(2);
                sec.conn.Add(3);
                break;
        }

        Debug.Log(string.Join(" ", sec.conn));
    }

    public void initPipeState(PipeSection sec)
    {
        sec.accessed = false;
        foreach (Transform child in sec.transform)
        {
            GameObject childObject = child.gameObject;
            childObject.SetActive(false);
        }

        GameObject BasePlate = sec.transform.GetChild(0).gameObject;
        BasePlate.SetActive(true);

        updateState(sec);
    }

    private void Start()
    {
        interactionDist = 10.0f;

        playerCamera = GameObject.Find("PlayerCameraRoot").GetComponent<Camera>();

        //Generate the pipe puzzle originating from this objects position
        Vector3 origin = transform.position;

        grid = new GameObject[gridx * gridz];

        for(int i = 0; i < gridx; i++)
        {
            for(int j = 0; j < gridz; j++)
            { 
                int idx = i * gridx + j;
                GameObject newObj = GameObject.Instantiate(loadedPrefab);
                grid[idx] = newObj;

                //Set position
                grid[idx].transform.position = origin + new Vector3(rx*i, 0, rz*j);
                PipeSection sec = grid[idx].GetComponent<PipeSection>();
                sec.numConn = 0;

                initPipeState(sec);
            }
        }

        PipeSection firstgrid = grid[0].GetComponent<PipeSection>();
        firstgrid.isStart = true;
        firstgrid.numConn = 1;
        initPipeState(firstgrid);

        PipeSection lastgrid = grid[gridx * gridz - 1].GetComponent<PipeSection>();
        lastgrid.isEnd = true;
        lastgrid.numConn = 1;
        initPipeState(lastgrid);
    }

    //Depth-First search of the arrays
    public bool DFS(PipeSection pipe, int grididx, int depth)
    {
        pipe.accessed = true;
        if (pipe.isEnd)
        {
            return true;
        }

        Debug.Log("GridID: " + grididx.ToString());
        Debug.Log("List: " + string.Join(" ", pipe.conn));

        //North
        if (grididx >= gridx && pipe.conn.Contains(0))
        {
            //Debug.Log("grididx: " + grididx.ToString() + " Current Connects North");
            GameObject north = grid[grididx - gridx];   
            PipeSection northPS = north.GetComponent<PipeSection>();
            //Debug.Log("North Pipe List: " + "List: " + string.Join(" ", northPS.conn));

            if (northPS.accessed == false && northPS.conn.Contains(2))
            {
                if (DFS(northPS, grididx - gridz, depth + 1)) return true;
            }

        }
        //East
        if ((grididx + 1) % gridx != 0 && pipe.conn.Contains(1))
        {
            Debug.Log("grididx: " + grididx.ToString() + " Current Connects East");
            GameObject east = grid[grididx + 1];
            PipeSection eastPS = east.GetComponent<PipeSection>();  

            if (eastPS.accessed == false && eastPS.conn.Contains(3))
            {
                if(DFS(eastPS, grididx + 1, depth + 1)) return true;
            }
        }

        //South
        if (grididx <= (gridx * gridz - 1) - gridx && pipe.conn.Contains(2))
        {
            Debug.Log("grididx: " + grididx.ToString() + " Current Connects South");
            GameObject south = grid[grididx + gridx];
            PipeSection southPS = south.GetComponent<PipeSection>();

            Debug.Log("South List: " + string.Join(" ", southPS.conn));

            if (southPS.accessed == false && southPS.conn.Contains(0))
            {
                if (DFS(southPS, grididx + gridz, depth + 1)) return true;
            }
        }

        //West
        if (grididx % gridx != 0 && pipe.conn.Contains(3))
        {
            Debug.Log("grididx: " + grididx.ToString() + " Current Connects West");
            GameObject west = grid[grididx - 1];
            PipeSection westPS = west.GetComponent<PipeSection>();

            if (westPS.accessed == false && westPS.conn.Contains(1))
            {
                if (DFS(westPS, grididx - 1, depth + 1)) return true;
            }
        }

        return false;
    }

    private void Update()
    {
        //Change pipe type
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, interactionDist))
            {
                if (hit.collider.gameObject.name == "Base-Plate")
                {
                    PipeSection par = hit.collider.gameObject.GetComponentInParent<PipeSection>();
                    par.numConn += 1;
                    if (par.numConn > 4) par.numConn = 0; //5 different pipes reset after 5th
                    updateState(par);
                }

                if (hit.collider.gameObject.name == "Pressure-Button")
                {
                    GameObject startObj = null;
                    int finalidx = -1;
                    //Loop through the grid to find the start
                    for(int i = 0; i < gridz; i++)
                    {
                        for(int j = 0; j < gridx; j++)
                        {
                            int idx = i * gridz + j;
                            PipeSection tmp = grid[idx].GetComponent<PipeSection>();
                            if (tmp.isStart)
                            {
                                startObj = grid[idx];
                                finalidx = idx;
                            }

                            tmp.accessed = false; //Update all accesses for DFS to false to restart
                        }
                      
                    }
                    if (startObj != null)
                    {
                        if (DFS(startObj.GetComponent<PipeSection>(), finalidx, 0))
                        {
                            Debug.Log("Found End!");
                        }
                        else
                        {
                            Debug.Log("No link to End!");
                        }
                    }
                    else
                    {
                        Debug.Log("Error: Couldn't find start pipe");
                    }
                }
            }
        }

        //Rotate pipe
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, interactionDist))
            {
                if (hit.collider.gameObject.name == "Base-Plate")
                {
                    PipeSection par = hit.collider.gameObject.GetComponentInParent<PipeSection>();
                    par.rot += 90.0f;
                    if (par.rot > 270.0f) par.rot = 0.0f; //4 directions reset after 3rd
                    updateRot(par);
                }
            }
        }
    }
}
