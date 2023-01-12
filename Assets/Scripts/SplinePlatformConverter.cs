using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Dreamteck.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SplinePlatformConverter : MonoBehaviour
{
    public GameObject geometry;
    public Material defaultGround;
    public Material defaultWall;
    public void ConvertGeometry()
    {
        GameObject surface;
        SurfaceGenerator surfaceGenerator;

        GameObject wall;
        PathGenerator wallGenerator;

        Mesh mesh = geometry.GetComponent<MeshFilter>().sharedMesh;
        List<int> indices = GetIndices(mesh);

        Debug.Log(string.Join(",", indices));

        SplineComputer spline = geometry.AddComponent<SplineComputer>();
        SplinePlatform iFace = geometry.AddComponent<SplinePlatform>();
        SplinePoint[] points = new SplinePoint[indices.Count];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = transform.TransformPoint(mesh.vertices[indices[i]]);
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }

        spline.SetPoints(points);
        spline.Close();
        

        spline.sampleRate = 20;
        spline.type = Spline.Type.BSpline;

        // Create child surface object
        surface = new GameObject("Surface", typeof(SurfaceGenerator));
        surface.GetComponent<Renderer>().sharedMaterial = defaultGround;
        surface.transform.parent = geometry.transform;

        surfaceGenerator = surface.GetComponent<SurfaceGenerator>();
        surfaceGenerator.spline = spline;
        surfaceGenerator.extrude = 0.1f;
        surfaceGenerator.expand = 0.05f;
        surfaceGenerator.offset = new Vector3(0f, -0.1f, 0f);

        // Create child wall object
        wall = new GameObject("Wall", typeof(PathGenerator));
        wall.GetComponent<Renderer>().sharedMaterial = defaultWall;
        wall.transform.parent = geometry.transform;

        wallGenerator = wall.GetComponent<PathGenerator>();
        wallGenerator.spline = spline;
        wallGenerator.size = 10f;
        wallGenerator.offset = new Vector3(0f, -5.0f, 0f);
        wallGenerator.rotation = 90f;
        wallGenerator.useShapeCurve = true;
        wallGenerator.shapeExposure = 10f;
        wallGenerator.slices = 8;
        wallGenerator.shape = AnimationCurve.Constant(0, 1, 0);

        iFace.surface = surfaceGenerator;
        iFace.wall = wallGenerator;

        geometry.GetComponent<MeshRenderer>().enabled = false;
        geometry.GetComponent<BoxCollider>().enabled = false;
    }

    List<int> GetIndices(Mesh mesh)
    {
        return new List<int> (mesh.triangles.AsEnumerable().Distinct().Where(
            index => mesh.normals[index] == Vector3.up
            ).ToList());
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(SplinePlatformConverter))]
public class SplinePlatformConverterEditor : Editor
{
    SplinePlatformConverter script;
    private void OnEnable()
    {
        script = (SplinePlatformConverter)target;
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Convert Geometry"))
        {
            script.ConvertGeometry();
        }
    }
}

#endif
