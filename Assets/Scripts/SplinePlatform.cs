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
        wallUVScale = wall.uvScale;
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
        wall.uvScale = wallUVScale;
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
    bool showSurfaceParameters;
    bool showWallParameters;

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
        //DrawDefaultInspector();

        showMaterialParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showMaterialParameters, "Materials");
        if(showMaterialParameters)
        {
            EditorGUI.BeginChangeCheck();

            script.surfaceMat = EditorGUILayout.ObjectField("Surface Material", script.surfaceMat, typeof(Material), false) as Material;
            script.wallMat = EditorGUILayout.ObjectField("Wall Material", script.wallMat, typeof(Material), false) as Material;

            if(EditorGUI.EndChangeCheck()) 
            { 
                Undo.RecordObject(target, "Changed Mats"); 
                script.SyncMaterials(); 
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        showSurfaceParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showSurfaceParameters, "Surface");
        if(showSurfaceParameters)
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            script.surfaceUVScale = EditorGUILayout.Vector2Field("UV Scale", script.surfaceUVScale);
            script.surfaceSideUVScale = EditorGUILayout.Vector2Field("Side UV Scale", script.surfaceSideUVScale);

            if (EditorGUI.EndChangeCheck()) 
            { 
                Undo.RecordObject(target, "Changed UV"); 
                script.SyncUV(); 
            }

        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        EditorGUILayout.Space();

        showWallParameters = EditorGUILayout.BeginFoldoutHeaderGroup(showWallParameters, "Wall");
        if (showWallParameters)
        {
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Shape", EditorStyles.boldLabel);
            float depth = EditorGUILayout.FloatField("Platform Depth", script.platformDepth);

            if (EditorGUI.EndChangeCheck()) 
            { 
                Undo.RecordObject(target, "Changed Shape");
                script.platformDepth = depth;
                script.SyncShape();
            }

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("UV Coordinates", EditorStyles.boldLabel);
            script.wallUVScale = EditorGUILayout.Vector2Field("Side UV Scale", script.wallUVScale);

            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject(target, "Changed UV");
                script.SyncUV(); 
            }

        }
        EditorGUILayout.EndFoldoutHeaderGroup();


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


