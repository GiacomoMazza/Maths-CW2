/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

//This script was slightly modified. The only parts that were modified is methods that are not commented and the
//new methods which have been commented as "//MODIFIED".

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[ExecuteInEditMode]
public class MeshStudy : MonoBehaviour
{
    Mesh originalMesh;

    [HideInInspector]
    public Mesh clonedMesh;

    MeshFilter meshFilter;
    int[] triangles;

    [HideInInspector]
    public Vector3[] v3_vertices;

    [HideInInspector]
    public bool isCloned = false;

    // For Editor
    public float radius = 0.2f;
    public float pull = 0.3f;
    public float handleSize = 0.03f;
    public List<int>[] connectedVertices;
    public List<Vector3[]> allTriangleList;
    public bool moveVertexPoint = true;

    void Start()
    {
        InitMesh();
    }

    //MODIFIED: an error was showing up, so I had to add the exceptiong for a SkinnedMeshRenderer component.
    public void InitMesh()
    {
        if(!gameObject.GetComponent<MeshFilter>())
        {
            Mesh SkinMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            originalMesh = SkinMesh;
            clonedMesh = new Mesh(); //2

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            //meshFilter.mesh = clonedMesh;  //3

            v3_vertices = clonedMesh.vertices; //4
            triangles = clonedMesh.triangles;
            isCloned = true; //5
            Debug.Log("Init & Cloned: SkinMesh");
        }

        else if (gameObject.GetComponent<MeshFilter>())
	    {
            meshFilter = GetComponent<MeshFilter>();
            originalMesh = meshFilter.sharedMesh; //1
            clonedMesh = new Mesh(); //2

            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            meshFilter.mesh = clonedMesh;  //3

            v3_vertices = clonedMesh.vertices; //4
            triangles = clonedMesh.triangles;
            isCloned = true; //5
            Debug.Log("Init & Cloned: Filter"); 
        }
    }

    /// <summary>
    /// Resets the mesh to its original values.
    /// </summary>
    public void Reset()
    {
        if (clonedMesh != null && originalMesh != null) //1
        {
            clonedMesh.vertices = originalMesh.vertices; //2
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            clonedMesh.uv = originalMesh.uv;
            if (gameObject.GetComponent<MeshFilter>())
            {
                meshFilter.mesh = clonedMesh; //3 
            }

            v3_vertices = clonedMesh.vertices; //4
            triangles = clonedMesh.triangles;
        }
    }

    /// <summary>
    /// Initialises the list of all the vertices that are connected.
    /// </summary>
    public void GetConnectedVertices()
    {
        connectedVertices = new List<int>[v3_vertices.Length];
    }

    /// <summary>
    /// Pulls the vertices sharing the same position when interacting with a handle.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="localPos"></param>
    public void DoAction(int index, Vector3 localPos)
    {
        PullSimilarVertices(index, localPos);
    }

    // returns List of int that is related to the targetPt.
    private List<int> FindRelatedVertices(Vector3 v3_targetPt, bool findConnected)
    {
        // list of int
        List<int> l_relatedVertices = new List<int>();

        int idx = 0;
        Vector3 pos;

        // loop through triangle array of indices
        for (int t = 0; t < triangles.Length; t++)
        {
            // current idx return from tris
            idx = triangles[t];
            // current pos of the vertex
            pos = v3_vertices[idx];
            // if current pos is same as targetPt
            if (pos == v3_targetPt)
            {
                // add to list
                l_relatedVertices.Add(idx);
                // if find connected vertices
                if (findConnected)
                {
                    // min
                    // - prevent running out of count
                    if (t == 0)
                    {
                        l_relatedVertices.Add(triangles[t + 1]);
                    }
                    // max 
                    // - prevent runnign out of count
                    if (t == triangles.Length - 1)
                    {
                        l_relatedVertices.Add(triangles[t - 1]);
                    }
                    // between 1 ~ max-1 
                    // - add idx from triangles before t and after t 
                    if (t > 0 && t < triangles.Length - 1)
                    {
                        l_relatedVertices.Add(triangles[t - 1]);
                        l_relatedVertices.Add(triangles[t + 1]);
                    }
                }
            }
        }
        // return compiled list of int
        return l_relatedVertices;
    }

    //MODIFIED: an error was showing up, so I had to add the exceptiong for a SkinnedMeshRenderer component.
    //public void BuildTriangleList()
    //{
    //}

    //MODIFIED: an error was showing up, so I had to add the exceptiong for a SkinnedMeshRenderer component.
    //public void ShowTriangle(int idx)
    //{
    //}

    //MODIFIED: an error was showing up, so I had to add the exceptiong for a SkinnedMeshRenderer component.
    // Pulling only one vertex pt, results in non-manifold mesh.
    //private void PullOneVertex(int index, Vector3 newPos)
    //{
    //    vertices[index] = newPos; //1
    //    clonedMesh.vertices = vertices; //2
    //    clonedMesh.RecalculateNormals(); //3
    //}

    /// <summary>
    /// Pulls similar vertices, which share the same position.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="newPos"></param>
    private void PullSimilarVertices(int index, Vector3 newPos)
    {
        Vector3 v3_targetVertexPos = v3_vertices[index];
        List<int> l_relatedVertices = FindRelatedVertices(v3_targetVertexPos, false);
        foreach (int i in l_relatedVertices)
        {
            v3_vertices[i] = newPos;
        }
        clonedMesh.vertices = v3_vertices;
        clonedMesh.RecalculateNormals();
    }

    //MODIFIED: an error was showing up, so I had to add the exceptiong for a SkinnedMeshRenderer component.
    // To test Reset function
    //public void EditMesh()
    //{
    //    v3_vertices[2] = new Vector3(2, 3, 4);
    //    v3_vertices[3] = new Vector3(1, 2, 4);
    //    clonedMesh.vertices = v3_vertices;
    //    clonedMesh.RecalculateNormals();
    //}
}
