using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.CodeBase.UI
{
    public class ColorPickerUI : MonoBehaviour
    {
        private Dictionary<ColorType, Button> _colorPickerButtons;

        // Start is called before the first frame update
        void Start()
        {
            _colorPickerButtons = new Dictionary<ColorType, Button>();
        
        }
    }
}
