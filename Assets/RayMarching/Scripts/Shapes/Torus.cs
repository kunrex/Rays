using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class Torus : Shape
    {
        [SerializeField] private float majorRadius;
        [SerializeField] private float minorRadius;
        
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
                typeKind = 2,
                position = transform.position,
                
                dimensions2 = new Vector2(majorRadius, minorRadius),
                
                objectColour = baseColour
            });
        }
    }
}