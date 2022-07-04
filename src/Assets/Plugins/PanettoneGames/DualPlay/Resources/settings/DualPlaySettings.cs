using UnityEngine;
namespace MultiPlay
{
    public class DualPlaySettings : ScriptableObject
    {
        [HideInInspector]
        public int maxNumberOfClients { get; set; }
        [Tooltip("Enabeling this will increase the project size but will transfer project data like startup scene")]
        public bool copyLibrary;
        [HideInInspector]
        public string clonesPath { get; set; }

        private void OnEnable()
        {
            maxNumberOfClients = 1;
            copyLibrary = true;

            if (string.IsNullOrEmpty(clonesPath))
                clonesPath = Application.persistentDataPath;
        }
    }
}