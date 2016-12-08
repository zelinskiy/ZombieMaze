using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuController : MonoBehaviour {

    [Header("Basic UI Layer")]
    public GameObject BasicInterfaceLayer;
    public Text UserNameText;
    public InputField UserNameInput;
    public Button NewGameButton;
    public Button ShowTopScoresButton;
    public Slider MazeWidthSlider;
    public Slider MazeHeightSlider;
    public Button ExitGameButton;
    public InputField SimplicityInput;

    [Header("Top Scores UI Layer")]
    public GameObject TopScoresLayer;    
    public Button HideTopScoresButton;    
    public Text TopScoresText;

    void Start()
    {
        LoadPlayerPrefs();
        LoadButtonHandlers();
    }

    void LoadButtonHandlers()
    {
        NewGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MainScene");
        });
        ExitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        ShowTopScoresButton.onClick.AddListener(() =>
        {
            LoadTopScores();
            BasicInterfaceLayer.SetActive(false);
            TopScoresLayer.SetActive(true);
        });
        HideTopScoresButton.onClick.AddListener(() =>
        {
            BasicInterfaceLayer.SetActive(true);
            TopScoresLayer.SetActive(false);
        });
        UserNameInput.onEndEdit.AddListener((value) =>
        {
            PlayerPrefs.SetString("UserName", value);
            UserNameText.text = "Hello, " + PlayerPrefs.GetString("UserName");
        });

        MazeHeightSlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt("MazeHeight", Mathf.FloorToInt(value));
        });
        MazeWidthSlider.onValueChanged.AddListener((value) =>
        {
            PlayerPrefs.SetInt("MazeWidth", Mathf.FloorToInt(value));
        });
    }
    

    /// <summary>
    /// Loads values from PlayerPrefs into UI or sets them to default
    /// </summary>
    void LoadPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("UserName"))
        {
            PlayerPrefs.SetString("UserName", "UserName");
        }
        UserNameText.text = "Hello, " + PlayerPrefs.GetString("UserName");

        if (!PlayerPrefs.HasKey("MazeHeight"))
        {
            PlayerPrefs.SetInt("MazeHeight", 5);
        }
        MazeHeightSlider.value = PlayerPrefs.GetInt("MazeHeight");

        if (!PlayerPrefs.HasKey("MazeWidth"))
        {
            PlayerPrefs.SetInt("MazeWidth", 10);            
        }
        MazeWidthSlider.value = PlayerPrefs.GetInt("MazeWidth");

        if (!PlayerPrefs.HasKey("Simplicity"))
        {
            PlayerPrefs.SetInt("Simplicity", 10);
        }
        SimplicityInput.text = PlayerPrefs.GetInt("Simplicity").ToString();
    }

    void LoadTopScores()
    {
        var savedGames = SavesManager.LoadGames().OrderByDescending(g => g.GameBegin).ToList();
        TopScoresText.text = fixedString("UserName  Coins Time    Reason    Date", 53) + "\n" + new string('=', 53) + "\n";
        TopScoresText.text += string.Join("\n", savedGames
            .Select(game =>
                fixedString(game.UserName, 10)
                + fixedString(game.Coins.ToString(), 6)
                + fixedString(game.TimeInMazeSeconds.ToString(), 8)
                + fixedString(game.GameOverReason, 10)
                + game.GameBegin.ToString("dd/MM/yyyy HH:mm:ss")
                + "\n").ToArray());
        TopScoresText.text += new string('=', 53);
        TopScoresText.text += new string('\n', 5);
        
    }


    /// <summary>
    /// Extends or crops string to fixed size
    /// </summary>
    /// <param name="input">Input string</param>
    /// <param name="length">Desirable length</param>
    /// <returns></returns>
    static string fixedString(string input, int length)
    {
        if(input.Length >= length)
        {
            return string.Concat(input.Take(length));
        }
        return input + new string(' ', length - input.Length);
    }
	
}
