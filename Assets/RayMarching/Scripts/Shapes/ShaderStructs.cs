using UnityEngine;

namespace RayMarching.Shapes
{
    public struct ShapeData
    {
        public int typeKind;

        public Vector3 position;

        public float dimensions1;
        public Vector2 dimensions2;
        public Vector3 dimensions3;

        public Color objectColour;
    }

    public struct ObjectData
    {
        public int blendKind;
        
        public int shapeCount;
        public int startIndex;
    }
}