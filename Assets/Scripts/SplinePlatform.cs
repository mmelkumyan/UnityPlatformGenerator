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
    [HideInInspector] public SurfaceGenerator wall;

    public Material surfaceMat;
    public Material wallMat;

    public float platformDepth;

    public Vector2 surfaceUVScale;
    public Vector2 surfaceSideUVScale;
    public Vector2 wallUVScale;

    internal void SyncEditor()
    {
        platformDepth = -wall.extrude;
        surfaceUVScale = surface.uvScale;
        surfaceSideUVScale = surface.sideUvScale;
        wallUVScale = wall.sideUvScale;
    }

    internal void SyncMaterials()
    {
        surface.GetComponent<MeshRenderer>().material = surfaceMat;
        wall.GetComponent<MeshRenderer>().material = wallMat;
    }

    internal void SyncUV()
    {
        surface.uvScale = surfaceUVScale;
        surface.sideUvScale = surfaceSideUVScale;
        wall.sideUvScale = wallUVScale;
    }

    internal void SyncShape()
    {
        wall.extrude = -platformDepth;
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

    private void OnEnable()
    {
        script = (SplinePlatform)target;
        script.SyncEditor();

        Undo.undoRedoPerformed += SyncAll;

    }

    private void SyncAll()
    {
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
            script.surfaceMat = EditorGUILayout.ObjectField("Surface Material", script.surfaceMat, typeof(Material), false) as Material;
            script.wallMat = EditorGUILayout.ObjectField("Wall Material", script.wallMat, typeof(Material), false) as Material;
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Surface section
        showSurfaceParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showSurfaceParameters, "Surface");
        if(showSurfaceParameters) {
            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            script.surfaceUVScale = EditorGUILayout.Vector2Field("UV Scale", script.surfaceUVScale);
            script.surfaceSideUVScale = EditorGUILayout.Vector2Field("Side UV Scale", script.surfaceSideUVScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        // Wall section
        showWallParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showWallParameters, "Wall");
        if (showWallParameters) {
            EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
            script.platformDepth = EditorGUILayout.FloatField("Platform Depth", script.platformDepth);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            script.wallUVScale = EditorGUILayout.Vector2Field("Side UV Scale", script.wallUVScale);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        if (EditorGUI.EndChangeCheck()) {
            SyncAll();            
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


