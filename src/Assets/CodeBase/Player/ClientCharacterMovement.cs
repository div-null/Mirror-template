using Mirror;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public class ClientCharacterMovement : NetworkBehaviour
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Animator _animator;
        
        private void Move(Vector2 inputVector)
        {
            if (inputVector != Vector2.zero)
            {
                Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
                this.transform.forward = moveDirection.normalized;
                _characterController.Move(moveDirection / 20);
            }
        }

        [Command]
        private void MoveCommand(Vector3 moveDirection)
        {
            if (hasAuthority)
            {
                MoveRpc(moveDirection);
            }
        }
    
        [ClientRpc]
        private void MoveRpc(Vector3 moveDirection)
        {
            this.transform.forward = moveDirection.normalized;
            _characterController.Move(moveDirection / 20);
            //_animator.SetTrigger("Moving");
        }
    }
}
