
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
