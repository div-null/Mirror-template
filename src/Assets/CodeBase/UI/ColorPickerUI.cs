using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerUI : MonoBehaviour
{
    private Dictionary<ColorType, Button> _colorPickerButtons;

    // Start is called before the first frame update
    void Start()
    {
        _colorPickerButtons = new Dictionary<ColorType, Button>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
