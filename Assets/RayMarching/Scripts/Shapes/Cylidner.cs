using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class Cylinder : Shape
    {
        [SerializeField] private float height;
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
                typeKind = 4,
                position = transform.position,
                
                dimensions2 = new Vector2(radius, height),
                
                objectColour = baseColour
            });
        }
    }
}