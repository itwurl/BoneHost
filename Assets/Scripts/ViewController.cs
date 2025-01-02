using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewController : MonoBehaviour
{
    public float FieldOfView;
    public float TranslationSpeed = 100.0F;
    float translation = 0.0F;
    float straffe = 0.0F;

    private BoneController br;

    // Use this for initialization
    void Start ()
    {
        // set the camera's default field of view
        Camera.main.orthographicSize = 250;

        // make the camera's range 3 times as big as widest axis
        Camera.main.farClipPlane = FieldOfView * 3F;
    }

    // Update is called once per frame
    void Update()
    {
        //translation = Input.GetAxis("Vertical") * TranslationSpeed;
        //straffe = Input.GetAxis("Horizontal") * TranslationSpeed;

        //translation *= Time.deltaTime;
        //straffe *= Time.deltaTime;

        //this.transform.Translate(straffe, translation, 0, Space.Self);

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            // forward
            if (Camera.main.orthographicSize > (TranslationSpeed / 10))
                Camera.main.orthographicSize -= (TranslationSpeed / 10);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            // backward
            Camera.main.orthographicSize += (TranslationSpeed / 10);
        }
        else if (UnityEngine.Input.GetMouseButton(2))
        {
            translation -= Input.GetAxis("Mouse Y") * (TranslationSpeed * 10);
            straffe -= Input.GetAxis("Mouse X") * (TranslationSpeed * 10);

            translation *= Time.deltaTime;
            straffe *= Time.deltaTime;

            this.transform.Translate(straffe, translation, 0);
        }
    }

    public void CenterView(Vector3 CenterOfMass, Bounds bounds)
    {
        // field of view should be consider widest bone axis (bone should be aligned with global axis before)
        FieldOfView = Mathf.Max(Mathf.Max(bounds.extents.x, bounds.extents.y), bounds.extents.z);

        // make the camera's field of view as big as widest bone axis
        Camera.main.orthographicSize = FieldOfView * 1.5F;

        // make the camera's range 3 times as big as widest axis
        Camera.main.farClipPlane = FieldOfView * 3F;

        // reset position for Capsule/Camera
        //this.transform.position = new Vector3(bounds.center.x, bounds.center.y, bounds.center.z - (FieldOfView * 1.5F));
        this.transform.position = new Vector3(0, 0, - (FieldOfView * 1.5F));

        // instance of BoneRotation
        br = FindFirstObjectByType(typeof(BoneController)) as BoneController;

        // reset transformation for mesh (rotation and translation) 
        br.transform.rotation = Quaternion.identity;
        br.transform.position = new Vector3(0, 0, 0);
    }

    public void CenterView()
    {
        // make the camera's field of view as big as widest bone axis
        Camera.main.orthographicSize = FieldOfView * 1.5F;

        // reset position for Capsule/Camera
        this.transform.position = new Vector3(0, 0, - (FieldOfView * 1.5F));

        // instance of BoneRotation
        br = FindFirstObjectByType(typeof(BoneController)) as BoneController;

        // reset transformation for mesh (rotation and translation) 
        br.transform.rotation = Quaternion.identity;
        br.transform.position = new Vector3(0, 0, 0);
    }

}
