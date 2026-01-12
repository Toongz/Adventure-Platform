using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickableUI : MonoBehaviour
{
    [SerializeField] private Button _homeBtn;
    [SerializeField] private Button _reloadBtn;
    [SerializeField] private Button _nextBtn;


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        _homeBtn.onClick.AddListener(() => SceneController.Instance.NavigateToMainMenu());
        _reloadBtn.onClick.AddListener(() => SceneController.Instance.ReloadCurrentScene());
        _nextBtn.onClick.AddListener(() => SceneController.Instance.NextLevel());

        CheckAndToggleNextButton();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndToggleNextButton();
    }

    private void CheckAndToggleNextButton()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // Check next scene
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);

            if (nextSceneName.ToLower().StartsWith("level"))
            {
                _nextBtn.gameObject.SetActive(true);
            }
            else
            {
                _nextBtn.gameObject.SetActive(false);
            }
        }
        else
        {
            _nextBtn.gameObject.SetActive(false);
        }
    }
}