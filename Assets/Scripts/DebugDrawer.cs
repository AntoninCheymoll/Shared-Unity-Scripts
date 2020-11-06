using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DebugDrawer : MonoBehaviour
{
    private class Point
    {
        public Color color;
        public float size;
        public Vector3 position;
        public bool updateEachFrame;
        
        public Point(Vector3 position, Color color, float size, bool updateEachFrame)
        {
            this.position = position;
            this.size = size;
            this.color = color;
            this.updateEachFrame = updateEachFrame;
        }
    }

    private class Vector
    {
        public Color color;
        public Vector3 value;
        public Vector3 initialPosition;
        public bool updateEachFrame;

        public Vector(Vector3 value, Vector3 initialPosition, Color color, bool updateEachFrame)
        {
            this.color = color;
            this.value = value;
            this.initialPosition = initialPosition;
            this.updateEachFrame = updateEachFrame;
        }
    }

    List<GameObject> toRemoveGO = new List<GameObject>();
    List<LineRenderer> toRemoveLR = new List<LineRenderer>();

    List<Point> points = new List<Point>();

    List<Vector> vectors = new List<Vector>();

    [Header("Point parameters")]
    public Color defaultPointColor = Color.red;
    public float defaultPointSize = 0.05f;

    [Header("Vector parameters")]
    public Color defaultVectorColor = Color.blue;
    public float defaultVectorLength = 1;
    public float vectorSphereSize = 0.05f;
    public float vectorRayWidth = 0.01f;



    void Update()
    {

        foreach (GameObject go in toRemoveGO) Destroy(go);
        toRemoveGO.Clear();

        foreach (LineRenderer go in toRemoveLR) Destroy(go);
        toRemoveLR.Clear();

        //point 
        foreach (Point p in points)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            
            sphere.gameObject.transform.position = p.position;
            sphere.GetComponent<Renderer>().material.color = p.color;
            sphere.transform.localScale = new Vector3(p.size, p.size, p.size);

            DestroyImmediate(sphere.GetComponent<Collider>());
            if (p.updateEachFrame) toRemoveGO.Add(sphere);

        }
        points = new List<Point>();

        //vector
        foreach (Vector v in vectors)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

            sphere.gameObject.transform.position = v.initialPosition + v.value;
            sphere.GetComponent<Renderer>().material.color = v.color;
            sphere.transform.localScale = new Vector3(vectorSphereSize, vectorSphereSize, vectorSphereSize);

            DestroyImmediate(sphere.GetComponent<Collider>());
            if (v.updateEachFrame) toRemoveGO.Add(sphere);

            LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
            lineRenderer.material.color = v.color;
            lineRenderer.startWidth = vectorRayWidth;
            lineRenderer.endWidth = vectorRayWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;

            lineRenderer.SetPosition(0, v.initialPosition);
            lineRenderer.SetPosition(1, v.initialPosition + v.value);

            if (v.updateEachFrame) toRemoveLR.Add(lineRenderer);


        }

        vectors = new List<Vector>();
    }

    public void AddPoint(Vector3 position, bool updateEachFrame, Color color = default, float size = default)
    {
        if (color == default) color = defaultPointColor;
        if (size == default) size = defaultPointSize;
        points.Add(new Point(position, color, size, updateEachFrame));
    }

    public void AddVector(Vector3 value, Vector3 initialPosition, bool updateEachFrame, Color color = default, float length = default)
    {
        if (color == default) color = defaultVectorColor;
        value = (length != default)?value.normalized * length : value.normalized *  defaultVectorLength;

        vectors.Add(new Vector(value, initialPosition, color, updateEachFrame));
    }
}
