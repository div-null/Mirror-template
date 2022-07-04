using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UsernameEnterUI : MonoBehaviour
{
    private Button SaveButton;
    private TextField UsernameTextField;

    public Action<string> UsernameChanged;
    
    // Start is called before the first frame update
    void Start()
    {
        VisualElement rootVisualElement = this.GetComponent<UIDocument>().rootVisualElement;
        SaveButton = rootVisualElement.Q<Button>("SaveButton");
        UsernameTextField = rootVisualElement.Q<TextField>("UsernameTextField");

        SaveButton.clicked += SaveUsername;
    }

    private void SaveUsername()
    {
        string username = UsernameTextField.text;
        if (username != "")
        {
            PlayerPrefs.SetString("Username", username);
            PlayerPrefs.Save();
            UsernameChanged?.Invoke(username);
            Debug.Log("username is changed");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
