using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class MainMenuController : MonoBehaviour {
    

    public Button ExitGameButton;
    public Button NewGameButton;

    public Button ShowTopScoresButton;
    public Button HideTopScoresButton;

    public InputField UserNameInput;

    public Text UserNameText;

    public Slider MazeWidthSlider;
    public Slider MazeHeightSlider;

    public GameObject BasicInterfaceLayer;
    public GameObject TopScoresLayer;
    public Text TopScoresText;

    void Start()
    {

        
        if (PlayerPrefs.HasKey("UserName"))
        {
            UserNameText.text = "Hello, " + PlayerPrefs.GetString("UserName");
        }
        if (PlayerPrefs.HasKey("MazeHeight"))
        {
            MazeHeightSlider.value = PlayerPrefs.GetInt("MazeHeight");
        }
        if (PlayerPrefs.HasKey("MazeWidth"))
        {
            MazeHeightSlider.value = PlayerPrefs.GetInt("MazeWidth");
        }
        
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

    void LoadTopScores()
    {
        var savedGames = SavesManager.LoadGames().OrderByDescending(g => g.GameBegin).ToList();
        TopScoresText.text = "UserName  Coins Time    Reason    Date\n" + new string('=', 53) + "\n";
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

    string fixedString(string input, int length)
    {
        if(input.Length > length)
        {
            throw new System.ArgumentException("Input is too long");
        }
        return input + new string(' ', length - input.Length);
    }
	
}
