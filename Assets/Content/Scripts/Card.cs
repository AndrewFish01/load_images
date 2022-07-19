using Content.Scripts.Enums;
using Content.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Content.Scripts
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private RawImage _loadImage;
        [SerializeField] private GameObject _faceSide;
        [SerializeField] private GameObject _backSide;

        private CardSide _currentSide = CardSide.Back;

        public CardSide CurrentSide => _currentSide;

        private void OnDestroy()
        {
            _loadImage.texture.Release();
        }

        public void SetImage(Texture2D texture2D)
        {
            _loadImage.texture.Release();
            _loadImage.texture = texture2D;
        }

        public void SetSide(CardSide cardSide)
        {
            if(_currentSide == cardSide) return;
            _currentSide = cardSide;
            ActivatorSide(CardSide.Face == cardSide);
        }

        private void ActivatorSide(bool isFaceSide)
        {
            _faceSide.SetActive(isFaceSide);
            _backSide.SetActive(!isFaceSide);
        }
    }
}