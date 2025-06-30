using System.Collections.Generic;
using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class Box : Shape
    {
        [SerializeField] private float length;
        [SerializeField] private float breadth;
        [SerializeField] private float height;

        [SerializeField] private float borderRadius;

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
                typeKind = 0,
                position = transform.position,
                
                dimensions1 = borderRadius,
                dimensions3 = new Vector3(length, breadth, height),
                
                objectColour = baseColour
            });
        }
    }
}