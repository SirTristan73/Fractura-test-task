using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EventBus
{
    public class BaseButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private ButtonEvent _thisButtonEvent;
        [SerializeField] private TMP_Text _buttonText;


        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
            SetText();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            EventBus.Trigger(new OnClickEvent(_thisButtonEvent));
        }

        private void SetText()
        {
            _buttonText.text = _thisButtonEvent.ToString();
        }
    }
}
