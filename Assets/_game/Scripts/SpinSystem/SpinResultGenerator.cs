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
                //TODO need of this method can be managed better
                void ConvertSpinResult(SpinData sd, int spinCount)
                {
                    var spinResult = new SpinResult()
                    {
                        Spin = sd.Spin,
                        SpinCount = spinCount
                    };

                    InsertSpinResult(spinResultList, spinResult);
                }

                var spinDataResult = GetSpinResult(availableSpinList, spinData, ConvertSpinResult);
                foreach (var spinCount in spinDataResult)
                {
                    ConvertSpinResult(spinData, spinCount);
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

        private List<int> GetSpinResult(List<int> availableSpinList, SpinData spinData,
            Action<SpinData, int> addSpinResult)
        {
            var spinResult = new List<int>();
            if (spinData.Percentage == 0) return spinResult;
            var interval = Mathf.FloorToInt((float)_sampleSize / spinData.Percentage);

            var intervalStart = 0;
            var intervalEnd = interval;

            var remainder = _sampleSize % spinData.Percentage;
            while (intervalEnd <= _sampleSize)
            {
                if (remainder != 0)
                {
                    intervalEnd += 1;
                    remainder--;
                }

                // _logHelper.LogInterval(spinData.Percentage, intervalStart, intervalEnd);
                var result = GetRandomSpinResultForInterval(availableSpinList, intervalStart, intervalEnd);
                if (result != -1)
                {
                    spinResult.Add(result);
                }

                intervalStart = intervalEnd;
                intervalEnd += interval;
            }

            if (spinResult.Count != spinData.Percentage)
            {
                var diff = spinData.Percentage - spinResult.Count;
                for (var i = 0; i < diff; i++)
                {
                    _noneMatchedCountList.Add(() =>
                    {
                        var inS = intervalStart;
                        var inE = intervalEnd;
                        var result = FindFarSpin(availableSpinList, spinResult);
                        _logHelper.LogFarSpinDecision(inS, inE, spinData, result, availableSpinList,
                            spinResult);

                        if (result != -1) availableSpinList.Remove(result);
                        addSpinResult?.Invoke(spinData, result);
                    });
                }
            }

            return spinResult;
        }

        private int GetRandomSpinResultForInterval(List<int> availableSpinList, int intervalStart, int intervalEnd)
        {
            intervalEnd -= 1;
            var possibleSpins = new List<int>();
            foreach (var spinCount in availableSpinList)
            {
                if (spinCount >= intervalStart && spinCount < intervalEnd)
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

            return selectedSpin;
        }
    }
}