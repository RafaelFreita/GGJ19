using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSpawn : MonoBehaviour
{

    public RespawnObject respawn;
    // Start is called before the first frame update
    void Awake()
    {
        respawn.spawnPoint = transform;
    }
}
