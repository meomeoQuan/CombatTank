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

            if (moveInput > 0)
                transform.localScale = new Vector3(-0.3f, 0.3f, 1f);
            else if (moveInput < 0)
                transform.localScale = new Vector3(0.3f, 0.3f, 1f);



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
                Shot(); // gọi animation bắn thay vì nhảy
            }
        }


        #region Tank Animations
        public void Idle()
        {
            Animator.SetBool("Idle", true);
            Animator.SetBool("Move", false);
            Animator.SetBool("Destroy", false);
        }

        public void Move()
        {
            Animator.SetBool("Idle", false);
            Animator.SetBool("Move", true);
            Animator.SetBool("Destroy", false);
        }

        public void Destroy()
        {
            Animator.SetBool("Idle", false);
            Animator.SetBool("Move", false);
            Animator.SetBool("Destroy", true);
        }

        public void Shot()
        {
            Animator.SetTrigger("Shot");
        }
        #endregion
    }
}
