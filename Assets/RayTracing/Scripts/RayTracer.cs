using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RayTracing.Scripts
{
    public sealed class RayTracer : MonoBehaviour
    {
        public static RayTracer instance;
        
        [SerializeField] private Camera cam;
        [SerializeField] private ComputeShader rayTracingShader;
        
        [SerializeField] private float maximumRenderThreshold;
        [SerializeField] private float minimumRenderThreshold;

        [SerializeField] private int pixelRayCount;
        [SerializeField] private int childRayCount;

        [SerializeField, Range(0, 1)] private float ambientStrength;
        [SerializeField, Min(1)] private float ambientRefractiveIndex;
        
        [SerializeField] private Color ambientColorSky;
        [SerializeField] private Color ambientColorHorizon;
        
        private Sphere[] spheres;
        private RenderTexture texture;

        private int kernel;

        private int resultTextureIndex;
        
        private int cameraToWorldIndex;
        private int cameraProjectionInverseIndex;

        private int maximumRenderThresholdIndex;
        private int minimumRenderThresholdIndex;
        
        private int sphereCountIndex;
        private int sphereBufferIndex;
        
        private int childRayCountIndex;
        private int pixelRayCountIndex;

        private int ambientStrengthIndex;
        private int ambientRefractiveIndexIndex;
        
        private int ambientColorSkyIndex;
        private int ambientColorHorizonIndex;
        
        private List<SphereData> sphereData;

        public static RayTracer Instance
        {
            get => instance;
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                return;
            }
            
            Destroy(gameObject);
        }
        
        private void Start()
        {
            texture = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            texture.enableRandomWrite = true;
            texture.Create();

            sphereData = new List<SphereData>();
            spheres = FindObjectsByType<Sphere>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            
            SetConstants();
        }

        private void SetConstants()
        {
            kernel = rayTracingShader.FindKernel("CSMain");

            cameraToWorldIndex = Shader.PropertyToID("cameraToWorld");
            cameraProjectionInverseIndex = Shader.PropertyToID("cameraProjectionInverse");
            
            resultTextureIndex = Shader.PropertyToID("result");
            
            minimumRenderThresholdIndex = Shader.PropertyToID("_minimumRenderThreshold");
            maximumRenderThresholdIndex = Shader.PropertyToID("_maximumRenderThreshold");

            sphereCountIndex = Shader.PropertyToID("sphereCount");
            sphereBufferIndex = Shader.PropertyToID("sphereBuffer");

            pixelRayCountIndex = Shader.PropertyToID("_pixelRayCount");
            childRayCountIndex = Shader.PropertyToID("_childRayCount");

            ambientStrengthIndex = Shader.PropertyToID("_ambientStrength");
            ambientRefractiveIndexIndex = Shader.PropertyToID("_ambientRefractiveIndex");
            
            ambientColorSkyIndex = Shader.PropertyToID("_ambientColorSky");
            ambientColorHorizonIndex = Shader.PropertyToID("_ambientColorHorizon");
        }

        private void WriteConstants()
        {
            rayTracingShader.SetMatrix(cameraToWorldIndex, cam.cameraToWorldMatrix);
            rayTracingShader.SetMatrix(cameraProjectionInverseIndex, cam.projectionMatrix.inverse);
            
            rayTracingShader.SetFloat(minimumRenderThresholdIndex, minimumRenderThreshold);
            rayTracingShader.SetFloat(maximumRenderThresholdIndex, maximumRenderThreshold);
            
            rayTracingShader.SetInt(pixelRayCountIndex, pixelRayCount);
            rayTracingShader.SetInt(childRayCountIndex, childRayCount);
            
            rayTracingShader.SetFloat(ambientStrengthIndex, ambientStrength);
            rayTracingShader.SetFloat(ambientRefractiveIndexIndex, ambientRefractiveIndex);
            
            rayTracingShader.SetVector(ambientColorSkyIndex, ambientColorSky);
            rayTracingShader.SetVector(ambientColorHorizonIndex, ambientColorHorizon);
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            WriteConstants();
            
            sphereData.Clear();
            foreach (var sphere in spheres)
            {
                sphereData.Add(new SphereData(sphere)); 
            }
            
            var buffer = new ComputeBuffer(spheres.Length, Marshal.SizeOf(typeof(SphereData)));
            buffer.SetData(sphereData);
            
            rayTracingShader.SetInt(sphereCountIndex, sphereData.Count);
            rayTracingShader.SetBuffer(kernel, sphereBufferIndex, buffer); 
            
            rayTracingShader.SetTexture(kernel, resultTextureIndex, texture);
            
            rayTracingShader.Dispatch(kernel, texture.height / 8, texture.width / 8, 1);
            Graphics.Blit(texture, destination);
            
            buffer.Dispose(); 
        }
    }
}