using System;
using System.Collections.Generic;
using _game.Scripts.SpinSystem.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _game.Scripts.SpinSystem
{
    [Serializable]
    public class SpinResultGenerator
    {
        private readonly int _sampleSize = 100;
        private List<Action> _noneMatchedCountList;
        private readonly SpinResultLogHelper _logHelper;

        public SpinResultGenerator()
        {
            _logHelper = new SpinResultLogHelper(true);
            _noneMatchedCountList = new List<Action>();
        }

        public List<SpinResult> GenerateSpinResults(List<SpinData> spinDataList)
        {
            _noneMatchedCountList.Clear();
            OrderByRemainder(spinDataList);
            
            var availableSpinList = GetAvailableSpinList();
            var spinResultList = new List<SpinResult>();
            foreach (var spinData in spinDataList)
            {
                var spinDataResult = GetSpinResult(availableSpinList, spinData);
                foreach (var spinCount in spinDataResult)
                {
                    var spinResult = new SpinResult()
                    {
                        Spin = spinData.Spin,
                        SpinCount = spinCount
                    };

                    InsertSpinResult(spinResultList, spinResult);
                }
            }

            foreach (var action in _noneMatchedCountList)
            {
                action?.Invoke();
            }

            if (availableSpinList.Count != 0)
            {
                Debug.LogException(
                    new Exception($"Distribution Failed Available Spin Count:{availableSpinList.Count}"));
            }

            return spinResultList;
        }

        private void OrderByRemainder(List<SpinData> spinDataList)
        {
            spinDataList.Sort((a, b) => (_sampleSize % a.Percentage).CompareTo(_sampleSize % b.Percentage));
        }


        private void InsertSpinResult(List<SpinResult> spinResultList, SpinResult spinResult)
        {
            if (spinResultList.Count == 0)
            {
                spinResultList.Add(spinResult);
                return;
            }

            for (var i = 0; i < spinResultList.Count; i++)
            {
                var item = spinResultList[i];
                if (spinResult.SpinCount > item.SpinCount) continue;
                spinResultList.Insert(i, spinResult);
                return;
            }

            spinResultList.Add(spinResult);
        }

        private List<int> GetAvailableSpinList()
        {
            var availableSpinList = new List<int>();

            for (var i = 0; i < _sampleSize; i++)
            {
                availableSpinList.Add(i);
            }

            return availableSpinList;
        }

        private List<int> GetSpinResult(List<int> availableSpinList, SpinData spinData)
        {
            var spinResult = new List<int>();
            if (spinData.Percentage == 0) return spinResult;
            var interval = Mathf.CeilToInt((float)_sampleSize / spinData.Percentage);

            var intervalStart = 0;
            var intervalEnd = interval;

            while (intervalEnd < _sampleSize)
            {
                var result = GetRandomSpinResultForInterval(availableSpinList, intervalStart, intervalEnd);
                
                intervalStart = intervalEnd;
                intervalEnd += interval;

                if (result != -1)
                {
                    spinResult.Add(result);
                }

            }

            var lastSpin = GetRandomSpinResultForInterval(availableSpinList, intervalStart, _sampleSize);
            if (lastSpin != -1)
            {
                spinResult.Add(lastSpin);    
            }
            
            if (spinResult.Count != spinData.Percentage)
            {
                _noneMatchedCountList.Add(() =>
                {
                    var diff = spinData.Percentage - spinResult.Count;
                    for (var i = 0; i < diff; i++)
                    {
                        var result = FindFarSpin(availableSpinList, spinResult);
                        _logHelper.LogFarSpinDecision(intervalStart, intervalEnd, spinData, result, availableSpinList, spinResult);
                        spinResult.Add(result);
                    }
                });
            }

            return spinResult;
        }
        
        private int GetRandomSpinResultForInterval(List<int> availableSpinList, int intervalStart, int intervalEnd)
        {
            intervalEnd -= 1;
            var possibleSpins = new List<int>();
            foreach (var spinCount in availableSpinList)
            {
                if (spinCount >= intervalStart && spinCount <= intervalEnd)
                {
                    possibleSpins.Add(spinCount);
                }
            }

            if (possibleSpins.Count == 0)
            {
                return -1;
            }

            var index = Random.Range(0, possibleSpins.Count);
            var result = possibleSpins[index];
            availableSpinList.Remove(result);

            return result;
        }

        private int FindFarSpin(List<int> availableSpinList, List<int> spinResult)
        {
            var selectedSpin = -1;

            if (spinResult.Count == 0)
            {
                var index = Random.Range(0, availableSpinList.Count);
                selectedSpin = availableSpinList[index];
                availableSpinList.Remove(selectedSpin);
                return selectedSpin;
            }

            var maxDistance = int.MinValue;
            foreach (var availableSpin in availableSpinList)
            {
                // Find min difference to existing spins
                var minDifference = int.MaxValue;
                foreach (var spin in spinResult)
                {
                    var difference = Mathf.Abs(availableSpin - spin);
                    if (difference < minDifference)
                    {
                        minDifference = difference;
                    }
                }

                // Select max distance to all of them
                if (minDifference > maxDistance)
                {
                    selectedSpin = availableSpin;
                    maxDistance = minDifference;
                }
            }

            if (selectedSpin == -1)
            {
                Debug.LogException(new Exception("No Spin Found"));
            }
            else
            {
                availableSpinList.Remove(selectedSpin);
            }

            return selectedSpin;
        }
    }
}