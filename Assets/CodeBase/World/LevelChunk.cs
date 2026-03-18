using UnityEngine;

namespace EventBus
{
    public class LevelChunk : MonoBehaviour
    {
        public void PlaceAt(float zPosition)
        {
            transform.position = new Vector3(0f, 0f, zPosition);
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}