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

        [Header("Side buttons")] public ReactiveCommand onStartGame;
        public ReactiveCommand onSearchServers;
        public ReactiveCommand onQuit;

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

            onStartGame = new ReactiveCommand();
            onQuit = new ReactiveCommand();
            onSearchServers = new ReactiveCommand();

            Color = new ReactiveCommand<Color>();

            StartGameButton.OnClickAsObservable().Subscribe(_ => onStartGame.Execute());
            QuitButton.OnClickAsObservable().Subscribe(_ => onQuit.Execute());
            SearchServersButton.OnClickAsObservable().Subscribe(_ => onSearchServers.Execute());
        }

        public void SetHostButtons()
        {
            _hostMenuButtons.SetActive(true);
            _clientMenuButtons.SetActive(false);
        }

        public void SetupSlot(LobbyPlayer lobbyPlayer)
        {
            BasePlayer basePlayer = lobbyPlayer.BasePlayer;
            int slotId = basePlayer.Id;
            PlayerSlotUI slot = PlayerSlots[slotId];

            basePlayer.ColorChanged.Subscribe((color) => updateColor(slotId, color));
            basePlayer.UsernameChanged.Subscribe(slot.SetNickname);
            lobbyPlayer.ReadyChanged.Subscribe(slot.SetReady);

            slot.Initialize(basePlayer.Username, lobbyPlayer.IsReady);
            updateColor(slotId, basePlayer.Color);
        }

        private void updateColor(int slotId, Color color)
        {
            Debug.Log($"Color updated for {slotId} = {color}");
        }
    }
}