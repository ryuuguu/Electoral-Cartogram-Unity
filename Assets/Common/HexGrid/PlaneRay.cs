using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRay : MonoBehaviour
{
    //Attach a cube GameObject in the Inspector before entering Play Mode
    public GameObject m_Cube;

    //This is the distance the clickable plane is from the camera. Set it in the Inspector before running.
    public Vector3 normal;

    public Plane m_Plane;
    public Vector3 hitPoint;
    public float enter;


    void Start() {
        //This is how far away from the Camera the plane is placed
    
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        m_Plane = new Plane(normal, transform.position);
    }

    void Update() {
        //Detect when there is a mouse click
        if (Input.GetMouseButton(0)) {
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            enter = 0.0f;
            m_Plane.Raycast(ray, out enter);
            
            
            if (m_Plane.Raycast(ray, out enter))
            {
                //Get the point that is clicked
                hitPoint = ray.GetPoint(enter);

                //Move your cube GameObject to the point where you clicked
                m_Cube.transform.position = hitPoint;
            }
        }
    }
}