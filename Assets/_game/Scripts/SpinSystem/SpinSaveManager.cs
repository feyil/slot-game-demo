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

        [Button]
        public SpinResultPref SaveSpinResult(SpinResultPref spinResultPref)
        {
            var rawData = JsonUtility.ToJson(spinResultPref);
            PlayerPrefs.SetString(SPIN_RESULT_PREF_KEY, rawData);
            return spinResultPref;
        }

        public SpinResultPref SaveSpinResult(List<SpinResult> spinResultList)
        {
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
    }
}