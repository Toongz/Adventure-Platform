using UnityEngine;
using UnityEngine.UI;

public class CongratulationUIManager : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _exitButton;





    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(SceneController.Instance.NavigateToMainMenu);
        
        _exitButton.onClick.AddListener(Application.Quit);
    }
}
