using UnityEngine;

namespace Assets.Constructors.FuturisticTanks.Scripts
{
    public class Tank : MonoBehaviour
    {
        [Header("Components")]
        public Animator Animator;
        private Rigidbody2D rb;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5.0f;

        private static readonly int IdleHash = Animator.StringToHash("Idle");
        private static readonly int MoveHash = Animator.StringToHash("Move");
        private static readonly int DestroyHash = Animator.StringToHash("Destroy");
        private static readonly int ShotHash = Animator.StringToHash("Shot");

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            HandleMovement();
            HandleShoot();
        }

        private void HandleMovement()
        {
            float moveInput = Input.GetAxis("Horizontal_WASD"); // dùng Horizontal mặc định (A/D, Left/Right)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            // if (moveInput > 0)
            //     transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
            // else if (moveInput < 0)
            //     transform.localScale = new Vector3(0.3f, 0.3f, 1f);
            if (moveInput != 0)
                transform.localScale = new Vector3(-0.3f * Mathf.Sign(moveInput), 0.3f, 1f);


            // Gọi state Idle / Move
            if (Mathf.Abs(moveInput) > 0.1f)
                Move();
            else
                Idle();
        }

        private void HandleShoot()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Shot();
            }
        }


        #region Tank Animations
        public void Idle()
        {
            Animator.SetBool(IdleHash, true);
            Animator.SetBool(MoveHash, false);
            Animator.SetBool(DestroyHash, false);
        }

        public void Move()
        {
            Animator.SetBool(IdleHash, false);
            Animator.SetBool(MoveHash, true);
            Animator.SetBool(DestroyHash, false);
        }

        public void Destroy()
        {
            Animator.SetBool(IdleHash, false);
            Animator.SetBool(MoveHash, false);
            Animator.SetBool(DestroyHash, true);
        }

        public void Shot()
        {
            Animator.SetTrigger(ShotHash);
        }
        #endregion
    }
}
