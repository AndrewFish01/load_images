using System.Collections.Generic;
using Content.Scripts.Enums;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Content.Scripts.UI
{
    public class MainGamePanel : MonoBehaviour
    {
        [SerializeField] private Button _buttonAllAtOnce;
        [SerializeField] private Button _buttonOneByOne;
        [SerializeField] private Button _buttonWhenReady;
        [SerializeField] private Button _buttonCancel;

        [SerializeField] private List<Card> _cards;
        
        public List<Card> GetCards() => _cards;
        public Button ButtonCancel => _buttonCancel;
        public event UnityAction<DownloadType> ClickMode;
        public event UnityAction ClickCancel;

        private void OnEnable()
        {
            _buttonAllAtOnce.onClick.AddListener(OnClickAllAtOnce);
            _buttonOneByOne.onClick.AddListener(OnClickOneByOne);
            _buttonWhenReady.onClick.AddListener(OnClickWhenReady);
            _buttonCancel.onClick.AddListener(OnClickCancel);
        }

        private void OnDisable()
        {
            _buttonAllAtOnce.onClick.RemoveListener(OnClickAllAtOnce);
            _buttonOneByOne.onClick.RemoveListener(OnClickOneByOne);
            _buttonWhenReady.onClick.RemoveListener(OnClickWhenReady);
            _buttonCancel.onClick.RemoveListener(OnClickCancel);
        }

        private void OnDestroy()
        {
            _cards.Clear();
        }

        private void OnClickAllAtOnce() => ClickMode?.Invoke(DownloadType.AllAtOnce);
        private void OnClickOneByOne() => ClickMode?.Invoke(DownloadType.OneByOne);
        private void OnClickWhenReady() => ClickMode?.Invoke(DownloadType.WhenReady);
        private void OnClickCancel() => ClickCancel?.Invoke();

        public void ActivatorModeButtons(bool isActivate)
        {
            _buttonAllAtOnce.interactable = isActivate;
            _buttonOneByOne.interactable = isActivate;
            _buttonWhenReady.interactable = isActivate;
            _buttonCancel.interactable = !isActivate;
        }
    }
}