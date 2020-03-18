using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script was made with the help of this tutorial: https://www.youtube.com/watch?v=UxLJ6XewTVs
/// <summary>
/// Turn mesh into a jelly.
/// </summary>
public class GM_MakeMeJelly : MonoBehaviour
{
    //How fast the mesh jiggles back and forth.
    public float fl_BounceSpeed;

    //Emulates mesh's mass when interacting with other objects.
    public float fl_FallForce;

    //The pressure given by the mouse input.
    public float fl_MousePressure;

    //The mesh's stiffness: helps to define how long and how much the mesh jiggles.
    public float fl_Stiffness;

    //The mesh's mesh and meshfilter components.
    private MeshFilter mf_MyMeshFilter;
    private Mesh ms_MyMesh;

    //References to the mesh's vertices and the current vertices.
    GM_VerticesHolder[] MeshVertices;
    Vector3[] v3_CurrentMeshVertices;

    // Start is called before the first frame update. Initialise our members and get the mesh's vertices.
    private void Start()
    {
        mf_MyMeshFilter = GetComponent<MeshFilter>();
        ms_MyMesh = mf_MyMeshFilter.mesh;

        GetVertices();
    }

    /// <summary>
    /// Get all the vertices in the mesh and store their values in GM_VerticesHolder. Set an ID for each vertex.
    /// </summary>
    private void GetVertices()
    {
        MeshVertices = new GM_VerticesHolder[ms_MyMesh.vertices.Length];
        v3_CurrentMeshVertices = new Vector3[ms_MyMesh.vertices.Length];

        for(int i = 0; i < ms_MyMesh.vertices.Length; i++)
        {
            MeshVertices[i] = new GM_VerticesHolder(i, ms_MyMesh.vertices[i], ms_MyMesh.vertices[i], Vector3.zero);
            v3_CurrentMeshVertices[i] = ms_MyMesh.vertices[i];
        }
    }

    // Update is called once per frame. Deform mesh on mouse input. Update the vertices' velocity and position.
    void Update()
    {
        if (fl_Stiffness < 1f)
        {
            fl_Stiffness = 1f;
        }

        if (fl_BounceSpeed < 0f)
        {
            fl_BounceSpeed = 0f;
        }

        if (fl_FallForce < 0f)
        {
            fl_FallForce = 0f;
        }

        if  (fl_MousePressure < 0f)
        {
            fl_MousePressure = 0f;
        }

        if (Input.GetMouseButton(1))
        {
            //Vector3 v3_MouseInput = Input.mousePosition + (Input.mousePosition * .1f);
            //ApplyPressureToPoint(v3_MouseInput, fl_MousePressure);

            //for (int i = 0; i < 20; i++)
            //{
            //Vector3 v3_MousePosition = Vector3.Cross(Input.mousePosition.normalized, new Vector3(i, i, i));
            //ApplyPressureToPoint(v3_MousePosition, fl_MousePressure);
            //}

            RaycastHit hit;
            Ray MyRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(MyRay, out hit, Mathf.Infinity))
            {
                ApplyPressureToPoint(hit.point, fl_MousePressure);
            }
        }

        UpdateVertices();
    }

    /// <summary>
    /// Update the vertices' position and velocity, and then recalculate the mesh.
    /// </summary>
    private void UpdateVertices()
    {
        //For each vertex identified by the index given before...
        for (int i = 0; i < MeshVertices.Length; i++)
        {
            //Update its velocity, and start to settle.
            MeshVertices[i].UpdateVelocity(fl_BounceSpeed);
            MeshVertices[i].Settle(fl_Stiffness);

            //Update the vertix's velocity and apply it.
            MeshVertices[i].v3_CurrentPositionVertex += MeshVertices[i].v3_CurrentVelocityVertex * Time.deltaTime;
            v3_CurrentMeshVertices[i] = MeshVertices[i].v3_CurrentPositionVertex;
        }

        //Recalculate where the vertices are, the mesh's bounds, tangents, and normals.
        ms_MyMesh.vertices = v3_CurrentMeshVertices;
        ms_MyMesh.RecalculateBounds();
        ms_MyMesh.RecalculateNormals();
        ms_MyMesh.RecalculateTangents();
    }

    /// <summary>
    /// Apply pressure to an area of the mesh. Specify where the input is, and with what force it has been applied.
    /// </summary>
    /// <param name="_v3_PointOfPressure"></param>
    /// <param name="_fl_PressureForce"></param>
    public void ApplyPressureToPoint(Vector3 _v3_PointOfPressure, float _fl_PressureForce)
    {
        for (int i = 0; i < MeshVertices.Length; i++)
        {
            MeshVertices[i].ApplyPressureToVertex(transform, _v3_PointOfPressure, _fl_PressureForce);
        }
    }

    /// <summary>
    /// Calculate the collision points between this mesh and the collision mesh.
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] cp_CollisionPoints = collision.contacts;

        for (int i = 0; i < cp_CollisionPoints.Length; i++)
        {
            Vector3 v3_InputPoint = cp_CollisionPoints[i].point + (cp_CollisionPoints[i].point * .1f);
            ApplyPressureToPoint(v3_InputPoint, fl_FallForce);
        }
    }
}
