using System;
using Game.CodeBase.Player;
using TMPro;
using UnityEngine;

namespace Game.CodeBase.UI
{
    public class LobbyUI : MonoBehaviour
    {
        public Action<string> UsernameChanged;
        public Action<Color> ColorChanged;
        public Action<int> SkinChanged;
        public Action ReadyChanged;

        [SerializeField] private GameObject _startButton;

        [SerializeField] private GameObject _hostMenuButtons;
        [SerializeField] private GameObject _clientMenuButtons;
        [SerializeField] private GameObject _customizationMenu;

        [SerializeField] private TextMeshProUGUI[] UsernameSlots;
        [SerializeField] private TextMeshProUGUI[] ReadySlots;

        [SerializeField] private TMP_InputField usernameInputField;

        public void StartGame()
        {
        }

        public void QuitLobby()
        {
        }

        public void SetHostButtons()
        {
            _hostMenuButtons.SetActive(true);
            _clientMenuButtons.SetActive(false);
        }

        public void PressReady()
        {
            ReadyChanged?.Invoke();
        }

        public void PressSetUsername()
        {
            string newUsername = usernameInputField.text;
            if (newUsername != "")
            {
                UsernameChanged.Invoke(newUsername);
                //Как достучаться до локального игрока?
            }
        }

        public void SetStartGameButtonAvailability(bool isAvailable)
        {
            _startButton.SetActive(isAvailable);
        }

        public void SetupSlot(LobbyPlayer lobbyPlayer, bool isOwner)
        {
            BasePlayer basePlayer = lobbyPlayer.BasePlayer;
            int slotId = basePlayer.Id;

            basePlayer.ColorChanged += (color) => updateColor(slotId, color);
            basePlayer.UsernameChanged += (username) => updateUsername(slotId, username);
            lobbyPlayer.ReadyChanged += (ready) => updateReady(slotId, ready);

            updateColor(slotId, basePlayer.Color);
            updateUsername(slotId, basePlayer.Username);
            updateReady(slotId, lobbyPlayer.IsReady);

            if (isOwner)
                SetHostButtons();
        }

        private void updateColor(int slotId, Color color)
        {
            Debug.Log($"Color updated for {slotId} = {color}");
        }

        private void updateUsername(int slotId, string newName)
        {
            UsernameSlots[slotId].text = newName;
        }

        private void updateReady(int slotId, bool ready)
        {
            ReadySlots[slotId].text = ready ? "Ready" : "Not ready";
        }
    }
}