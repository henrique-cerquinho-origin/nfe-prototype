using Unity.Entities;
using UnityEngine;

namespace Camera
{
    public class CinemachineBridgeComponent : IComponentData
    {
        public Transform LookAtTransform;
    }
}