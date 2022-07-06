using UnityEngine;

namespace Game.CodeBase.Infrastructure
{
    public class PlayerProgressData
    {
        public string Username;

        public PlayerProgressData()
        {
            string loadedUsername = PlayerPrefs.GetString("Username");
            if (loadedUsername == null)
            {
               SetUsername("Player" + Random.Range(1000, 10000));
               PlayerPrefs.Save();
            }
            else
            {
                Username = loadedUsername;
            }
        }

        public void SetUsername(string username)
        {
            Username = username;
            PlayerPrefs.SetString("Username", Username);
        }
    }
}