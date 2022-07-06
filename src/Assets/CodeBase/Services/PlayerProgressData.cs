using UnityEngine;

namespace Game.CodeBase.Services
{
    public class PlayerProgressData
    {
        public string Username;

        public void Load()
        {
            string loadedUsername = PlayerPrefs.GetString("Username", "");
            if (loadedUsername == "")
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