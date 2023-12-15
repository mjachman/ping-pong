using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsController : MonoBehaviour
{
    public UIDocument mainMenu;
    private UIDocument _settings;

    private Button _backButton;
    // Start is called before the first frame update
    void Awake()
    {
        
        _settings = GetComponent<UIDocument>();
        _backButton = _settings.rootVisualElement.Q<Button>("Foo");
         if (_backButton != null)
    {
        Debug.Log("Back button found");
        _backButton.clicked += BackButtonOnClicked;
    }
        _backButton.clicked += BackButtonOnClicked;

    }
    void BackButtonOnClicked()
    {
        //make settings menu invisible
        Debug.Log("BackButtonOnClicked");
        //_settings.enabled = false;
        mainMenu.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
