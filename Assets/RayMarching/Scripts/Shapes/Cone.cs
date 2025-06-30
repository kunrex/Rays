using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class Cone : Shape
    {
        [SerializeField] private float height;
        [Range(0, 90), SerializeField] private float baseAngle;
        
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
                typeKind = 3,
                position = transform.position,
                
                dimensions2 = new Vector2(baseAngle * Mathf.Deg2Rad, height),
                
                objectColour = baseColour
            });
        }
    }
}