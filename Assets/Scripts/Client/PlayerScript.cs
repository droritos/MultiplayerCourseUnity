using System;
using Fusion;
using UnityEngine;

namespace Game.Client
{
    [RequireComponent(typeof(NetworkMecanimAnimator))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerScript : NetworkBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Animator animator;
        [SerializeField] private float speed = 5f;

        private bool _isSprinting;
        private NetworkButtons _prevButtons;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!animator)
            {
                animator = GetComponentInChildren<Animator>();
            }

            if (!playerInput)
            {
                playerInput = GetComponent<PlayerInput>();
            }
        }
#endif

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out PlayerInputState input))
            {
                return;
            }

            var actualMove = new Vector3(input.move.x, 0f, input.move.y);
            transform.Translate(actualMove * (speed * Runner.DeltaTime));

            if (Object.HasStateAuthority)
            {
                if (input.buttons.WasPressed(_prevButtons, PlayerInputButtons.PlaceBombButton))
                {
                    Debug.Log("Place bomb");
                }

                if (input.buttons.WasPressed(_prevButtons, PlayerInputButtons.SprintButton))
                {
                    Debug.Log("sprint began!");
                }
                else if (input.buttons.WasReleased(_prevButtons, PlayerInputButtons.SprintButton))
                {
                    Debug.Log("sprint ended!");
                }
            }

            _prevButtons = input.buttons;
        }
    }
}
