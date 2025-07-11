﻿namespace AndreaFrigerio.Core.Runtime.Gameplay
{
    using UnityEngine;
    using UnityEngine.InputSystem;
    using Mirror;
    using Sirenix.OdinInspector;

    /// <summary>
    /// Server-authoritative paddle controller.
    /// The owning client reads a vertical axis (−1 … +1) from
    /// <see cref="m_moveAction"/>; that value is interpreted as velocity
    /// (uu/s) multiplied by <see cref="m_moveSpeed"/>.<br/>
    /// When the axis returns to zero the paddle remains at its last position.
    /// </summary>
    [HideMonoScript]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BoxCollider2D))]
    [AddComponentMenu("Andrea Frigerio/Core/Gameplay/Paddle Controller")]
    public sealed class PaddleController : NetworkBehaviour
    {
        #region Inspector

        [BoxGroup("Court")]
        [Tooltip("Half the play-field height in Unity units.")]
        [SerializeField, MinValue(0.05f)]
        private float m_courtHalfHeight = 4.5f;

        [BoxGroup("Movement")]
        [Tooltip("Units per second when axis = ±1.")]
        [SerializeField, MinValue(0.1f)]
        private float m_moveSpeed = 12f;

        [BoxGroup("Input")]
        [Tooltip("InputActionReference that outputs an Axis (float) or Vector2.\n" +
                 "Only the Y component is used.")]
        [SerializeField, Required]
        private InputActionReference m_moveAction;

        #endregion

        #region Private fields

        private float m_cachedAxis; // last analogue value (−1 … +1)

        #endregion

        #region Authority life-cycle

        public override void OnStartAuthority()
        {
            if (this.m_moveAction == null)
            {
                Debug.LogError($"{name}: Move Action Reference is missing.", this);
                return;
            }

            this.m_moveAction.action.Enable();
        }

        public override void OnStopAuthority() =>
            this.m_moveAction?.action.Disable();

        #endregion

        #region Update loop

        private void Awake()
        {
            syncDirection = SyncDirection.ClientToServer;
        }

        private void Update()
        {
            if (!this.authority)
            {
                return;
            }

            this.m_cachedAxis = ReadAxis(); // −1 … +1

            if (Mathf.Abs(this.m_cachedAxis) < 0.001f) // no movement
            {
                return;
            }

            float deltaY = this.m_cachedAxis * this.m_moveSpeed * Time.deltaTime;
            float targetY = Mathf.Clamp(this.transform.position.y + deltaY,
                                        -this.m_courtHalfHeight,
                                         this.m_courtHalfHeight);

            if (this.isServer) // host instance
            {
                ApplyServerY(targetY);
            }
            else // pure client
            {
                CmdRequestY(targetY);
            }
        }

        #endregion

        #region Input helper

        /// <summary>
        /// Reads either a float (Axis-1D) or the Y channel of a Vector2.
        /// </summary>
        private float ReadAxis()
        {
            InputAction action = this.m_moveAction.action;

            // If the action itself resolves to a float (Axis-1D) just read it
            if (action.activeValueType == typeof(float))
            {
                return action.ReadValue<float>();
            }

            // Otherwise it is Vector2 → read Y
            return action.ReadValue<Vector2>().y;
        }

        #endregion

        #region Server authority

        /// <summary>Applies the Y position immediately on the server.</summary>
        [Server]
        private void ApplyServerY(float y)
        {
            Vector3 pos = this.transform.position;
            pos.y = y;
            this.transform.position = pos;
        }

        /// <summary>
        /// Client → server request to move the paddle. The server clamps and applies.
        /// </summary>
        [Command/*(requiresAuthority = false)*/]
        private void CmdRequestY(float y) =>
            ApplyServerY(Mathf.Clamp(y, -this.m_courtHalfHeight, this.m_courtHalfHeight));

        #endregion
    }
}
