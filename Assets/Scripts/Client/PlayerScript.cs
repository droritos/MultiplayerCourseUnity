using Fusion;
using Game.Server;
using Game.Data;
using UnityEngine;

namespace Game.Client
{
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerScript : NetworkBehaviour
    {
        public PlayerInventory Inventory { get; private set; }

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Animator animator;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private float speed = 5f;

        private bool _isSprinting;
        private NetworkButtons _prevButtons;

        private const float _speedMultiplier = 1.5f;

        public override void Spawned()
        {
            base.Spawned();
            Inventory = new PlayerInventory();

            //Inventory.OnBombUseFailed += ShowNoBombFeedback; // Subscribe to bomb use failure feedback - GUI or sound feedback
        }

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

            var networkAnimator = GetComponentInChildren<NetworkMecanimAnimator>();
            networkAnimator.Animator = animator;
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

            if (_isSprinting)
            {
                deltaMove *= _speedMultiplier;
            }

            if (!Physics.SphereCast(transform.position, capsuleCollider.radius, actualMove.normalized, out RaycastHit hit, deltaMove.magnitude, LayerMask.GetMask("Default")))
            {
                transform.position += deltaMove;

                float speedPercent = actualMove.magnitude;
                if (_isSprinting)
                    speedPercent *= _speedMultiplier;

                animator.SetFloat(AnimatorParams.Speed, speedPercent);
            }

            if (Object.HasStateAuthority)
            {
                if (input.buttons.WasPressed(_prevButtons, PlayerInputButtons.PlaceBombButton))
                {
                    if(!Inventory.TryUseBomb()) return; // If no bombs available, exit early

                    GameManagerRequestBroker.RequestBomb(transform.position); // Player always get a bomb so why "RequestBomb"

                    animator.SetInteger(AnimatorParams.State, (int)PlayerState.PlacingBomb);

                }

                if (input.buttons.WasPressed(_prevButtons, PlayerInputButtons.SprintButton))
                {
                    _isSprinting = true;
                }
                else if (input.buttons.WasReleased(_prevButtons, PlayerInputButtons.SprintButton))
                {
                    _isSprinting = false;
                }
            }

            _prevButtons = input.buttons;
        }
    }
}
