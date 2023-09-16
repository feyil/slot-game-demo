using System;
using System.Collections.Generic;
using _game.Scripts.SpinSystem.Data;
using Sirenix.OdinInspector;

namespace _game.Scripts.SpinSystem
{
    [Serializable]
    public class SpinManager
    {
        private readonly SpinResultGenerator _spinResultGenerator;
        private readonly SpinSaveManager _spinSaveManager;

        private readonly List<SpinData> _spinDataList;

        private SpinResultPref _spinResultPref;

        public SpinManager(SpinResultGenerator spinResultGenerator, SpinSaveManager spinSaveManager,
            List<SpinData> spinDatalist)
        {
            _spinResultGenerator = spinResultGenerator;
            _spinSaveManager = spinSaveManager;
            _spinDataList = spinDatalist;
        }

        public void Start()
        {
            var spinResultPref = _spinSaveManager.LoadSpinResult();
            if (spinResultPref == null || spinResultPref.SpinResultList.Count == 0)
            {
                spinResultPref = GetNewResults();
            }

            _spinResultPref = spinResultPref;
        }

        private SpinResultPref GetNewResults()
        {
            var newResults = _spinResultGenerator.GenerateSpinResults(_spinDataList);
            var spinResultPref = _spinSaveManager.SaveSpinResult(newResults);
            return spinResultPref;
        }

        [Button]
        public SpinResult Spin()
        {
            if (_spinResultPref.SpinResultList.Count == 0)
            {
                _spinResultPref = GetNewResults();
            }

            var currentSpinData = _spinResultPref.SpinResultList[0];
            _spinResultPref.SpinResultList.Remove(currentSpinData);
            _spinSaveManager.SaveSpinResult(_spinResultPref);
            return currentSpinData;
        }
    }
}