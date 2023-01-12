using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamteck.Splines;
using log4net.Util;
using System.Linq;
using Unity.VisualScripting;

[System.Serializable]
public class ConverterWindow : EditorWindow
{
    [MenuItem("Spline Platform Tool/Window/Converter")]
    
    static void Init()
    {
        EditorWindow window = GetWindow<ConverterWindow>();
        window.titleContent = new GUIContent("Spline Platform Converter");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 300, 330);

    }

    public GameObject[] geometry;
    public Material defaultSurfaceMat;
    public Material defaultWallMat;
    private Vector2 scrollPos;

    private SerializedObject serializedObject;
    SerializedProperty m_Geometry;

    bool isMaterialWarning = false;
    bool isGeometryWarning = false;

    private void OnEnable()
    {
        serializedObject = new SerializedObject(this);
        m_Geometry = serializedObject.FindProperty("geometry");
    }

    void OnGUI()
    {
        serializedObject.Update();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));

        //EditorGUILayout.LabelField("This is an example of EditorWindow.ShowPopup", EditorStyles.wordWrappedLabel);

        defaultSurfaceMat = EditorGUILayout.ObjectField("Surface Material", defaultSurfaceMat, typeof(Material), false) as Material;
        defaultWallMat = EditorGUILayout.ObjectField("Wall Material", defaultWallMat, typeof(Material), false) as Material;

        EditorGUILayout.PropertyField(m_Geometry, new GUIContent("Geometry"), true);
        serializedObject.ApplyModifiedProperties();


        GUIStyle warningTextLabel = new GUIStyle(EditorStyles.wordWrappedLabel);
        warningTextLabel.normal.textColor = Color.red;

        GUILayout.FlexibleSpace();
        if (isMaterialWarning)
        {
            EditorGUILayout.LabelField("Missing materials", warningTextLabel);
        }
        if (isGeometryWarning)
        {
            EditorGUILayout.LabelField("Missing geometry", warningTextLabel);
        }
        if (GUILayout.Button("Convert!"))
        {
            if (defaultSurfaceMat == null || defaultWallMat == null) isMaterialWarning = true;
            else isMaterialWarning = false;

            if (geometry == null || geometry.Length == 0) isGeometryWarning = true;
            else isGeometryWarning = false;

            if(!isMaterialWarning && !isGeometryWarning) ProcessGeometry();
        }
        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }

    void ProcessGeometry()
    {
        foreach(GameObject geo in geometry)
        {
            ConvertGeometry(geo);
        }
        this.Close();
    }

    void ConvertGeometry(GameObject geo)
    {
        GameObject surface;
        SurfaceGenerator surfaceGenerator;

        GameObject wall;
        PathGenerator wallGenerator;

        Mesh mesh = geo.GetComponent<MeshFilter>().sharedMesh;
        List<int> indices = GetIndices(mesh);

        Debug.Log(string.Join(",", indices));

        SplineComputer spline = geo.AddComponent<SplineComputer>();
        SplinePlatform iFace = geo.AddComponent<SplinePlatform>();
        SplinePoint[] points = new SplinePoint[indices.Count];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = geo.transform.TransformPoint(mesh.vertices[indices[i]]);
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }

        spline.SetPoints(points);
        spline.Close();

        spline.sampleRate = 20;
        spline.type = Spline.Type.CatmullRom;

        // Create child surface object
        surface = new GameObject("Surface", typeof(SurfaceGenerator));
        surface.GetComponent<Renderer>().sharedMaterial = defaultSurfaceMat;
        surface.transform.parent = geo.transform;

        surfaceGenerator = surface.GetComponent<SurfaceGenerator>();
        surfaceGenerator.spline = spline;
        surfaceGenerator.extrude = 0.1f;
        surfaceGenerator.expand = 0.05f;
        surfaceGenerator.offset = new Vector3(0f, -0.1f, 0f);

        // Create child wall object
        wall = new GameObject("Wall", typeof(PathGenerator));
        wall.GetComponent<Renderer>().sharedMaterial = defaultWallMat;
        wall.transform.parent = geo.transform;

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

        geo.GetComponent<MeshRenderer>().enabled = false;
        geo.GetComponent<BoxCollider>().enabled = false;
    }

    List<int> GetIndices(Mesh mesh)
    {
        return new List<int>(mesh.triangles.AsEnumerable().Distinct().Where
            (
                index => mesh.normals[index] == Vector3.up
            ).ToList());
    }

}
