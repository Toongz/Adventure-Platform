using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButon;
    [SerializeField] private Button _quitButon;


    private void Awake()
    {
        _playButon.onClick.AddListener(SceneController.Instance.NextLevel);

        _quitButon.onClick.AddListener(Application.Quit);
    }

    
}
