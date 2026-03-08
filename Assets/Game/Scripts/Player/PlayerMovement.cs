using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TarodevController
{
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// I have a premium version on Patreon, which has every feature you'd expect from a polished controller. Link: https://www.patreon.com/tarodev
    /// You can play and compete for best times here: https://tarodev.itch.io/extended-ultimate-2d-controller
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/tarodev
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerMovement : MonoBehaviour, IPlayerController
    {
        [SerializeField] private ScriptableStats _stats;
        private Rigidbody2D _rb;
        private CapsuleCollider2D _col;
        private FrameInput _frameInput;
        private float oppositeValue = 1;
        [NonSerialized] public Vector2 frameVelocity;
        private bool _cachedQueryStartInColliders;

        private PlayerStateMachine _sm;

        public bool IsDebug = false;
        [SerializeField] bool GodMode = false;
        [SerializeField] bool IsFacingLeft = false;

        [NonSerialized] public Animator animator;

        #region Interface

        public Vector2 FrameInput => _frameInput.Move;
        public event Action<bool, float> GroundedChanged;
        public event Action Jumped;

        #endregion

        private float _time;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _col = GetComponent<CapsuleCollider2D>();
            _sm = new PlayerStateMachine();
            if(GetComponent<Animator>() != null)animator = GetComponent<Animator>();
            _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        }

        private void Start()
        {
            _sm.ChangeState(new IdleState(_stats, _sm, this));
            _intialGravityScale = _rb.gravityScale;
        }

        private void Update()
        {
            _time += Time.deltaTime;
            GatherInput();
            //_sm.Tick(Time.deltaTime);
        }

        private void GatherInput()
        {
            _frameInput = new FrameInput
            {
                JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C),
                JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C),
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
            };

            if (_stats.SnapInput)
            {
                _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.x);
                _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold ? 0 : Mathf.Sign(_frameInput.Move.y);
            }

            if (_frameInput.JumpDown)
            {
                jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                GodMode = !GodMode;
            }
        }

        private void FixedUpdate()
        {
            CheckCollisions();

            HandleJump();
            HandleDirection();
            FlipSprite();
            HandleAnimation();
            HandleGravity();

            ApplyMovement();
        }

        #region Collisions

        private float _frameLeftGrounded = float.MinValue;
        private float _intialGravityScale;
        [NonSerialized] public bool grounded;

        //private void OnCollisionEnter2D(Collision2D collision)
        //{
        //    if (!collision.collider.CompareTag("Platform"))
        //    {
        //        // Hit a Ceiling
        //        bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);
        //        if (ceilingHit) frameVelocity.y = Mathf.Min(0, frameVelocity.y);
        //    }
        //}
        LayerMask cellingmask;
        LayerMask groundmask;
        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = false;

            if (_stats.PlayerLayer == LayerMask.GetMask("Player1"))
            {
                cellingmask = LayerMask.GetMask("Ground", "Ground1");
                groundmask = LayerMask.GetMask("Ground", "Ground1", "Platform", "Platform1");
            } else  {
                cellingmask = LayerMask.GetMask("Ground", "Ground2");
                groundmask = LayerMask.GetMask("Ground", "Ground2", "Platform", "Platform2");
            }


            // Ground and Ceiling
            bool groundHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _stats.GrounderDistance, groundmask/*~_stats.PlayerLayer*/);
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, cellingmask);

            if (ceilingHit)
            {
                //Debug.Log("Ceiling Hit Detected");
                frameVelocity.y = Mathf.Min(0, frameVelocity.y);
            }

            // Landed on the Ground
            if (!grounded && groundHit)
            {
                grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
                GroundedChanged?.Invoke(true, Mathf.Abs(frameVelocity.y));
            }
            // Left the Ground
            else if (grounded && !groundHit)
            {
                grounded = false;
                _frameLeftGrounded = _time;
                GroundedChanged?.Invoke(false, 0);
            }

            if (grounded)
            {
                _rb.gravityScale = 0;
            }
            else
            {
                _rb.gravityScale = _intialGravityScale;
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
        }


        
        #endregion


        #region Jumping

        [NonSerialized] public bool jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private float _timeJumpWasPressed;

        private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
        private bool CanUseCoyote => _coyoteUsable && !grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

        private void HandleJump()
        {
            if (!_endedJumpEarly && !grounded && !_frameInput.JumpHeld && _rb.linearVelocity.y > 0) _endedJumpEarly = true;

            if (!jumpToConsume && !HasBufferedJump) return;

            if (grounded || CanUseCoyote) ExecuteJump();

            jumpToConsume = false;
        }

        private void ExecuteJump()
        {
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            frameVelocity.y = _stats.JumpPower;
            Jumped?.Invoke();
        }

        #endregion

        #region Horizontal

        private void HandleDirection()
        {
            if (_frameInput.Move.x == 0)
            {
                var deceleration = grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
            }
            else
            {
                frameVelocity.x = Mathf.MoveTowards(frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed * oppositeValue, _stats.Acceleration * Time.fixedDeltaTime);
            }
        }

        private void FlipSprite()
        {
            if (_frameInput.Move.x > 0)
                GetComponent<SpriteRenderer>().flipX = IsFacingLeft;
            else if (_frameInput.Move.x < 0)
                GetComponent<SpriteRenderer>().flipX = !IsFacingLeft;
        }

        #endregion

        #region Gravity

        private void HandleGravity()
        {
            if (grounded && frameVelocity.y <= 0f)
            {
                frameVelocity.y = _stats.GroundingForce;
            }
            else
            {
                var inAirGravity = _stats.FallAcceleration;
                if (_endedJumpEarly && frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
                frameVelocity.y = Mathf.MoveTowards(frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #endregion

        private void HandleAnimation()
        {
            if(animator == null) return;
            animator.SetFloat("xVelocity", Math.Abs(frameVelocity.x));
            animator.SetFloat("yVelocity", frameVelocity.y);
            animator.SetBool("isJumping", !grounded);

        }

        private void ApplyMovement() => _rb.linearVelocity = frameVelocity;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
        }
#endif

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Spike"))
            {
                Debug.Log(name + " hit a spike!");
                if (GodMode) return;
                if (animator != null) animator.SetTrigger("Death");
                if (_rb.simulated) _rb.simulated = false;
                StartCoroutine(RestartScenesAfterDelay(1.25f));
            }

            if (collision.CompareTag("Potion"))
            {
                Debug.Log(name + " hit a potion!");
                oppositeValue *= -1;
                IsFacingLeft = !IsFacingLeft;
                Destroy(collision.gameObject);
            }
        }

        #region Toggle Player
        public void ToggleEnble(bool value)
        {
            if (value)
                EnablePlayer();
            else
                DisablePlayer();
        }

        private void DisablePlayer()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.enabled = false;

            if (_rb.simulated) _rb.simulated = false;
        }

        private void EnablePlayer()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.enabled = true;

            if (!_rb.simulated) _rb.simulated = true;
        }
        #endregion

        public IEnumerator RestartScenesAfterDelay(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Restarting Scene...");
        }
    }
}