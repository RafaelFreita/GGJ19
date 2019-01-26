using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowResize : MonoBehaviour
{

    public GameObject handle;
    public GameObject tip;

    [SerializeField]
    private float size = 1;

    private Vector3 handleOriginalScale;
    private Vector3 tipOriginalPos;
    private Vector3 initialDistance;

    private void Start()
    {
        handleOriginalScale = handle.transform.localScale;
        tipOriginalPos = tip.transform.localPosition;
        initialDistance =handle.transform.position - tip.transform.position;
    }

    public float Size
    {
        get { return size; }
        set
        {
            Debug.Log("New size"+ size);
            size = value;
            handle.transform.lossyScale.Set(1, value, 1);
            tip.transform.position.Set(tipOriginalPos.x, tipOriginalPos.y, tipOriginalPos.z);
        }
    }

    private void Update()
    { 
        handle.transform.localScale = new Vector3(handleOriginalScale.x, handleOriginalScale.y * size, handleOriginalScale.z);
        tip.transform.localPosition = new Vector3(tipOriginalPos.x, tipOriginalPos.y, initialDistance.z + handle.transform.localPosition.z - handle.transform.localScale.y);
    }

}
