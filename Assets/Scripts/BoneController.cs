using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class BoneController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Hello();

    private GameObject Popup;

    private Button AddLandmarkButton;
    private Button RemoveLandmarkButton;
    private Button CenterViewButton;
    private Button DownloadMeshButton;
    private Button CancelButton;

    // access to script 'ViewController'
    public ViewController view;

    // center of rotation
    public Vector3 Pivot;

    // popup
    private bool ShowP = false;

    // variables for rotation
    private float dx = 0.0F;
    private float dy = 0.0F;
    private float x = 0.0F;
    private float y = 0.0F;
    private float b = 0.0F;

    // avoid unwanted transformation
    private bool move = false;

    // Use this for initialization
    void Start()
    {
        // initialize 'popup'
        Popup = GameObject.Find("PopupPanel");
        Popup.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // access to view
        view = FindFirstObjectByType(typeof(ViewController)) as ViewController;

        // add landmark
        AddLandmarkButton = GameObject.Find("AddLandmark").GetComponent<Button>();
        AddLandmarkButton.onClick.AddListener(delegate { AddLandmark(); });

        // remove landmark
        RemoveLandmarkButton = GameObject.Find("RemoveLandmark").GetComponent<Button>();
        RemoveLandmarkButton.onClick.AddListener(delegate { RemoveLandmark(); });

        // center view
        CenterViewButton = GameObject.Find("CenterView").GetComponent<Button>();
        CenterViewButton.onClick.AddListener(delegate { Center(); });

        // download mesh
        DownloadMeshButton = GameObject.Find("DownloadMesh").GetComponent<Button>();
        DownloadMeshButton.onClick.AddListener(delegate { DownloadMesh(); });

        // cancel
        CancelButton = GameObject.Find("Cancel").GetComponent<Button>();
        CancelButton.onClick.AddListener(delegate { Cancel(); });


        // hide the popup menue
        Popup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // pop up window
        if (ShowP)
        {
            Popup.SetActive(true);
            // let popup follow mouse position
            Popup.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }

        // check first mouse button being released 
        if (Input.GetMouseButtonUp(0))
        {
            move = false;
        }

        // check for first mouse button being pressed and verify pointer is not hover any ui element (in that moment) 
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            move = true;
        }
        else
        {
            // hide popup, method above avoid closing popup, force to close it
            ShowP = false;
        }

        // moving ...
        if (move)
        {
            // delta for mouse movement
            dx = Input.mousePosition.x - x;
            dy = Input.mousePosition.y - y;

            if ((dx != 0) || (dy != 0))
            {
                // norm
                b = Mathf.Sqrt(Mathf.Pow(dx, 2) + Mathf.Pow(dy, 2));

                dx = dx / b;
                dy = dy / b;

                // save current position
                x = Input.mousePosition.x;
                y = Input.mousePosition.y;

                // transform object around vector perpendicular to movement vector
                transform.RotateAround(new Vector3(0, 0, 0), new Vector3(dy, -dx, 0), 5);
            }

            // hide popup
            ShowP = false;
        }

        // second mouse button pressed
        else if (UnityEngine.Input.GetMouseButton(1))
        {
            // show popup
            ShowP = true;
            // show popup window
            Popup.SetActive(true);
        }
    }

    private void AddLandmark()
    {
        //Popup.SetActive(false);
        Debug.Log("add landmark");
    }

    private void RemoveLandmark()
    {
        //Popup.SetActive(false);
        Debug.Log("remove landmark");
    }

    private void Center()
    {
        //Popup.SetActive(false);
        view.CenterView();

        ShowP = false;
        Popup.SetActive(false);
    }

    private void DownloadMesh()
    {
        //Popup.SetActive(false);
        Debug.Log("download mesh");

        //DEBUG
        try
        {
            Hello();
        }
        catch
        {
            Debug.Log("WebGL?");
        }

        ShowP = false;
        Popup.SetActive(false);
    }


    private void Cancel()
    {
        //Popup.SetActive(false);
        Debug.Log("cancel");

        ShowP = false;
        Popup.SetActive(false);
    }

}
