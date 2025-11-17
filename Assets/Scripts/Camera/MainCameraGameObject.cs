using UnityEngine;

namespace Camera
{
    public class MainCameraGameObject : MonoBehaviour
    {
        public static UnityEngine.Camera Instance;

        void Awake()
        {
            Instance = GetComponent<UnityEngine.Camera>();
        }
    }
}