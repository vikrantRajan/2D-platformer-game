using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Script for parallax background
/// </summary>

public class ParallaxBackground : MonoBehaviour
{
    public float speed_x;
    public float speed_y;

    private Camera main_camera;
    private RawImage img;
    //private Renderer render;

    void Start()
    {
        main_camera = TheCamera.Get().GetCamera();
        img = GetComponent<RawImage>();
        //render = GetComponent<Renderer>();
    }

    void Update()
    {
        Camera cam = main_camera.GetComponent<Camera>();
        img.uvRect = new Rect(cam.transform.position.x * speed_x, cam.transform.position.y * speed_y, img.uvRect.width, img.uvRect.height);
        //transform.position = new Vector3(cam.transform.position.x * speed, transform.position.y, transform.position.z);

    }
}