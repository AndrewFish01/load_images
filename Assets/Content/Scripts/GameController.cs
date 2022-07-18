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

        public GameController(MainGamePanel mainPanel)
        {
            _mainPanel = mainPanel;
            _cards = mainPanel.GetCards();
            _imageDownloader = new ImageDownloader();
            SubscribeOnEvents();
            InitGame();
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

        private void OnClickMode(DownloadType type)
        {
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
            _mainPanel.ActivatorModeButtons(false);
            await command.ExecuteAsync();
            _mainPanel.ActivatorModeButtons(true);
        }

        private void OnClickCancel()
        {
            _currentCommand?.Undo();
            _mainPanel.ActivatorModeButtons(true);
        }
    }
}