using UnityEngine;

namespace HotFixEntry
{
    public interface IGameObj
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }
    }

    public class CGameObj : IGameObj
    {
        private Transform m_transform;
        public CGameObj(Transform a_transform)
        {
            m_transform = a_transform;
        }

        public Transform transform => m_transform;

        public GameObject gameObject => transform.gameObject;
    }
}