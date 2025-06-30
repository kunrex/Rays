using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using UnityEngine;

using RayMarching.Shapes;

public sealed class RayMarcher : MonoBehaviour
{
    private static RayMarcher instance;
    
    public static RayMarcher Instance
    {
        get => instance;
    }
    
    [SerializeField] private Camera cam;
    
    [SerializeField] private Color lightColour;
    [SerializeField] private Vector3 lightPosition;

    [SerializeField] private float ambientStrength;
    [SerializeField] private float specularStrength;
    
    [SerializeField] private float renderDistance;
    [SerializeField] private float minimumThreshold;

    [SerializeField] private float blendStrength;
    [SerializeField] private float shadowAttenuation;
    
    [SerializeField] private RenderTexture texture;
    [SerializeField] private ComputeShader rayMarchingShader;
    
    private List<Shape> shapes;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            
            shapes = new ();
            
            DontDestroyOnLoad(gameObject);
            return;
        }
        
        Destroy(gameObject);
    }
    private void Start()
    {
        texture = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        texture.enableRandomWrite = true;
        texture.Create();

        SetConstants();
    }

    private void SetConstants()
    {
        rayMarchingShader.SetVector("_lightColour", lightColour);
        rayMarchingShader.SetVector("_lightPosition", lightPosition);
        
        rayMarchingShader.SetFloat("_ambientStrength", ambientStrength);
        rayMarchingShader.SetFloat("_specularStrength", specularStrength);
        
        rayMarchingShader.SetFloat("_renderDistance", renderDistance);
        rayMarchingShader.SetFloat("_minimumThreshold", minimumThreshold);
        
        rayMarchingShader.SetFloat("_shadowAttenuation", shadowAttenuation);
        
        rayMarchingShader.SetFloat("blendStrength", blendStrength);
    }

    public void AddShape(Shape shape)
    {
        shapes.Add(shape);
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        rayMarchingShader.SetMatrix("cameraToWorld", cam.cameraToWorldMatrix);
        rayMarchingShader.SetMatrix("cameraProjectionInverse", cam.projectionMatrix.inverse);

        SetConstants();
        
        var kernel = rayMarchingShader.FindKernel("CSMain");
        
        rayMarchingShader.SetTexture(kernel, "source", source);
        rayMarchingShader.SetTexture(kernel, "result", texture);

        int currentShape = 0;
        var objectData = shapes.Select(x =>
        {
            var data = x.AsObjectData();
            data.startIndex = currentShape;

            currentShape += data.shapeCount;
            return data;
        }).ToArray();
        
        var shapeData = new List<ShapeData>();
        shapes.ForEach(x => x.PushShapeData(shapeData));
        
        var objectBuffer = new ComputeBuffer(objectData.Length, Marshal.SizeOf(typeof(ObjectData)));
        objectBuffer.SetData(objectData);
        
        rayMarchingShader.SetFloat("objectCount", objectData.Length);
        rayMarchingShader.SetBuffer(kernel, "objects", objectBuffer);
        
        var shapeBuffer = new ComputeBuffer(shapeData.Count, Marshal.SizeOf(typeof(ShapeData)));
        shapeBuffer.SetData(shapeData);
        
        rayMarchingShader.SetFloat("shapeCount", shapeData.Count);
        rayMarchingShader.SetBuffer(kernel, "shapes", shapeBuffer);
        
        rayMarchingShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
        
        shapeBuffer.Dispose();
        objectBuffer.Dispose();
        Graphics.Blit(texture, destination);
    }
}
