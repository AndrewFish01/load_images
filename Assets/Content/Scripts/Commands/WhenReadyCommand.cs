using System;
using System.Collections.Generic;
using System.Threading;
using Content.Scripts.Enums;
using Content.Scripts.Network;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Content.Scripts.Commands
{
    public class WhenReadyCommand : ICommand
    {
        private List<Card> _cards;
        private ImageDownloader _imageDownloader;
        private CardFlipper _cardFlipper;
        private CancellationTokenSource _cts;

        public WhenReadyCommand(List<Card> cards, ImageDownloader imageDownloader)
        {
            _cards = cards;
            _imageDownloader = imageDownloader;
            _cardFlipper = new CardFlipper(new Vector3(0, 90, 0), 100, 0.3f, Ease.InBack);          
            _cts = new CancellationTokenSource();
        }

        public async UniTask ExecuteAsync()
        {
            await _cardFlipper.FlipCardListAsync(_cards, CardSide.Back);
            try
            {
                await _cards.Select(DownloadSelector);
            }
            catch (OperationCanceledException e)
            {
                if (_cts.Token == e.CancellationToken)
                {
                    Debug.LogError("Cancel load!");
                    _cardFlipper.FlipCardListAsync(_cards, CardSide.Back).Forget();
                }
                else Debug.LogError(e);
            }
            finally
            {
                _cts.Dispose();
                _cts = null;
            }
        }
        
        private async UniTask DownloadSelector(Card card)
        {
            var downloadImage = _imageDownloader.DownloadImageAsync(APIConstants.PicsumPhoto, _cts.Token);
            card.SetImage(await downloadImage);
            await _cardFlipper.FlipCardAsync(card, CardSide.Face);
        }

        public void Undo()
        {
            _cts?.Cancel();
        }
    }
}