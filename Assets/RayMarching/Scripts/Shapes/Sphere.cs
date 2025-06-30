using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class Sphere : Shape
    {
        [SerializeField] private float radius;

        public override ObjectData AsObjectData()
        {
            return new ObjectData()
            {
                blendKind = 2,
                shapeCount = 1
            };
        }
        
        public override void PushShapeData(in List<ShapeData> data)
        {
            data.Add(new ShapeData()
            {
                typeKind = 1, 
                position = transform.position,
                
                dimensions1 = radius,
                
                objectColour = baseColour
            });
        }
    }
}