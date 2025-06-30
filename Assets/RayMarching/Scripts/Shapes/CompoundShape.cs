using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public sealed class CompoundShape : Shape
    {
        [Range(0, 4), SerializeField] private int blendKind;
        [SerializeField] private List<Shape> children;
        
        public override ObjectData AsObjectData()
        {
            return new ObjectData()
            {
                blendKind = blendKind,
                shapeCount = children.Count
            };
        }

        public override void PushShapeData(in List<ShapeData> data)
        {
            foreach(var child in children)
                child.PushShapeData(data);
        }
    }
}