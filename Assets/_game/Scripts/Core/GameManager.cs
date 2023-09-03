using _game.Scripts.Core.Ui;
using _game.Scripts.Ui.Controllers;
using _game.Scripts.Utility;
using Sirenix.OdinInspector;

namespace _game.Scripts.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private void Awake()
        {
            InitializeAwake();
        }

        private void Start()
        {
            InitializeStart();
        }

        private void InitializeAwake()
        {
            UiManager.Instance.Initialize();
        }

        private void InitializeStart()
        {
            StartGame();
        }
        
        [Button]
        private void StartGame()
        {
            UiManager.Get<GameUiController>().Show();
        }
    }
}