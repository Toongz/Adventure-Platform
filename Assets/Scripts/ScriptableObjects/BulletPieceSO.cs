using UnityEngine;
[CreateAssetMenu()]
public class BulletPieceSO : ScriptableObject
{
    public GameObject piecePrefab;
    public int pieCount;
    public float scatterForce;
    public float torqueMin;
    public float torqueMax;
    public float pieceLifeTime;
}

