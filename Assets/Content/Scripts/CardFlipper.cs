using System.Collections.Generic;
using Content.Scripts.Enums;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts
{
    public class CardFlipper
    {
        private float _duration;
        private Vector3 _endRotateValue;
        private float _endMoveValue;
        private Ease _easeMode;
        public CardFlipper(Vector3 endRotateValue, float endMoveValue, float durationAnim, Ease easeMode)
        {
            _endRotateValue = endRotateValue;
            _endMoveValue = endMoveValue;
            _duration = durationAnim;
            _easeMode = easeMode;
        }

        public async UniTask FlipCardAsync(Card card, CardSide cardSide)
        {
            if (card.CurrentSide == cardSide) return;
            await PlayFlippingAnim(card.transform, _endRotateValue, _endMoveValue);
            card.SetSide(cardSide);
            await PlayFlippingAnim(card.transform, Vector3.zero, 0);
        }

        public async UniTask FlipCardListAsync(List<Card> cards, CardSide cardSide)
        {
            var flippingCards = cards.Select(card => FlipCardAsync(card, cardSide));
            await UniTask.WhenAll(flippingCards);
        }

        private async UniTask PlayFlippingAnim(Transform card, Vector3 endRotate, float endMove)
        {
            var mySequence = DOTween.Sequence();
            await mySequence.Join(card.DORotate(endRotate, _duration))
                .Join(card.DOLocalMoveY(endMove, _duration))
                .SetEase(_easeMode)
                .AsyncWaitForCompletion();
        }
    }
}