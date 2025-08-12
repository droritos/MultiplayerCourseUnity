using System;
using Fusion;
using UnityEngine;

namespace Game.Client
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NetworkMecanimAnimator))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerScript : NetworkBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Animator animator;
        [SerializeField] private CapsuleCollider capsuleCollider;
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

            if (!capsuleCollider)
            {
                capsuleCollider = GetComponent<CapsuleCollider>();
            }

            var networkMechanim = GetComponent<NetworkMecanimAnimator>();
            networkMechanim.Animator = animator;
        }
#endif

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out PlayerInputState input))
            {
                return;
            }

            var actualMove = new Vector3(input.move.x, 0f, input.move.y);
            var deltaMove = actualMove * (speed * Runner.DeltaTime);

            if (actualMove != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(actualMove);
            }

            if (!Physics.SphereCast(transform.position, capsuleCollider.radius, actualMove.normalized, out RaycastHit hit, deltaMove.magnitude, LayerMask.GetMask("Default")))
            {
                transform.position += deltaMove;
            }

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
