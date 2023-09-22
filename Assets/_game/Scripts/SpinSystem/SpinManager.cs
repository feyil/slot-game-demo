using System;
using System.Collections.Generic;
using _game.Scripts.SpinSystem.Data;
using Sirenix.OdinInspector;
using UnityEngine;

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
            LogSpinResult(newResults);

            var spinResultPref = _spinSaveManager.SaveSpinResult(newResults);
            return spinResultPref;
        }

        private void LogSpinResult(List<SpinResult> spinResultList)
        {
            var spinCountDict = new Dictionary<SpinId, List<int>>();
            foreach (var spinResult in spinResultList)
            {
                if (!spinCountDict.ContainsKey(spinResult.Spin))
                {
                    spinCountDict.Add(spinResult.Spin, new List<int>());
                }

                spinCountDict[spinResult.Spin].Add(spinResult.SpinCount);
            }

            var totalCount = 0;
            foreach (var entry in spinCountDict)
            {
                var valueString = "";
                foreach (var value in entry.Value)
                {
                    valueString += value + ";";
                }

                totalCount += entry.Value.Count;
                Debug.Log($"[SPIN_MANAGER] SpinId:{entry.Key}::Count:{entry.Value.Count}::Occurence:{valueString}");
            }

            Debug.Log($"[SPIN_MANAGER] Total count: {totalCount}");
        }

        [Button]
        public SpinResult Spin()
        {
            var currentIndex = _spinSaveManager.GetCurrentIndex();
            if (currentIndex >= _spinResultPref.SpinResultList.Count)
            {
                _spinResultPref = GetNewResults();
                currentIndex = _spinSaveManager.GetCurrentIndex();
            }
            
            var currentSpinData = _spinResultPref.SpinResultList[currentIndex];
            _spinSaveManager.IncreaseIndex();
            return currentSpinData;
        }
    }
}