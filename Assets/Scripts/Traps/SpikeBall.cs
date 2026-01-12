using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _anchor;      
    [SerializeField] private Transform _spikeBall;   
    [Header("Swing Settings")]
    [SerializeField] private float _chainLength = 2f;      
    [SerializeField] private float _swingSpeed = 2f;       
    [SerializeField] private float _maxSwingAngle = 45f;   
    [SerializeField] private float _damping = 1;       


    [Header("Initial Settings")]
    [SerializeField] private float _startAngle = 45f;     
    [SerializeField] private bool _startFromLeft = true;  

    private float _currentAngle; 
    private float _angularVelocity;
    private const float _gravity = 9.81f;

    private void Start()
    {
        _currentAngle = _startAngle * Mathf.Deg2Rad;
        if (!_startFromLeft) _currentAngle = -_currentAngle;

        if (_anchor == null)
        {
            Debug.LogError("Anchor is not assigned");
        }
        if (_spikeBall == null)
        {
            Debug.LogError("SpikeBall is not assigned");
        }
    }

    private void Update()
    {
        SimulateSwing();
        UpdateAnchorRotation();
        UpdateSpikeBallPosition();
    }

    private void SimulateSwing()
    {
        // Công thức con lắc đơn: α = -(g/L) * sin(θ)
        float angularAcceleration = (-_gravity / _chainLength) * Mathf.Sin(_currentAngle);

        // Cập nhật vận tốc góc
        _angularVelocity += angularAcceleration * Time.deltaTime * _swingSpeed;

        // Giảm dần (damping) để bóng không lắc mãi
        _angularVelocity *= _damping;

        // Cập nhật góc hiện tại
        _currentAngle += _angularVelocity * Time.deltaTime;

        // Giới hạn góc lắc (không cho vượt quá max angle)
        float maxAngleRad = _maxSwingAngle * Mathf.Deg2Rad;
        _currentAngle = Mathf.Clamp(_currentAngle, -maxAngleRad, maxAngleRad);
    }


    private void UpdateAnchorRotation()
    {
        if (_anchor == null) return;

        // Chuyển góc từ radian sang độ
        float rotationDegree = _currentAngle * Mathf.Rad2Deg;

        // Xoay anchor theo trục Z
        _anchor.rotation = Quaternion.Euler(0f, 0f, rotationDegree);
    }

    private void UpdateSpikeBallPosition()
    { 
        if (_spikeBall == null || _anchor == null) return;

        // Tính toán vị trí mới của spike ball dựa trên góc
        float x = _chainLength * Mathf.Sin(_currentAngle);
        float y = -_chainLength * Mathf.Cos(_currentAngle);

        // Đặt vị trí spike ball (world position)
        _spikeBall.position = _anchor.position + new Vector3(x, y, 0f);

        // Xoay spike ball theo hướng lắc (optional - để ball trông tự nhiên hơn)
        _spikeBall.rotation = Quaternion.Euler(0f, 0f, _currentAngle * Mathf.Rad2Deg);
    }

    public void AddForce(float force)
    {
        _angularVelocity += force;
    }

    public void ResetSwing()
    {
        _currentAngle = _startAngle * Mathf.Deg2Rad;
        if (!_startFromLeft) _currentAngle = -_currentAngle;
        _angularVelocity = 0f;
    }

    private void OnDrawGizmos()
    {
        if (_anchor == null || _spikeBall == null) return;


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(_anchor.position, _spikeBall.position);

    
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_anchor.position, 0.1f);

        Gizmos.color = Color.cyan;
        DrawSwingArc();
    }

    private void DrawSwingArc()
    {
        if (_anchor == null) return;

        int segments = 30;
        float maxAngleRad = _maxSwingAngle * Mathf.Deg2Rad;

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float t = (float)i / segments;
            float angle = Mathf.Lerp(-maxAngleRad, maxAngleRad, t);

            float x = _chainLength * Mathf.Sin(angle);
            float y = -_chainLength * Mathf.Cos(angle);

            Vector3 point = _anchor.position + new Vector3(x, y, 0f);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, point);
            }
            prevPoint = point;
        }
    }


}
