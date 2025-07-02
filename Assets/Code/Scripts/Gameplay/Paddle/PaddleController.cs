using Mirror;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AndreaFrigerio.Gameplay.Paddle
{
    [HideMonoScript]
    public class PaddleController : MonoBehaviour
    {
        public float Speed = 5f;
        protected Rigidbody2D rb;

        [SerializeReference]
        public PaddleInput Input;

        protected void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            Input?.Setup(this);
        }

        protected void Update()
        {
            float input = Input.GetInput();
            rb.linearVelocity = new Vector2(0, input * Speed);
        }
    }

    public abstract class PaddleInput
    {
        protected PaddleController controller;

        public virtual void Setup(PaddleController controller)
        {
            this.controller = controller;
        }

        public abstract float GetInput();
    }


    [System.Serializable]
    public class PlayerPaddleInput : PaddleInput
    {
        public InputActionReference selectedAction;

        public override float GetInput()
        {
            float value = 0f;

            if (selectedAction?.action != null)
            {
                if (!selectedAction.action.enabled)
                    selectedAction.action.Enable();

                Vector2 move = selectedAction.action.ReadValue<Vector2>();
                value = move.y;
            }

            return value;
        }
    }

    [System.Serializable]
    public class AIPaddle : PaddleInput
    {
        [SerializeField] private Transform ball;
        [SerializeField] private float trackingSpeed = 1f;

        public override float GetInput()
        {
            if (ball == null || controller == null) return 0f;

            float diff = ball.position.y - controller.transform.position.y;
            return Mathf.Clamp(diff * trackingSpeed, -1f, 1f);
        }
    }

    //[System.Serializable]
    //public class NetworkPaddleInput : PaddleInput
    //{
    //    private float networkInput;

    //    public override void Setup(PaddleController controller)
    //    {
    //        base.Setup(controller);

    //        // Cerca l’oggetto di rete associato alla paddle
    //        if (controller.TryGetComponent<NetworkIdentity>(out var netIdentity) && netIdentity.hasAuthority)
    //        {
    //            // L’owner può inviare input
    //            NetworkInputSender sender = controller.gameObject.AddComponent<NetworkInputSender>();
    //            sender.OnInputReceived += input =>
    //            {
    //                networkInput = input;
    //            };
    //        }
    //    }

    //    public override float GetInput()
    //    {
    //        return networkInput;
    //    }
    //}

    //public class NetworkInputSender : NetworkBehaviour
    //{
    //    public delegate void InputReceived(float value);
    //    public event InputReceived OnInputReceived;

    //    [SerializeField] private string inputAxis = "Vertical";

    //    void Update()
    //    {
    //        if (!hasAuthority) return;

    //        float input = Input.GetAxisRaw(inputAxis);
    //        CmdSendInput(input);
    //    }

    //    [Command]
    //    void CmdSendInput(float value)
    //    {
    //        RpcReceiveInput(value);
    //    }

    //    [ClientRpc]
    //    void RpcReceiveInput(float value)
    //    {
    //        OnInputReceived?.Invoke(value);
    //    }
    //}
}
