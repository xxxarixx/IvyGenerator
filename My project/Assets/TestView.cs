using Ui.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace Ui.View
{
    class TestView : MonoBehaviour
    {
        [SerializeField]
        Toggle _toggle;

        [SerializeField]
        Text _text;

        Transform _toggleContainer;

        [SerializeField]
        RectTransform _rect;

        int Id;
        void OnDestroy()
        {
            _toggleContainer.GetChild(Id)
                            .GetComponent<Toggle>().onValueChanged
                            .RemoveListener(value => TestController.ToggleStateChanged(value, Id));
        }

        internal void CreateToggleCopy(bool initialValue, string text, Transform toggleContainer)
        {
            if (_toggleContainer == null)
                _toggleContainer = toggleContainer;
            TestView toggleView = Instantiate(gameObject, _toggleContainer).GetComponent<TestView>();
            toggleView._toggle.isOn = initialValue;
            toggleView._toggle.onValueChanged.AddListener(value => TestController.ToggleStateChanged(value, Id));
            toggleView._text.text = text;
            toggleView._toggleContainer = toggleContainer;
            toggleView.Id = toggleView.transform.GetSiblingIndex();
            toggleView._rect.anchoredPosition = new Vector3(0f, toggleView.Id * toggleView._rect.sizeDelta.y, 0f);
        }
    }
}
