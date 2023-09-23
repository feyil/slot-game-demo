using System.Collections.Generic;
using _game.Scripts.SpinSystem.Data;
using UnityEngine;

namespace _game.Scripts.SpinSystem
{
    public class SpinResultLogHelper
    {
        private bool _isEnabled;

        public SpinResultLogHelper(bool isEnabled)
        {
            _isEnabled = isEnabled;
        }

        public void LogFarSpinDecision(int intervalStart, int intervalEnd, SpinData spinData, int result,
            List<int> availableSpinList, List<int> spinResult)
        {
            if (!_isEnabled) return;
            
            var str = "";
            foreach (var availableSpin in availableSpinList)
            {
                str += availableSpin + ";";
            }

            var str2 = "";
            foreach (var availableSpin in spinResult)
            {
                str2 += availableSpin + ";";
            }


            Debug.Log(
                $"FindFarSpin ::::{str2}::::{spinData.Spin}:{spinData.Percentage}:{result}xxxx{str}");
        }

        public void LogInterval(int percentage, int intervalStart, int intervalEnd)
        {
            Debug.Log($"Count:{percentage} [{intervalStart},{intervalEnd})");
        }
    }
}