using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour
{
    public bool checkpointIsEnabled;

    void Awake()
    {
        checkpointIsEnabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableCheckpoint()
    {
        if (checkpointIsEnabled)
        {
            checkpointIsEnabled = false;
        }
    }
}
