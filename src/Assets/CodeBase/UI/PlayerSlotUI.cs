using TMPro;
using UnityEngine;

namespace Game.CodeBase.UI
{
    public class PlayerSlotUI : MonoBehaviour
    {
        public TMP_Text NicknameText;
        public TMP_Text ReadyText;

        public void Initialize(string userName, bool readyState)
        {
            SetNickname(userName);
            SetReady(readyState);
        }

        public void SetNickname(string nickname)
        {
            NicknameText.text = nickname;
        }

        public void SetReady(bool value)
        {
            ReadyText.text = value ? "Ready" : "Not Ready";
        }
    }
}