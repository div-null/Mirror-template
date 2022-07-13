using Game.CodeBase.Player;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.CodeBase.UI
{
    public class LobbyUI : MonoBehaviour
    {
        public ReadOnlyReactiveProperty<string> Username;
        public ReactiveCommand<Color> Color;
        public ReadOnlyReactiveProperty<int> Skin;
        public ReadOnlyReactiveProperty<bool> Ready;

        [Header("Side buttons")] public ReactiveCommand OnStartGame;
        public ReactiveCommand OnSearchServers;
        public ReactiveCommand OnQuit;

        [SerializeField] private Button StartGameButton;
        [SerializeField] private Button ToggleReadyButton;
        [SerializeField] private Button QuitButton;
        [SerializeField] private Button SearchServersButton;


        [SerializeField] private GameObject _hostMenuButtons;
        [SerializeField] private GameObject _clientMenuButtons;
        [SerializeField] private GameObject _customizationMenu;

        [SerializeField] private PlayerSlotUI[] PlayerSlots;

        [Header("Update username")] [SerializeField]
        private Button SetUsername;

        [SerializeField] private TMP_InputField usernameInputField;


        private void Awake()
        {
            Username = SetUsername.OnClickAsObservable()
                .Select((_) => usernameInputField.text)
                .ToReadOnlyReactiveProperty();

            Ready = ToggleReadyButton.OnClickAsObservable()
                .Select(_ => !Ready.Value)
                .ToReadOnlyReactiveProperty();

            OnStartGame = new ReactiveCommand();
            OnQuit = new ReactiveCommand();
            OnSearchServers = new ReactiveCommand();

            Color = new ReactiveCommand<Color>();

            StartGameButton.OnClickAsObservable().Subscribe(_ => OnStartGame.Execute());
            QuitButton.OnClickAsObservable().Subscribe(_ => OnQuit.Execute());
            SearchServersButton.OnClickAsObservable().Subscribe(_ => OnSearchServers.Execute());
        }

        public void SetHostButtons()
        {
            _hostMenuButtons.SetActive(true);
            _clientMenuButtons.SetActive(false);
        }

        public void SetupSlot(LobbyPlayer lobbyPlayer)
        {
            int slotId = lobbyPlayer.Id;
            PlayerSlotUI slot = PlayerSlots[slotId];

            lobbyPlayer.ColorChanged.Subscribe((color) => updateColor(slotId, color));
            lobbyPlayer.UsernameChanged.Subscribe(slot.SetNickname);
            lobbyPlayer.ReadyChanged.Subscribe(slot.SetReady);

            slot.Initialize(lobbyPlayer.Username, lobbyPlayer.IsReady);
            updateColor(slotId, lobbyPlayer.Color);
        }

        private void updateColor(int slotId, Color color)
        {
            Debug.Log($"Color updated for {slotId} = {color}");
        }
    }
}