using UnityEngine;

public enum PlayerState { Idle, Run, Jump, DoubleJump, WallSlide, Fall, Hit, Appear, Desappear}
public class PlayerController : MonoBehaviour
{
    public bool IsDead => _isDead;

    [Header("Components")]
    [SerializeField] private Animator _myAnimator;
    [SerializeField] private ParticleSystem _dustParticle;

    [Header("Movement")]
    [SerializeField] private float _mySpeed;

    [Header("Jump")]
    [SerializeField] private float _myJumpForce;
    private int _maxJumpCount = 2;

    [Header("JumpWall")]
    [SerializeField] private float _wallSlideSpeed;
    [SerializeField] private float _wallJumpForceX;
    [SerializeField] private float _wallJumpForceY;
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallCheckDistance;


    private Rigidbody2D _myRigid2D;
    private PlayerState _myState;
    private bool _onGround;
    private bool _isDead = false;
    [SerializeField] private int _currentJumpCount = 0;
    [SerializeField] private float _coyoteTime = 0.2f;
    [SerializeField] private float _jumpBufferTime = 0.2f;

    [SerializeField] private bool _onWall;
    [SerializeField]private bool _isWallSliding;
    [SerializeField]private int _wallDirection;

    // Coyote and Jumpbuffer
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    // Animator  
    readonly private int _IDLE = Animator.StringToHash("Idle");
    readonly private int _Run = Animator.StringToHash("Run");
    readonly private int _Jump = Animator.StringToHash("Jump");
    readonly private int _DoubleJump = Animator.StringToHash("DoubleJump");
    readonly private int _WallSlide = Animator.StringToHash("WallSlide");
    readonly private int _Hit = Animator.StringToHash("Hit");
    readonly private int _Appear = Animator.StringToHash("Appear");
    readonly private int _Desappear = Animator.StringToHash("Desappear");
    readonly private int _Fall = Animator.StringToHash("Fall");

    private void Awake()
    {
        _myRigid2D = GetComponent<Rigidbody2D>();         
    }
    private void Update()
    {
        if(_isDead) return;

        CheckWall();
        CheckWallSlide();
        OnPlayerMove();

        HandleCoyoteTime();
        HandleJumpBuffer();
        OnPlayerJump();

    }
    private void OnDust()
    {
        _dustParticle.Play();
    }
    private void OnChangeState(PlayerState newState)
    {
        if (_myState == newState) return;

        _myState = newState;
        
        switch (_myState)
        {
            case PlayerState.Idle:
                _myAnimator.Play(_IDLE);
                //Debug.Log("Idle");
                break;

            case PlayerState.Run:
               _myAnimator.Play(_Run);
                OnDust(); 
                //Debug.Log("Run");
                break;

            case PlayerState.Jump:
                _myAnimator.Play(_Jump);
                OnDust();
                //Debug.Log("Jump");
                break;

            case PlayerState.WallSlide:
                _myAnimator.Play(_WallSlide);
                //Debug.Log("Wall Slide");
                break;

            case PlayerState.Fall:
                _myAnimator.Play(_Fall);
                //Debug.Log("Fall");
                break;

            case PlayerState.DoubleJump:
                _myAnimator.Play(_DoubleJump);
                OnDust();
                //Debug.Log("Double jump");
                break;

            case PlayerState.Appear:
                _myAnimator.Play(_Appear);
                //Debug.Log("Appear");
                break;

            case PlayerState.Desappear:
                _myAnimator.Play(_Desappear);
                //Debug.Log("Desappear");
                break;

            case PlayerState.Hit:
                _myAnimator.Play(_Hit);
                //Debug.Log("Hit");
                break;
        }
    }

    private void OnPlayerMove()
    {
        if (_isWallSliding) return;

        float moveX = Input.GetAxis("Horizontal");
        _myRigid2D.linearVelocity = new Vector2(moveX * _mySpeed, _myRigid2D.linearVelocity.y);

        
        // Player moving
        if(moveX != 0)
        {

            // CHỈ TẠO BỤI KHI BẮT ĐẦU CHẠY hoặc KHI Ở TRÊN GROUND
            if (_onGround && _myState != PlayerState.Run)
            {
                OnDust();
            }

            // Player running
            if (_onGround) this.OnChangeState(PlayerState.Run);
            else
            {
                // Player fall
                if (!_isWallSliding)
                {
                    if (_myRigid2D.linearVelocityY < -0.1f) this.OnChangeState(PlayerState.Fall);
                }
               
                //// Player can jump
                //else
                //{

                //}
            }
            // Flip Player
            float euler = moveX > 0 ? 0f : 180f;
            transform.eulerAngles = Vector3.up * euler;
        }
        // Player standing
        else
        {
            if(_onGround) this.OnChangeState(PlayerState.Idle);
            else
            {
                // Player fall
                if(!_isWallSliding)
                {
                    if (_myRigid2D.linearVelocityY < -0.1f) this.OnChangeState(PlayerState.Fall);
                }
                
                //// Player can jump
                //else
                //{

                //}
            }
        }
    }

    //private void OnPlayerJump()
    //{
    //    // Reset jump count when on ground
    //    if (_onGround && _currentJumpCount > 0 && _myRigid2D.linearVelocityY <= 0.1f)
    //    {
    //        _currentJumpCount = 0;
    //        Debug.Log("Reset jump count");
    //    }
    //    // Check input jump
    //    if (Input.GetButtonDown("Jump"))
    //    {
    //        if (_isWallSliding)
    //        {
    //            // Jump out the wall
    //            float jumpDirectX = -_wallDirection;
    //            _myRigid2D.linearVelocity = new Vector2(jumpDirectX * _wallJumpForceX, _wallJumpForceY);

    //            // Flip player
    //            float euler = jumpDirectX > 0 ? 0f : 180f;
    //            transform.eulerAngles = Vector3.up * euler;

    //            _currentJumpCount = 1;
    //            _isWallSliding = false;
    //            this.OnChangeState(PlayerState.Jump);

    //        }

    //        // Can jump
    //        if(_currentJumpCount < _maxJumpCount )
    //        {
    //            // Apply jump force
    //            //_myRigid2D.AddForce(Vector2.up * _myJumpForce, ForceMode2D.Impulse);
    //            _myRigid2D.linearVelocity = new Vector2(_myRigid2D.linearVelocityX, _myJumpForce);
    //            _currentJumpCount++;

    //            if (_currentJumpCount == 1) this.OnChangeState(PlayerState.Jump);
    //            else if (_currentJumpCount == 2) this.OnChangeState(PlayerState.DoubleJump);
    //        }
    //    }

    //}
    private void OnPlayerJump()
    {
        if (_onGround && _currentJumpCount > 0 && _myRigid2D.linearVelocityY <= 0.1f)
        {
            _currentJumpCount = 0;
        }

        if (_jumpBufferCounter > 0f)
        {
          
            if (_isWallSliding)
            {
                float jumpDirectX = -_wallDirection;
                _myRigid2D.linearVelocity = new Vector2(jumpDirectX * _wallJumpForceX, _wallJumpForceY);

                // Flip player
                float euler = jumpDirectX > 0 ? 0f : 180f;
                transform.eulerAngles = Vector3.up * euler;

                _currentJumpCount = 1;
                _isWallSliding = false;
           
                this.OnChangeState(PlayerState.Jump);

             
                _jumpBufferCounter = 0f;
                _coyoteTimeCounter = 0f;
                return;
            }

            // Normal / coyote / double jump:
            // Allow jump if within coyote time OR has remaining jump counts
            if (_coyoteTimeCounter > 0f || _currentJumpCount < _maxJumpCount)
            {
                // Apply jump force
                _myRigid2D.linearVelocity = new Vector2(_myRigid2D.linearVelocityX, _myJumpForce);
                _currentJumpCount++;

                if (_currentJumpCount == 1)
                {
                    this.OnChangeState(PlayerState.Jump);
                }

                else if (_currentJumpCount == 2)
                {
                    this.OnChangeState(PlayerState.DoubleJump);
                }

                _jumpBufferCounter = 0f;
                _coyoteTimeCounter = 0f;
            }
        }
    }
    private void CheckWall()
    {
        _wallDirection = transform.eulerAngles.y == 0 ? 1 : -1;

        Vector2 direction = _wallDirection == 1 ? Vector2.right : Vector2.left;
        _onWall = Physics2D.Raycast(transform.position, direction, _wallCheckDistance, _wallLayer);
    }
    private void CheckWallSlide()
    {
        if(_onWall && !_onGround && _myRigid2D.linearVelocityY < 0)
        {
            _isWallSliding = true;
            _myRigid2D.linearVelocity = new Vector2(_myRigid2D.linearVelocity.x, -_wallSlideSpeed);
            this.OnChangeState(PlayerState.WallSlide);

            _currentJumpCount = 0;
        }
        else
        {
            _isWallSliding = false;
        }
    }

   

    private void HandleCoyoteTime()
    {
        if (_onGround)
        {
            _coyoteTimeCounter = _coyoteTime;
        }
        else
        {
            _coyoteTimeCounter -= Time.deltaTime;
            if (_coyoteTimeCounter < 0f) _coyoteTimeCounter = 0f;
        }
    }

    private void HandleJumpBuffer()
    {
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
            if (_jumpBufferCounter < 0f) _jumpBufferCounter = 0f;
        }
    }
  


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            //if (collision.transform.position.y < transform.position.y)
            //{
                _onGround = true;
            //}
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
         if(collision.tag == "Ground")
         {
            _onGround = false;
         } 
    }
 
    public void OnHit()
    {
        Debug.Log("Hit");
        _myRigid2D.linearVelocity = Vector2.zero;
        _myRigid2D.AddForce(Vector2.up * _myJumpForce * 0.75f, ForceMode2D.Impulse);
    }
    public void OnPlayerHit()
    {
        Debug.Log("Dead..");
        if (!_isDead)
        {
            _isDead = true;

            Camera.main.GetComponent<CameraShake2D>().Shake(0.3f, 0.35f);
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;

            if (_myRigid2D != null)
            {
                _myRigid2D.linearVelocity = Vector2.zero;
                _myRigid2D.gravityScale = 4f;
                _myRigid2D.AddForce(new Vector2(Random.Range(-10f, 10f), 10f), ForceMode2D.Impulse);
                _myRigid2D.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
            }

            this.OnChangeState(PlayerState.Hit);
            SoundManager.Instance.OnPlaySound(SoundType.Hit);
        }
    }
    
    

}
