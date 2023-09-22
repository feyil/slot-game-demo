using System;
using System.Collections.Generic;
using _game.Scripts.SpinSystem.Data;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace _game.Scripts.SpinSystem
{
    [Serializable]
    public class SpinSaveManager
    {
        private readonly string SPIN_RESULT_PREF_KEY = "spin_result_pref";
        private readonly string SPIN_INDEX_PREF_KEY = "spin_index_pref";

        [Button]
        public SpinResultPref SaveSpinResult(SpinResultPref spinResultPref)
        {
            var rawData = JsonUtility.ToJson(spinResultPref);
            PlayerPrefs.SetString(SPIN_RESULT_PREF_KEY, rawData);
            return spinResultPref;
        }

        public SpinResultPref SaveSpinResult(List<SpinResult> spinResultList)
        {
            SetCurrentIndex(0);
            var spinResultPref = new SpinResultPref()
            {
                SpinResultList = spinResultList
            };

            return SaveSpinResult(spinResultPref);
        }

        public SpinResultPref LoadSpinResult()
        {
            var rawData = PlayerPrefs.GetString(SPIN_RESULT_PREF_KEY, "");
            if (rawData.IsNullOrWhitespace())
            {
                return null;
            }

            return JsonUtility.FromJson<SpinResultPref>(rawData);
        }

        public void IncreaseIndex()
        {
            var newIndex = GetCurrentIndex() + 1;
            SetCurrentIndex(newIndex);
        }

        private void SetCurrentIndex(int currentIndex)
        {
            PlayerPrefs.SetInt(SPIN_INDEX_PREF_KEY, currentIndex);
        }

        public int GetCurrentIndex()
        {
            //TODO Can be cached for consecutive accesses, not sure about the underlying implementation 
            return PlayerPrefs.GetInt(SPIN_INDEX_PREF_KEY, 0);
        }
    }
}