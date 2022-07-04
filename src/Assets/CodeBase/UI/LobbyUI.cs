using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public Action<string> UsernameChanged;
    public Action<ColorType> ColorChanged;
    public Action<int> SkinChanged;
    public Action ReadyChanged;

    [SerializeField]
    private GameObject _startButton;
    
    [SerializeField]
    private GameObject _hostMenuButtons;
    [SerializeField]
    private GameObject _clientMenuButtons;
    [SerializeField]
    private GameObject _customizationMenu;

    [SerializeField] private TextMeshProUGUI[] UsernameSlots;
    [SerializeField] private TextMeshProUGUI[] ReadySlots;

    [SerializeField]
    private TMP_InputField usernameInputField;


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

    public void QuitLobby()
    {
        
    }

    public void ChangeUsernameToPlayer(int Id, string newUsername)
    {
        UsernameSlots[Id].text = newUsername;
    }
    
    public void ChangeReadyStatusToPlayer(bool newReadyStatus, int Id)
    {
        if (newReadyStatus)
            ReadySlots[Id].text = "Ready";
        else
            ReadySlots[Id].text = "Not ready";
    }
    
    public void StartGame()
    {
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetStartGameButtonAvailability(bool isAvailable)
    {
        _startButton.SetActive(isAvailable);
    }
}
