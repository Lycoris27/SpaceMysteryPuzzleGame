using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class PipeSection : MonoBehaviour
{
    [SerializeField] public GameObject activeObj;
    [SerializeField] public bool isStart = false;
    [SerializeField] public bool isEnd = false;
    [SerializeField] public int numConn;
    [SerializeField] public float rot;
    [SerializeField] public List<int> conn;
    [SerializeField] public bool accessed; //for DFS
    [SerializeField] public bool locked; //for Locking specific pipes
}
