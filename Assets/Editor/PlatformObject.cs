using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamteck.Splines;

public class PlatformObject : Editor
{
    [MenuItem("GameObject/3D Object/Spline Platform", false, -1)]
    public static void SplinePlatform()
    {
        GameObject platform;
        SplineComputer platformComputer;

        GameObject surface;
        SurfaceGenerator surfaceGenerator;

        GameObject wall;
        SurfaceGenerator wallGenerator;

        platform = new GameObject("Platform", typeof(SplineComputer));

        platformComputer = platform.GetComponent<SplineComputer>();
        platformComputer.sampleRate = 20;
        platformComputer.type = Spline.Type.BSpline;

        surface = new GameObject("Surface", typeof(SurfaceGenerator), typeof(MeshRenderer), typeof(MeshFilter));
        surface.transform.parent = platform.transform;

        surfaceGenerator = surface.GetComponent<SurfaceGenerator>();
        surfaceGenerator.spline = platformComputer;
        surfaceGenerator.extrude = 0.1f;
        surfaceGenerator.offset = new Vector3(0f, -0.1f, 0f);

        wall = new GameObject("Wall", typeof(SurfaceGenerator), typeof(MeshRenderer), typeof(MeshFilter));
        wall.transform.parent = platform.transform;

        wallGenerator = wall.GetComponent<SurfaceGenerator>();
        wallGenerator.spline = platformComputer;
        wallGenerator.extrude = -1f;
        wallGenerator.offset = new Vector3(0f, -0.1f, 0f);
    }
}
