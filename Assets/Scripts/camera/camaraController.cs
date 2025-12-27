using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camaraController : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;
    public Transform nearBackground;
    public bool StopFollow = false;
    private Vector3 lastPos;
    private Vector3 CameraSpeed;
    public float minHeight;
    public float maxHeight;
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!StopFollow)
        {
            CameraSpeed = transform.position - lastPos;

            
            nearBackground.position+= new Vector3(CameraSpeed.x*0.5f, CameraSpeed.y * 0.3f, 0f);


            lastPos = transform.position;
        }
    }
}
