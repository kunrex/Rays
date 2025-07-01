using System;
using UnityEngine;

namespace RayMarching.Scripts
{
    public sealed class FractalMarcher : MonoBehaviour
    {
        [SerializeField] private Camera cam;
        
        [SerializeField] private Color lightColour;
        [SerializeField] private Vector3 lightPosition;
        
        [SerializeField] private float ambientStrength;
        [SerializeField] private float specularStrength;
        
        [SerializeField] private float shadowAttenuation;

        [SerializeField] private float borderStrength;
        [SerializeField] private Color borderColour;
        [SerializeField] private int borderSteps;
        
        [Range(2, 100), SerializeField] private float renderDistance;
        [SerializeField] private float minimumThreshold;
        
        [Range(2, 16), SerializeField] private float power;
        [Range(2, 16), SerializeField] private int bailout;
        [Range(10, 256), SerializeField] private int iterations;

        [SerializeField] private Color nearColour;
        [SerializeField] private Color farColour;
        
        [SerializeField] private RenderTexture texture;
        [SerializeField] private ComputeShader rayMarchingShader;

        private void Start()
        {
            texture = new RenderTexture (cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            texture.enableRandomWrite = true;
            texture.Create();
        }

        private void SetConstants()
        {
            rayMarchingShader.SetVector("_lightColour", lightColour);
            rayMarchingShader.SetVector("_lightPosition", lightPosition);
        
            rayMarchingShader.SetFloat("_ambientStrength", ambientStrength);
            rayMarchingShader.SetFloat("_specularStrength", specularStrength);
            
            rayMarchingShader.SetFloat("_shadowAttenuation", shadowAttenuation);
        
            rayMarchingShader.SetFloat("_renderDistance", renderDistance);
            rayMarchingShader.SetFloat("_minimumThreshold", minimumThreshold);
            
            rayMarchingShader.SetFloat("power", power);
            rayMarchingShader.SetInt("bailout", bailout);
            rayMarchingShader.SetInt("iterations", iterations);
            
            rayMarchingShader.SetVector("nearColour", nearColour);
            rayMarchingShader.SetVector("farColour", farColour);
            
            rayMarchingShader.SetInt("_borderSteps", borderSteps);
            rayMarchingShader.SetFloat("_borderStrength", borderStrength);
            rayMarchingShader.SetVector("_borderColour", borderColour);
        }
        
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            rayMarchingShader.SetMatrix("cameraToWorld", cam.cameraToWorldMatrix);
            rayMarchingShader.SetMatrix("cameraProjectionInverse", cam.projectionMatrix.inverse);

            SetConstants();
        
            var kernel = rayMarchingShader.FindKernel("CSMain");
               
            rayMarchingShader.SetTexture(kernel, "source", source);
            rayMarchingShader.SetTexture(kernel, "result", texture);
            
            rayMarchingShader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);
            
            Graphics.Blit(texture, destination);
        }
    }
}