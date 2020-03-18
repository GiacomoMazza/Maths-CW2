using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script was written using this tutorial: https://www.youtube.com/watch?v=UxLJ6XewTVs&t=1s

/// <summary>
/// Holds all the information about the single vertex.
/// </summary>
public class GM_VerticesHolder
{
    public int in_VertexIndex;
    public Vector3 v3_InitialPositionVertex;
    public Vector3 v3_CurrentPositionVertex;

    public Vector3 v3_CurrentVelocityVertex;

    /// <summary>
    /// Constructor - Set the values for each single vertex.
    /// </summary>
    /// <param name="_in_Index"></param>
    /// <param name="_in_InitialPosition"></param>
    /// <param name="_v3_CurrentPosition"></param>
    /// <param name="_v3_CurrentVelocity"></param>
    public GM_VerticesHolder(int _in_Index, Vector3 _v3_InitialPosition, Vector3 _v3_CurrentPosition, Vector3 _v3_CurrentVelocity)
    {
        in_VertexIndex = _in_Index;
        v3_InitialPositionVertex = _v3_InitialPosition;
        v3_CurrentVelocityVertex = _v3_CurrentVelocity;
        v3_CurrentPositionVertex = _v3_CurrentPosition;
    }

    /// <summary>
    /// Calculate displacement vector between current position and initial position.
    /// </summary>
    /// <returns></returns>
    public Vector3 v3_GetDisplacementCurrent()
    {
        return v3_CurrentPositionVertex - v3_InitialPositionVertex;
    }

    /// <summary>
    /// Calculate the vertex's velocity according to displacement vector, bounciness, and time.
    /// </summary>
    /// <param name="_fl_BounceSpeed"></param>
    public void UpdateVelocity(float _fl_BounceSpeed)
    {
        v3_CurrentVelocityVertex = v3_CurrentVelocityVertex - v3_GetDisplacementCurrent() * _fl_BounceSpeed * Time.deltaTime;
    }

    /// <summary>
    /// This assures that the velocity will return to zero over time, accordingly to its stiffness.
    /// </summary>
    /// <param name="_fl_Stiffness"></param>
    public void Settle(float _fl_Stiffness)
    {
        v3_CurrentVelocityVertex *= 1f - _fl_Stiffness * Time.deltaTime;
    }

    /// <summary>
    /// Apply pressure to vertex. Get distance from vertex to input position, and calculate a pressure to turn into
    /// velocity. Get the right direction for the velocity by getting the normalised value of were the mesh was touched.
    /// </summary>
    /// <param name="_tr_MyTransform"></param>
    /// <param name="_v3_MuPosition"></param>
    /// <param name="_fl_pressure"></param>
    public void ApplyPressureToVertex(Transform _tr_MyTransform, Vector3 _v3_MyPosition, float _fl_pressure)
    {
        Vector3 v3_DistanceVertexPoint = v3_CurrentPositionVertex - _tr_MyTransform.InverseTransformPoint(_v3_MyPosition);
        float fl_AdaptedPressure = _fl_pressure / (1f + v3_DistanceVertexPoint.sqrMagnitude);
        float fl_Velocity = fl_AdaptedPressure * Time.deltaTime;
        v3_CurrentVelocityVertex += v3_DistanceVertexPoint.normalized * fl_Velocity;
    }
}
