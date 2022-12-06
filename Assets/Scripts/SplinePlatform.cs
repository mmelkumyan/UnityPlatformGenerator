using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;

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
[Serializable]
public class SplinePlatform : MonoBehaviour
{
    [HideInInspector] public SurfaceGenerator surface;
    [HideInInspector] public PathGenerator wall;

    public Material surfaceMat;
    public Material wallMat;

    public float surfaceDepth;
    public float surfaceExpand;
    
    public float wallDepth;
    public AnimationCurve wallCurve;
    public float wallCurveScale;
    public int wallResolution;

    public Vector2 surfaceUVScale;
    public Vector2 surfaceSideUVScale;
    public Vector2 wallUVScale;

    public bool bakeButtonToggle = true;
    
    internal void SyncEditor()
    {
        // Initialize editor with component values
        // Surface shape
        surfaceDepth = surface.extrude;
        surfaceExpand = surface.expand;

        // Wall shape
        wallDepth = wall.size;
        wallCurve = wall.shape;
        wallCurveScale = wall.shapeExposure;
        wallResolution = wall.slices;
        
        // UVs
        surfaceUVScale = surface.uvScale;
        surfaceSideUVScale = surface.sideUvScale;
        wallUVScale = wall.uvScale;

        // Materials
        surfaceMat = surface.GetComponent<Renderer>().sharedMaterial;
        wallMat = wall.GetComponent<Renderer>().sharedMaterial;
    }

    internal void SyncShape()
    {
        // Surface
        surface.extrude = surfaceDepth;
        surface.offset = new Vector3(0f, -surfaceDepth, 0f);
        surface.expand = surfaceExpand;
        
        // Walls
        wall.size = wallDepth;
        wall.offset = new Vector3(0f, -wallDepth/2-surfaceDepth, 0f);
        wall.shape = wallCurve;
        wall.shapeExposure = wallCurveScale;
        wall.slices = wallResolution;
    }
    
    internal void SyncUV()
    {
        surface.uvScale = surfaceUVScale;
        surface.sideUvScale = surfaceSideUVScale;
        wall.uvScale = wallUVScale;
    }
    
    internal void SyncMaterials()
    {
        surface.GetComponent<MeshRenderer>().material = surfaceMat;
        wall.GetComponent<MeshRenderer>().material = wallMat;
    }

    internal void BakeMeshes() {
        surface.Bake(true, false);
        wall.Bake(true, false);
    }

    internal void UnbakeMeshes() {
        surface.Unbake();
        wall.Unbake();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SplinePlatform))]
public class SplinePlatformEditor : Editor
{
    SplinePlatform script;

    bool showMaterialParameters = true;
    bool showSurfaceParameters = true;
    bool showWallParameters = true;

    Material surfaceMat, wallMat;
    AnimationCurve wallCurve;
    Vector2 surfaceUVScale, surfaceSideUVScale, wallUVScale;
    float surfaceDepth, surfaceExpand, wallDepth, wallCurveScale;
    int wallResolution;

    private void OnEnable()
    {
        script = (SplinePlatform)target;
        script.SyncEditor();

        Undo.undoRedoPerformed += SyncAll;
    }

    private void SyncAll()
    {
        if (script == null)
            return;

        script.SyncMaterials();
        script.SyncUV();
        script.SyncShape();
    }

    public override void OnInspectorGUI()
    {
        // DrawDefaultInspector();
        EditorGUI.BeginChangeCheck();

        // Material section
        showMaterialParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterialParameters, "Materials");
        if(showMaterialParameters) {
            surfaceMat = EditorGUILayout.ObjectField("Surface Material", script.surfaceMat, typeof(Material), false) as Material;
            wallMat = EditorGUILayout.ObjectField("Wall Material", script.wallMat, typeof(Material), false) as Material;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Surface section
        showSurfaceParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showSurfaceParameters, "Surface");
        if(showSurfaceParameters) {
            EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
            surfaceDepth = EditorGUILayout.FloatField("Depth", script.surfaceDepth);
            surfaceExpand = EditorGUILayout.FloatField("Expand", script.surfaceExpand);
            
            EditorGUILayout.Space();
            
            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            surfaceUVScale = EditorGUILayout.Vector2Field("UV Scale", script.surfaceUVScale);
            surfaceSideUVScale = EditorGUILayout.Vector2Field("Side UV Scale", script.surfaceSideUVScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Wall section
        showWallParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showWallParameters, "Wall");
        if (showWallParameters) {
            EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
            wallDepth = EditorGUILayout.FloatField("Depth", script.wallDepth);
            wallCurve = EditorGUILayout.CurveField("Curve", script.wallCurve);
            wallCurveScale = EditorGUILayout.FloatField("Curve Scale", script.wallCurveScale);
            wallResolution = EditorGUILayout.IntField("Resolution", script.wallResolution);
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            wallUVScale = EditorGUILayout.Vector2Field("UV Scale", script.wallUVScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();
        
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Platform Change");

            script.surfaceMat = surfaceMat;
            script.wallMat = wallMat;

            script.surfaceDepth = surfaceDepth;
            script.surfaceExpand = surfaceExpand;

            script.wallDepth = wallDepth;
            script.wallCurveScale = wallCurveScale;
            script.wallCurve = wallCurve;
            script.wallResolution = wallResolution;

            script.surfaceUVScale = surfaceUVScale;
            script.wallUVScale = wallUVScale;
            script.surfaceSideUVScale = surfaceSideUVScale;

            SyncAll();            
        }

        // Bake/ Rever Bake Toggle
        if (script.bakeButtonToggle) {
            if (GUILayout.Button("Bake Meshes")) {
                script.BakeMeshes();
                script.bakeButtonToggle = !script.bakeButtonToggle;
            }
        }
        else {
            if (GUILayout.Button("Revert Bake")) {
                script.UnbakeMeshes();
                script.bakeButtonToggle = !script.bakeButtonToggle;
            }
        }
        
    }



    /*
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement inspector = new VisualElement();

        Foldout materialView = new Foldout();
        materialView.name = "Materials";
        materialView.text = "Materials";
        //materialView.Add(EditorGUILayout.ObjectField(script.surfaceMat, typeof(Material), false));

        inspector.Add(materialView);
        return inspector;
    }*/
}
#endif


