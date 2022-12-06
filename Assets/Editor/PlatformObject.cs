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
        SplinePlatform iFace;

        GameObject surface;
        SurfaceGenerator surfaceGenerator;

        GameObject wall;
        PathGenerator wallGenerator;

        // Create parent platform object
        platform = new GameObject("Platform", typeof(SplineComputer), typeof(SplinePlatform));

        platformComputer = platform.GetComponent<SplineComputer>();
        iFace = platform.GetComponent<SplinePlatform>();
        platformComputer.sampleRate = 20;
        platformComputer.type = Spline.Type.BSpline;

        // Create child surface object
        surface = new GameObject("Surface", typeof(SurfaceGenerator));
        surface.transform.parent = platform.transform;

        surfaceGenerator = surface.GetComponent<SurfaceGenerator>();
        surfaceGenerator.spline = platformComputer;
        surfaceGenerator.extrude = 0.1f;
        surfaceGenerator.expand = 0.05f;
        surfaceGenerator.offset = new Vector3(0f, -0.1f, 0f);

        // Create child wall object
        wall = new GameObject("Wall", typeof(PathGenerator));
        wall.transform.parent = platform.transform;

        wallGenerator = wall.GetComponent<PathGenerator>();
        wallGenerator.spline = platformComputer;
        wallGenerator.size = 10f;
        wallGenerator.offset = new Vector3(0f, -5.0f, 0f);
        wallGenerator.rotation = 90f;
        wallGenerator.useShapeCurve = true;
        wallGenerator.shapeExposure = 10f;
        wallGenerator.slices = 8;
        wallGenerator.shape = AnimationCurve.Constant(0, 1, 0);

        iFace.surface = surfaceGenerator;
        iFace.wall = wallGenerator;
    }
}
