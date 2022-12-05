using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

/*
 * TO EXPOSE
 * UVS
 *  Surface Top UV Scale
 *  Surface Side UV Scale
 *  Wall Side UV Scale
 */
public class SplinePlatform : MonoBehaviour
{
    [HideInInspector] public SurfaceGenerator surface;
    [HideInInspector] public SurfaceGenerator wall;

    public Material surfaceMat;
    public Material wallMat;

    public float platformDepth;

    public Vector2 surfaceUVScale;
    public Vector2 surfaceSideUVScale;
    public Vector2 wallUVScale;

    public void Sync()
    {
        platformDepth = -wall.extrude;
        surfaceUVScale = surface.uvScale;
        surfaceSideUVScale = surface.sideUvScale;
        wallUVScale = wall.uvScale;
    }

    private void OnValidate()
    {
        wall.extrude = -platformDepth;

        surface.uvScale = surfaceUVScale;
        surface.sideUvScale = surfaceSideUVScale;
        wall.uvScale = wallUVScale;

        surface.GetComponent<MeshRenderer>().material = surfaceMat;
        wall.GetComponent<MeshRenderer>().material = wallMat;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SplinePlatform))]
public class SplinePlatformEditor : Editor
{
    SplinePlatform script;

    private void OnEnable()
    {
        script = (SplinePlatform)target;
        script.Sync();

    }
}
#endif


