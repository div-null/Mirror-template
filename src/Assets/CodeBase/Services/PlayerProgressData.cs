using System;
using Game.CodeBase.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.CodeBase.Services
{
    public class PlayerProgressData
    {
        private const string PlayerProgressPath = "playerProgress";
        public PlayerProgress Progress => _backing.Value;

        private Lazy<PlayerProgress> _backing;

        public PlayerProgressData()
        {
            _backing = new Lazy<PlayerProgress>(LoadOrGenerate);
        }

        private PlayerProgress LoadOrGenerate()
        {
            var progress = LoadPlayerProgress();
            if (progress == null)
            {
                progress = PlayerProgress.Generate();
                SavePlayerProgress(progress);
            }

            return progress;
        }

        public void SetPlayerProgress(PlayerProgress progress)
        {
            SavePlayerProgress(progress);
            _backing = new Lazy<PlayerProgress>(LoadOrGenerate);
        }

        private PlayerProgress LoadPlayerProgress()
        {
            string json = PlayerPrefs.GetString(PlayerProgressPath, "");
            if (json == "") return null;

            try
            {
                return JsonConvert.DeserializeObject<PlayerProgress>(json);
            }
            catch (JsonSerializationException e)
            {
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void SavePlayerProgress(PlayerProgress progress)
        {
            string json = JsonConvert.SerializeObject(progress);
            PlayerPrefs.SetString(PlayerProgressPath, json);
            PlayerPrefs.Save();
        }
    }
}