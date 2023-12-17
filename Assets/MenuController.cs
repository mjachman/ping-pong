
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class MenuController1 : MonoBehaviour
{
    // Start is called before the first frame update
    private UIDocument _doc;
    private Button _playButton;
    private Button _settingsButton;
    private Button _quitButton;

    public VisualTreeAsset _settingsMenuTemplate;
    private VisualElement _settingsMenu;

    private VisualElement _mainMenu;

    private RadioButtonGroup _player1Settings;
    private RadioButtonGroup _player2Settings;

    void Awake()
    {
        _doc = GetComponent<UIDocument>();
        _mainMenu = _doc.rootVisualElement.Q<VisualElement>("Screen");

        _playButton = _doc.rootVisualElement.Q<Button>("PlayButton");
        _playButton.clicked += PlayButtonOnClicked;

        _settingsButton = _doc.rootVisualElement.Q<Button>("SettingsButton");
        _settingsButton.clicked += SettingsButtonOnClicked;

        _settingsMenu = _settingsMenuTemplate.CloneTree();
        var BackButton = _settingsMenu.Q<Button>("BackButton");
        BackButton.clicked += BackButtonOnClicked;


        _quitButton = _doc.rootVisualElement.Q<Button>("QuitButton");
        _quitButton.clicked += QuitButtonOnClicked;

        _player1Settings = _settingsMenu.Q<RadioButtonGroup>("Player1Settings");
        _player1Settings.RegisterValueChangedCallback(evt => OnPlayerSettingsChanged(1, evt));
        _player2Settings = _settingsMenu.Q<RadioButtonGroup>("Player2Settings");
        _player2Settings.RegisterValueChangedCallback(evt => OnPlayerSettingsChanged(2, evt));
    }

    void PlayButtonOnClicked()
    {
        SceneManager.LoadScene(1);
    }
    void BackButtonOnClicked()
    {
        //clear the screen
        _doc.rootVisualElement.Clear();
        //add the menu screen
        _doc.rootVisualElement.Add(_mainMenu);
    }
    void SettingsButtonOnClicked()
    {
        //clear the screen
        _doc.rootVisualElement.Clear();
        //add the settings menu
        _doc.rootVisualElement.Add(_settingsMenu);

       _player1Settings.value = PlayerPrefs.GetInt("Player1Controls", 0);
       _player2Settings.value = PlayerPrefs.GetInt("Player2Controls", 0);
    }
    void OnPlayerSettingsChanged(int playerNumber, ChangeEvent<int> evt)
    {
        //if player 1 settings changed
        PlayerPrefs.SetInt($"Player{playerNumber}Controls", evt.newValue);

        //save the player 1 settings
        //if player 2 settings changed
        Debug.Log($"Player {playerNumber} settings changed to {evt.newValue}");
    }

    void QuitButtonOnClicked()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
