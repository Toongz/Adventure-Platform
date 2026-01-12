using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance => _instance;

    private static SceneController _instance;

    [SerializeField] private Animator _animator;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else Destroy(gameObject);
    }
    
    public void NextLevel()
    {
        StartCoroutine(LoadLevel());

    }
    public void ReloadCurrentScene()
    {
        StartCoroutine(ReloadScene());
    }

    public void NavigateToMainMenu()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    private IEnumerator LoadLevel()
    {
        _animator.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        _animator.SetTrigger("Start");
    }
    private IEnumerator ReloadScene()
    {
        _animator.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        _animator.SetTrigger("Start");
    }

}
