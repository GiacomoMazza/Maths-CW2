using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GM_GameManager : MonoBehaviour
{
    //------------------------------------------------------------ Camera members.
    /// <summary>
    /// All the cameras in the scene that you want to iterate through.
    /// </summary>
    [SerializeField]
    [Header("CAMERAS")]
    [Tooltip("All the cameras in the scene to show the object from different angles.")]
    private Camera[] c_CamerasInScene;

    //Value that will be passed as index element of the array above.
    private int in_CameraValue = 0;

    //------------------------------------------------------------ Plane Stuff
    [SerializeField]
    [Header("PLANE")]
    [Tooltip("The plane in the scene.")]
    private GameObject go_PlaneInScene;

    //------------------------------------------------------------ Object members.
    /// <summary>
    /// MUST BE A PREFAB ASSET. LOOK AT REAM ME FOR MORE INFORMATION. The object (CHILD) that needs to be studied.
    /// </summary>
    [SerializeField]
    [Header("MESH")]
    [Tooltip("The mesh you want to study. It's the one you saved after you adjusted your model.")]
    private GameObject go_MeshStudied;

    /// <summary>
    /// The child object of the one above.
    /// </summary>
    private GameObject go_ChildObject;

    //Members for the speed value of the rotation of the object.
    private int in_SpeedY = 0;
    private int in_SpeedX = 0;

    //The limit until the speed goes back to zero.
    private int in_SpeedLimit = 3;

    [Header("INITIAL MODEL")]
    [Tooltip("Your model, the one you need to adjust.")]
    [SerializeField]
    private GameObject go_Studied;

    [Header("DROP SPHERE")]
    [Tooltip("The jelly sphere to drop.")]
    [SerializeField]
    private GameObject go_SphereDrop;

    //------------------------------------------------------------ Jelly members
    //[SerializeField]
    //[Tooltip("Input field for the jelly's stiffness.")]
    //private InputField if_Stiffness;

    //[SerializeField]
    //[Tooltip("Input field for the jelly's stiffness.")]
    //private InputField if_BounceForce;

    //[SerializeField]
    //[Tooltip("Input field for the jelly's stiffness.")]
    //private InputField if_FallForce;

    //[SerializeField]
    //[Tooltip("Input field for the jelly's stiffness.")]
    //private InputField if_MousePressure;

    private void Start()
    {
        //Deactivate all the cameras but the one at element 0.
        for (int i = 1; i < c_CamerasInScene.Length; i++)
        {            
            c_CamerasInScene[i].enabled = false;
        }

        //If the object has been assigned...
        if (go_MeshStudied != null)
        {
            //Set the child to go_ChildObject (DEPRECATED).
            //if (go_MeshStudied.transform.childCount != 1)
            //{
            //    Debug.LogError("WARNING: The selected prefab must have ONE child. See the READ ME file for further information.");
            //}

            //Instantiate the object to study (DEPRECATED) .
            //GameObject go_Temp = Instantiate(go_MeshStudied, Vector3.up, Quaternion.identity);

            //And re-assign go_Temp to our object child (DEPRECATED).
            //go_ChildObject = go_Temp.transform.GetChild(0).gameObject;
            go_ChildObject = Instantiate(go_MeshStudied, Vector3.up, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (go_ChildObject != null)
        {
            if (in_SpeedX != 0)
            {
                go_ChildObject.transform.Rotate(Vector3.up * in_SpeedX);
            }

            if (in_SpeedY != 0)
            {
                go_ChildObject.transform.Rotate(Vector3.forward * in_SpeedY);
            } 
        }
    }

    #region Drop Sphere Method
    /// <summary>
    /// The method used to instantiate the jelly sphere and drop it on top of the mesh to show jelly collision.
    /// </summary>
    public void DropSphereButton()
    {
        GameObject go_Temp = Instantiate(go_SphereDrop, new Vector3(0, 9, 0), Quaternion.identity);
        StartCoroutine("DestroyJellySphere", go_Temp);
    } 

    private IEnumerator DestroyJellySphere(GameObject _Instance)
    {
        yield return new WaitForSeconds(8);
        Destroy(_Instance);
        yield return null;
    }
    #endregion

    #region Save Mesh.
    /// <summary>
    /// Save the object.
    /// </summary>
    public void SaveButton()
    {
        if (go_Studied != null)
        {
            AssetDatabase.CreateAsset(go_Studied.GetComponent<MeshStudy>().clonedMesh, "Assets/GM_SavedMeshes/GameObject_" + Random.Range(0, 999).ToString() + ".asset");
            AssetDatabase.SaveAssets(); 
        }

        else
        {
            Debug.LogError("Warning: the object to be saved as mesh needs to be assigned in GM_GameManager.");
        }
    } 
    #endregion

    #region Explode Jelly - Used.
    /// <summary>
    /// Make jelly explode! Calls a function in GM_ExplodingJelly to make the object explode.
    /// </summary>
    public void ExplodeJelly()
    {
        if (go_ChildObject != null)
        {
            if (go_ChildObject.GetComponent<GM_ExplodingJelly>() == null)
            {
                go_ChildObject.AddComponent<GM_ExplodingJelly>();
                go_ChildObject.GetComponent<GM_ExplodingJelly>().StartCoroutine("SplitMesh", true);
            }
        }
    }
    #endregion

    #region Add MakeMeJelly Component - Used (Variables to be changed in the Editor).
    /// <summary>
    /// Make that object jelly. Adds GM_MakeMeJelly, a Rigidbody, a Mesh Collider, and it sets it as convex.
    /// </summary>
    public void Jellyfy()
    {
        if (go_ChildObject != null)
        {
            if (go_ChildObject.GetComponent<GM_MakeMeJelly>() == null)
            {
                go_ChildObject.AddComponent<GM_MakeMeJelly>();
                go_ChildObject.AddComponent<Rigidbody>();
                go_ChildObject.AddComponent<MeshCollider>();
                go_ChildObject.GetComponent<MeshCollider>().convex = true;
                //  go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_FallForce = float.Parse(if_FallForce.text);
                //  go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_Stiffness = float.Parse(if_Stiffness.text);
                //  go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_BounceSpeed = float.Parse(if_BounceForce.text);
                //  go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_MousePressure = float.Parse(if_MousePressure.text);

                //  if (go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_Stiffness >= 45)
                //  {
                //      go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_Stiffness = 45;
                //  }

                //  if (go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_MousePressure >= 5000)
                //  {
                //      go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_MousePressure = 5000;
                //  }

                //  if (go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_BounceSpeed >= 5000)
                //  {
                //      go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_BounceSpeed = 5000;
                //  }

                //  if (go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_FallForce >= 5000)
                //  {
                //      go_ChildObject.GetComponent<GM_MakeMeJelly>().fl_FallForce = 5000;
                //  }
            }

            else if (go_ChildObject.GetComponent<GM_MakeMeJelly>())
            {
                Destroy(go_ChildObject.GetComponent<GM_MakeMeJelly>());
                Destroy(go_ChildObject.GetComponent<Rigidbody>());
                Destroy(go_ChildObject.GetComponent<MeshCollider>());
            } 
        }
    } 
    #endregion

    #region Rotation of Mesh - Used.
    /// <summary>
    /// Change the speed of the horizontal rotation.
    /// </summary>
    public void ChangeSpeedX()
    {
        if (in_SpeedX < in_SpeedLimit)
        {
            in_SpeedX++;
        }

        else
        {
            in_SpeedX = 0;
        }
    }

    /// <summary>
    /// Change the speed of the vertical rotation.
    /// </summary>
    public void ChangeSpeedY()
    {
        if (in_SpeedY < in_SpeedLimit)
        {
            in_SpeedY++;
        }

        else
        {
            in_SpeedY = 0;
        }
    }

    /// <summary>
    /// Stop the mesh from rotating.
    /// </summary>
    public void Stop()
    {
        in_SpeedX = 0;
        in_SpeedY = 0;
    }

    /// <summary>
    /// Stop the mesh from rotating and reset its rotation.
    /// </summary>
    public void ResetPosition()
    {
        in_SpeedY = 0;
        in_SpeedX = 0;

        if (go_ChildObject != null)
        {
            go_ChildObject.transform.rotation = Quaternion.identity; 
        }
    } 
    #endregion

    #region Plane Code - Used.
    /// <summary>
    /// Set the object in the scene (the plane in this case) active or inactive.
    /// </summary>
    public void SetPlane()
    {
        if (go_PlaneInScene.activeInHierarchy)
        {
            go_PlaneInScene.SetActive(false);
        }

        else if (!go_PlaneInScene.activeInHierarchy)
        {
            go_PlaneInScene.SetActive(true);
        }
    } 
    #endregion

    #region Cut Mesh Code - Not used.
    //This does not work. The script is read, but no visible changes happen.
    //[SerializeField]
    //private Material m_CutMaterial;

    //// Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        Debug.Log("RMB Pressed");
    //        RaycastHit myhit;
    //        Ray myray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        if (Physics.Raycast(myray, out myhit, Mathf.Infinity))
    //        {
    //            Debug.Log("Gone through");
    //            GM_Cutter.Cut(myhit.transform.gameObject, myhit.point, myray.direction, m_CutMaterial, true, true);
    //        }
    //    }
    //}
    #endregion

    #region Camera Code - Used.
    private void ChangeToCameraGeneral(int Index)
    {
        //Set true the camera that was clicked via a button.
        c_CamerasInScene[Index].enabled = true;

        //Set i to be this value because if index == 0 then we don't need i to check element 0 in the array, but we can go to 1 immediately. If index is 1, then we
        //need to check element 0.
        int i = 1 - Index;

        //If Index >= 2, then we need to go through element 0 and check all the others.
        if (i < 0)
        {
            i = 0;
        }

        //Iterate through the cameras using i as a value.
        for (; i < c_CamerasInScene.Length; i++)
        {
            //If i is not the camera we selected to be active (Index), and if the camera we selected is still active in the scene, set it as not active.
            if (i != Index && c_CamerasInScene[i] != null)
            {
                c_CamerasInScene[i].enabled = false; 
            }
        }
    }

    /// <summary>
    /// Link this method to a button to iterate through the cameras stored in the array.
    /// </summary>
    public void ChangeCameraButton()
    {
        //If in_CameraValue is in the boundaries of the array, then go to the next camera...
        if(in_CameraValue != c_CamerasInScene.Length - 1)
        {
            in_CameraValue++;
            ChangeToCameraGeneral(in_CameraValue);
        }

        //... Otherwise, start from the beginning camera.
        else
        {
            in_CameraValue = 0;
            ChangeToCameraGeneral(in_CameraValue);
        }
    }
    #endregion

    #region Old Camera Code - Not used.
    //This was used as the first and then second method. Both methods required to chnage the code; now using the function above, everything is modular.
    //public void ChangeToCamera0()
    //{
    //    ChangeToCameraGeneral(0);
    //}

    //public void ChangeToCamera1()
    //{
    //    ChangeToCameraGeneral(1);
    //}

    //public void ChangeToCamera2()
    //{
    //    ChangeToCameraGeneral(2);
    //}

    //public void ChangeToCamera3()
    //{
    //    //This was repeated in every single function. Now there is a general function and these function just 
    //    //call an index.
    //    //c_CamerasInScene[3].enabled = true;

    //    //for (int i = 0; i < c_CamerasInScene.Length; i++)
    //    //{
    //    //    if (i != 3)
    //    //    {
    //    //        c_CamerasInScene[i].enabled = false;
    //    //    }
    //    //}

    //    ChangeToCameraGeneral(3);
    //}
    #endregion
}
