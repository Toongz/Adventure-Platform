using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPoint : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    readonly private int _Idle = Animator.StringToHash("Idle");
    readonly private int _End = Animator.StringToHash("End");

    private bool _isLevelComplete = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag == "Player")
        {
            Debug.Log("Next level");
            _animator.Play(_End);
            if (!_isLevelComplete)
            {
                _isLevelComplete = true;
                StartCoroutine(DelayNextLevel());
            }
           
        }
    }

    private IEnumerator DelayNextLevel()
    {
        yield return new WaitForSeconds(1f);
        SceneController.Instance.NextLevel();
    }

}
