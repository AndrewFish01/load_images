using System;
using System.Collections.Generic;
using Content.Scripts.Commands;
using Content.Scripts.Enums;
using Content.Scripts.Network;
using Content.Scripts.UI;
using Cysharp.Threading.Tasks;

namespace Content.Scripts
{
    public class GameController
    {
        private MainGamePanel _mainPanel;
        private ImageDownloader _imageDownloader;
        private ICommand _currentCommand;
        private List<Card> _cards;
        private bool _isRun;

        public GameController(MainGamePanel mainPanel)
        {
            _mainPanel = mainPanel;
            _cards = mainPanel.GetCards();
            _imageDownloader = new ImageDownloader();
            _isRun = false;
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
            _mainPanel.ClickMode += OnClickMode;
        }

        private void UnsubscribeOnEvents()
        {
            _mainPanel.ClickCancel -= OnClickCancel;
            _mainPanel.ClickMode -= OnClickMode;
        }
        
        private void OnClickMode(DownloadType type)
        {
            if(_isRun) return;
            _isRun = true;
            _currentCommand = type switch
            {
                DownloadType.AllAtOnce => new AllAtOnceCommand(_cards, _imageDownloader),
                DownloadType.OneByOne => new OneByOneCommand(_cards, _imageDownloader),
                DownloadType.WhenReady => new WhenReadyCommand(_cards, _imageDownloader),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Not find download type")
            };
            
            ExecuteCommand(_currentCommand).Forget();
        }

        private async UniTaskVoid ExecuteCommand(ICommand command)
        {
            _mainPanel.ButtonCancel.interactable = true;
            await command.ExecuteAsync();
            _isRun = false;
            _mainPanel.ButtonCancel.interactable = false;
        }

        private void OnClickCancel()
        {
            _currentCommand?.Undo();
            _isRun = false;
            _mainPanel.ButtonCancel.interactable = false;
        }
    }
}