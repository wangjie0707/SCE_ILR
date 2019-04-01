using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Myth
{
    public class CommonButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField]
        private UnityEvent m_OnDown = null;

        [SerializeField]
        private UnityEvent m_OnUp = null;

        [SerializeField]
        private UnityEvent m_OnClick = null;

        private Animation m_Animation;

        private void Awake()
        {
            m_Animation = transform.GetComponent<Animation>();
        }

        public void PlayAnimation()
        {
            if (m_Animation != null)
            {
                m_Animation.Play();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_OnDown.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_OnUp.Invoke();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_OnClick.Invoke();
        }
    }
}
