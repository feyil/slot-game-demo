using System.Collections.Generic;
using _game.Scripts.Core.Ui;
using _game.Scripts.SlotComponent;
using _game.Scripts.SpinSystem;
using _game.Scripts.SpinSystem.Data;
using _game.Scripts.Ui.Controllers;
using _game.Scripts.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _game.Scripts.Core
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField] private List<SpinData> m_spinData;

        [Title("Runtime References")] [SerializeReference]
        private SpinManager _spinManager;

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
            var spinResultGenerator = new SpinResultGenerator();
            var spinSaveManager = new SpinSaveManager();

            var spinData = GetSpinData();

            var spinManager = new SpinManager(spinResultGenerator, spinSaveManager, spinData);
            spinManager.Start();
            _spinManager = spinManager;

            var gameUiController = UiManager.Get<GameUiController>();
            var slotMachine = gameUiController.GetSlotMachine();
            slotMachine.Initialize(OnSpin);
        }

        private void OnSpin(SlotMachineController slotMachineController)
        {
            var spinResult = _spinManager.Spin();
            Debug.Log($"[GAME_MANAGER] SpinResult:{spinResult.SpinCount}:{spinResult.Spin}");
            
            slotMachineController.Spin(spinResult.Spin);
        }

        private List<SpinData> GetSpinData()
        {
            // Data can be provided from different sources (Scriptable Object, Server etc.)
            return m_spinData;
            return new List<SpinData>()
            {
                new()
                {
                    Spin = SpinId.A_WILD_BONUS,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.WILD_WILD_SEVEN,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.JACKPOT_JACKPOT_A,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.WILD_BONUS_A,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.BONUS_A_JACKPOT,
                    Percentage = 0
                },
                new()
                {
                    Spin = SpinId.A_A_A,
                    Percentage = 9
                },
                new()
                {
                    Spin = SpinId.BONUS_BONUS_BONUS,
                    Percentage = 8
                },
                new()
                {
                    Spin = SpinId.SEVEN_SEVEN_SEVEN,
                    Percentage = 20
                },
                new()
                {
                    Spin = SpinId.WILD_WILD_WILD,
                    Percentage = 6
                },
                new()
                {
                    Spin = SpinId.JACKPOT_JACKPOT_JACKPOT,
                    Percentage = 5
                }
            };
        }
    }
}