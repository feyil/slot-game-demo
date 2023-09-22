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
            gameUiController.Show(OnSpin);
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
                    Spin = SpinId.AWildBonus,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.WildWildSeven,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.JackpotJackpotA,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.WildBonusA,
                    Percentage = 13
                },
                new()
                {
                    Spin = SpinId.BonusAJackpot,
                    Percentage = 0
                },
                new()
                {
                    Spin = SpinId.AAA,
                    Percentage = 9
                },
                new()
                {
                    Spin = SpinId.BonusBonusBonus,
                    Percentage = 8
                },
                new()
                {
                    Spin = SpinId.SevenSevenSeven,
                    Percentage = 20
                },
                new()
                {
                    Spin = SpinId.WildWildWild,
                    Percentage = 6
                },
                new()
                {
                    Spin = SpinId.JackpotJackpotJackpot,
                    Percentage = 5
                }
            };
        }
    }
}