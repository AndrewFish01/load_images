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
    public class AllAtOnceCommand : ICommand
    {
        private List<Card> _cards;
        private ImageDownloader _imageDownloader;
        private CardFlipper _cardFlipper;
        private CancellationTokenSource _cts;
        
        public AllAtOnceCommand(List<Card> cards, ImageDownloader imageDownloader)
        {
            _cards = cards;
            _cts = new CancellationTokenSource();
            _cardFlipper = new CardFlipper(new Vector3(0, 90, 0), 100, 0.3f, Ease.InBack);
            _imageDownloader = imageDownloader;
        }
        
        ~AllAtOnceCommand()
        {
            Debug.Log("destructor invoke AllAtOnceCommand");
        }

        public async UniTask ExecuteAsync()
        {
            await _cardFlipper.FlipCardListAsync(_cards, CardSide.Back);
            
            try
            {
                var download = _cards.Select(DownloadSelector);
                await UniTask.WhenAll(download);
                await _cardFlipper.FlipCardListAsync(_cards, CardSide.Face);
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
        }

        public void Undo()
        {
            _cts?.Cancel();
            _cardFlipper.FlipCardListAsync(_cards, CardSide.Back).Forget();
        }
    }
}