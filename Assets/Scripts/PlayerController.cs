using UnityEngine;

public enum PlayerState { Idle, Run, Jump, DoubleJump, WallSlide, Fall, Hit, Appear, Desappear}
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator _myAnimator;

    [Header("Movement")]
    [SerializeField] private float _mySpeed;

    [Header("Jump")]
    [SerializeField] private float _myJumpForce;
    private int _maxJumpCount = 2;


    private Rigidbody2D _myRigid2D;
    private PlayerState _myState;
    private bool _onGround;
    private int _currentJumpCount = 0;

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
        OnPlayerMove();
        OnPlayerJump();
    }

    private void OnChangeState(PlayerState newState)
    {
        if (_myState == newState) return;

        _myState = newState;
        
        switch (_myState)
        {
            case PlayerState.Idle:
                _myAnimator.Play(_IDLE);
                Debug.Log("Idle");
                break;

            case PlayerState.Run:
               _myAnimator.Play(_Run);
                Debug.Log("Run");
                break;

            case PlayerState.Jump:
                _myAnimator.Play(_Jump);
                Debug.Log("Jump");
                break;

            case PlayerState.WallSlide:
                break;

            case PlayerState.Fall:
                _myAnimator.Play(_Fall);
                Debug.Log("Fall");
                break;

            case PlayerState.DoubleJump:
                _myAnimator.Play(_DoubleJump);
                Debug.Log("Double jump");
                break;

            case PlayerState.Appear:
                _myAnimator.Play(_Appear);
                Debug.Log("Appear");
                break;

            case PlayerState.Desappear:
                _myAnimator.Play(_Desappear);
                Debug.Log("Desappear");
                break;

            case PlayerState.Hit:
                _myAnimator.Play(_Hit);
                Debug.Log("Hit");
                break;
        }
    }

    private void OnPlayerMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        _myRigid2D.linearVelocity = new Vector2(moveX * _mySpeed, _myRigid2D.linearVelocity.y);

        // Player moving
        if(moveX != 0)
        {
            // Player running
            if(_onGround) this.OnChangeState(PlayerState.Run);
            else
            {
                // Player fall
                if (_myRigid2D.linearVelocityY < -0.1f) this.OnChangeState(PlayerState.Fall);
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
                if (_myRigid2D.linearVelocityY < -0.1f) this.OnChangeState(PlayerState.Fall);
                //// Player can jump
                //else
                //{

                //}
            }
        }
    }

    private void OnPlayerJump()
    {
        // Reset jump count when on ground
        if (_onGround && _currentJumpCount > 0 && _myRigid2D.linearVelocityY <= 0.1f)
        {
            _currentJumpCount = 0;
            Debug.Log("Reset jump count");
        }
        // Check input jump
        if (Input.GetButtonDown("Jump"))
        {
            // Can jump
            if(_currentJumpCount < _maxJumpCount )
            {
                // Apply jump force
                _myRigid2D.AddForce(Vector2.up * _myJumpForce, ForceMode2D.Impulse);
                _currentJumpCount++;

                if (_currentJumpCount == 1) this.OnChangeState(PlayerState.Jump);
                else if (_currentJumpCount == 2) this.OnChangeState(PlayerState.DoubleJump);
                
            }
        }
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            if(collision.transform.position.y < this.transform.position.y) 
                _onGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
         if(collision.tag == "Ground")
         {
            _onGround = false;
         } 
    }

}
