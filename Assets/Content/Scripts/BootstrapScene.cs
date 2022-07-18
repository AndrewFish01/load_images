using Content.Scripts.UI;
using UnityEngine;

namespace Content.Scripts
{
    public class BootstrapScene : MonoBehaviour
    {
        [SerializeField] private MainGamePanel _mainGamePanel;

        private void Awake()
        {
            var mainGamePanel = Instantiate(_mainGamePanel);
            new GameController(mainGamePanel);
        }
    }
}