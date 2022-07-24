using System;
using System.Collections.Generic;
using Content.Scripts.Commands;
using Content.Scripts.Enums;
using Content.Scripts.Network;
using Content.Scripts.UI;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using UnityEngine;

namespace Content.Scripts
{
    public class GameController
    {
        private MainGamePanel _mainPanel;
        private ImageDownloader _imageDownloader;
        private ICommand _currentCommand;
        private List<Card> _cards;
        private bool _isCompleted;

        public GameController(MainGamePanel mainPanel)
        {
            _mainPanel = mainPanel;
            _cards = mainPanel.GetCards();
            _imageDownloader = new ImageDownloader();
            _isCompleted = true;
            SubscribeOnEvents();
            InitGame();
        }

        ~GameController()
        {
            _cards.Clear();
            UnsubscribeOnEvents();
        }

        private void InitGame()
        {
            _mainPanel.ActivatorModeButtons(true);
        }

        private void SubscribeOnEvents()
        {
            _mainPanel.ClickCancel += OnClickCancel;
            
            _mainPanel.ButtonAllAtOnce.OnClickAsAsyncEnumerable().ForEachAwaitAsync(
                async x =>
                {
                    Debug.Log("Click AllAtOnce");
                    await ExecuteCommandAsync(DownloadType.AllAtOnce);
                });
            
            _mainPanel.ButtonOneByOne.OnClickAsAsyncEnumerable().ForEachAwaitAsync(
                async x =>
                {
                    Debug.Log("Click OneByOne");
                    await ExecuteCommandAsync(DownloadType.OneByOne);
                });
            
            _mainPanel.ButtonWhenReady.OnClickAsAsyncEnumerable().ForEachAwaitAsync(
                async x =>
                {
                    Debug.Log("Click WhenReady");
                    await ExecuteCommandAsync(DownloadType.WhenReady);
                });
        }

        private void UnsubscribeOnEvents()
        {
            _mainPanel.ClickCancel -= OnClickCancel;
            //_mainPanel.ClickMode -= OnClickMode;
        }
        
        private async UniTask ExecuteCommandAsync(DownloadType type)
        {
            if(!_isCompleted) return;
            _currentCommand = type switch
            {
                DownloadType.AllAtOnce => new AllAtOnceCommand(_cards, _imageDownloader),
                DownloadType.OneByOne => new OneByOneCommand(_cards, _imageDownloader),
                DownloadType.WhenReady => new WhenReadyCommand(_cards, _imageDownloader),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Not find download type")
            };
            _isCompleted = false;
            _mainPanel.ButtonCancel.interactable = true;
            
            await _currentCommand.ExecuteAsync().ContinueWith(() =>
            {
                Debug.Log("_isCompleted");
                _isCompleted = true;
            });
            
            _mainPanel.ButtonCancel.interactable = false;
        }

        private void OnClickCancel()
        {
            _currentCommand?.Undo();
            _mainPanel.ButtonCancel.interactable = false;
        }
    }
}