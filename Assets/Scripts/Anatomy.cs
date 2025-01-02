using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Anatomy : MonoBehaviour
{
    public GameObject BoneMesh = null;
    public ViewController view;

    // text
    private Text msg;
    private Text Landmarks;

    // dataset
    private Dropdown DatasetSelector;

    // dataset
    private Dropdown AnatomySelector;
    
    // dataset
    private Dropdown SideSelector;
    
    // dataset
    private Dropdown GenderSelector;
    
    // dataset
    private Dropdown EthnicGroupSelector;

    //private BoneController bc;
    GameObject[] LandmarkSet;

    void Start()
    {
        // define listener 'SelectAnatomy' for dropdown element
        DatasetSelector = GameObject.Find("DatasetSelector").GetComponent<Dropdown>();
        DatasetSelector.onValueChanged.AddListener(delegate { SelectDataset(DatasetSelector.captionText.text); });

        AnatomySelector = GameObject.Find("AnatomySelector").GetComponent<Dropdown>();
        SideSelector = GameObject.Find("SideSelector").GetComponent<Dropdown>();
        GenderSelector = GameObject.Find("GenderSelector").GetComponent<Dropdown>();
        EthnicGroupSelector = GameObject.Find("EthnicGroupSelector").GetComponent<Dropdown>();

        // landmar text
        Landmarks = GameObject.Find("/Canvas/LandmarksPanel/Landmarks").GetComponent<Text>();

        // study text
        msg = GameObject.Find("/Canvas/Messages").GetComponent<Text>();

        // game object
        BoneMesh = new GameObject("Bone");

        BoneMesh.AddComponent<MeshCollider>();

        // add script for movement
        BoneMesh.AddComponent<BoneController>();

        // mesh filter and mesh renderer components are needed
        BoneMesh.AddComponent<MeshFilter>();
        BoneMesh.AddComponent<MeshRenderer>();

        // set default material
        BoneMesh.GetComponent<MeshRenderer>().material = Resources.Load<Material>("BoneMeshMaterial");

        // load mesh from ressource folder
        SelectDataset(DatasetSelector.captionText.text);
    }

    private void SelectDataset(string dataset)
    {
        // reset study text
        msg.text = "No messages yet!";

        // if no dataset is selected break (no mesh can be processed then)
        if (dataset == "Select Dataset")
            return;

        // define meta infos from dataset name (if possible)
        DefineSideAgeSexFromDatasetName(dataset);
        
        // ethnic group cannot be guessed by dataset name (yet)
        EthnicGroupSelector.value = 1;

        // load mesh
        BoneMesh.GetComponent<MeshFilter>().mesh = Resources.Load(dataset, typeof(Mesh)) as Mesh;

        // get center of mass from mesh
        //Vector3 CenterOfMass = GetCOM(BoneMesh.GetComponent<MeshFilter>().mesh);

        // store it in BoneController's 'pivot' variable
        BoneController br = BoneMesh.GetComponent<BoneController>();
        br.Pivot.x = BoneMesh.GetComponent<MeshFilter>().mesh.bounds.center.x;
        br.Pivot.y = BoneMesh.GetComponent<MeshFilter>().mesh.bounds.center.y;
        br.Pivot.z = BoneMesh.GetComponent<MeshFilter>().mesh.bounds.center.z;

        // center view
        view = FindFirstObjectByType(typeof(ViewController)) as ViewController;
        view.CenterView(new Vector3(0, 0, 0), BoneMesh.GetComponent<MeshFilter>().mesh.bounds);

        // load landmarks after mesh 
        LoadAnatomicalLandmarks(dataset + "-landmarks");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private Vector3 GetCOM(Mesh mesh)
    {
        Vector3 ret = new Vector3(0, 0, 0);

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; ++i)
        {
            ret.x += vertices[i].x;
            ret.y += vertices[i].y;
            ret.z += vertices[i].z;
        }

        ret.x = ret.x / vertices.Length;
        ret.y = ret.y / vertices.Length;
        ret.z = ret.z / vertices.Length;

        //Debug.Log(ret.x + " " + ret.y + " " + ret.z);

        return ret;
    }

    private void LoadAnatomicalLandmarks(string Anatomy)
    {
        // reset landmark list
        Landmarks.text = "";

        // read csv data as TextAsset
        TextAsset Text = Resources.Load(Anatomy) as TextAsset;
        string[] TextArray = Text.text.Split('\n');
        
        // if array for spheres is not null
        if (LandmarkSet != null)
        {
            // destroy every sphere
            foreach (GameObject lm in LandmarkSet)
            {
                Destroy(lm as GameObject);
            }
        }

        // create new array for spheres
        LandmarkSet = new GameObject[TextArray.Length - 1];

        for (int i = 0; i < TextArray.Length-1; ++i)
        {
            // 'split' needs char -> convert ',' to char
            string[] Entries = TextArray[i].Split(',');
            Landmarks.text += Entries[0] + "\n";

            // create sphere as landmark
            LandmarkSet[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            LandmarkSet[i].AddComponent<LandmarkHover>();

            // collider to make raycast oder mouse over possible
            LandmarkSet[i].AddComponent<SphereCollider>();

            // append anatomical name
            LandmarkSet[i].name += Entries[0];

            // use 'float.TryParse' otherweise cast is not working
            // workaround: right to left hand cs: invert x-coordinates, y = -z, z = y
            float x = 0.0F;
            x = float.Parse(Entries[1], CultureInfo.InvariantCulture.NumberFormat) * (-1);

            float y = 0.0F;
            y = float.Parse(Entries[3], CultureInfo.InvariantCulture.NumberFormat);

            float z = 0.0F;
            z = float.Parse(Entries[2], CultureInfo.InvariantCulture.NumberFormat) * (-1);

            // set correct position, scale with predefined values, load and set material
            LandmarkSet[i].transform.position = new Vector3(x, y, z);
            LandmarkSet[i].transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
            LandmarkSet[i].GetComponent<MeshRenderer>().material = Resources.Load<Material>("Green");
            
            // make transformation dependent from parent (=bone) 
            LandmarkSet[i].transform.parent = BoneMesh.transform;
        }


    }


    private void DefineSideAgeSexFromDatasetName(string dataset)
    {
        // make dataset great again
        dataset = dataset.ToUpper();

        // try to guess anatomy from dataset name (yet FEM, HUM, TIB)
        if (dataset.Contains("FEMUR"))
            AnatomySelector.value = 1;
        else if (dataset.Contains("HUMERUS"))
            AnatomySelector.value = 2;
        else if (dataset.Contains("TIBIA"))
            AnatomySelector.value = 3;
        else
            AnatomySelector.value = 0;

        // separator '-' must be found
        int pos = dataset.IndexOf("-");

        if (pos <= 0)
        {
            // try another approach to find side ...
            if (dataset.Contains("LEFT"))
                SideSelector.value = 1;
            else if (dataset.Contains("RIGHT"))
                SideSelector.value = 2;
            else
                SideSelector.value = 3;

            // ... and gender
            if (dataset.Contains("FEMALE"))
                GenderSelector.value = 1;
            else if (dataset.Contains("MALE"))
                GenderSelector.value = 2;
            else
                GenderSelector.value = 3;
            return;
	    }

        // set sex found at 4 position after '-'
        string gender = dataset.Substring(pos + 4, 1);

        if (gender.Contains("F"))
            GenderSelector.value = 1;
        else if (gender.Contains("M"))
            GenderSelector.value = 2;
        else
            GenderSelector.value = 3;

        // and side on first position after '-'
        string side = dataset.Substring(pos + 1, 1);

        if (side.Contains("L"))
            SideSelector.value = 1;
        else if (side.Contains("R"))
            SideSelector.value = 2;
        else
            GenderSelector.value = 3;
    }
}