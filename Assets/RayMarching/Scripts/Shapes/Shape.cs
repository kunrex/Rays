using System.Collections.Generic;

using UnityEngine;

namespace RayMarching.Shapes
{
    public abstract class Shape : MonoBehaviour
    {
        [SerializeField] private bool isChild;
        [SerializeField] protected Color baseColour;
        
        private void Start()
        {
            if(!isChild)
                RayMarcher.Instance.AddShape(this);
        }
        
        public abstract ObjectData AsObjectData();
        public abstract void PushShapeData(in List<ShapeData> data);
    }
}