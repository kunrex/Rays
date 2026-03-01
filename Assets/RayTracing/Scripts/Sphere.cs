using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RayTracing.Scripts
{
    public class Sphere : MonoBehaviour
    {
        public SphereMaterial material;

        public Vector3 center
        {
            get => transform.position;
        }

        public float radius
        {
            get => transform.localScale.x / 2;
        }
    }

    public enum MaterialType : int
    {
        Lambertian = 0,
        Metallic = 1
    }

    
    [Serializable] 
    public struct SphereMaterial
    {
        public MaterialType materialType;
        
        public Color albedo;
        public Color emissionColor;

        [Range(0, 1)] public float fuzzStrength;
        public float diffuseStrength;
        public float emissionStrength;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SphereData
    {
        public float radius;
        public Vector3 center;
        
        public int materialType;
                
        public Vector4 albedo;
        public Vector4 emissionColor;

        public float fuzzStrength;
        public float diffuseStrength;
        public float emissionStrength;
                
        public SphereData(Sphere sphere)
        {
            radius = sphere.radius;
            center = sphere.center;
            
            materialType = (int)sphere.material.materialType;

            albedo = sphere.material.albedo;
            emissionColor = sphere.material.emissionColor;

            fuzzStrength = sphere.material.fuzzStrength;
            diffuseStrength = sphere.material.diffuseStrength;
            emissionStrength = sphere.material.emissionStrength;
        }
    }
}