using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Luna.Unity;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    public Text endCardTitle;
    public Text endDescription;
    public Text endInstall;
    public Text score;
    public Text Intro;
    public Text retry;
    public Text Target;
    
    public GameObject endCard;
    public GameObject introText;
    public GameObject Hand;
    public GameObject targetObj;
    public GameObject retryBtn;

    private bool _gameEnded = false;
    private bool oneTime = false;
    
    [SerializeField] Image iconIMG;
    
    [Header("Playground End Card fields")]
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Title", 0 , "End Card Details")]
    public string title;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Description", 0 , "End Card Details")]
    public string description;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Install Text", 0 , "End Card Details")]
    public string installText;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Retry Text", 0 , "End Card Details")]
    public string retryText;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("Tutorial Text", 1, "Introduction Tutorial")]
    public string IntroText;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("Tutorial Target Text", 1, "Introduction Tutorial")]
    public string targetText;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("Text Colour", 2, "Change Text Colours")]
    public Color textColours;
    //Make sure to set the colour Alpha to 255 in inspector as its automatically set to 0

    public enum CharatcerType
    {
        Cat,
        Dog
    }
    
    [Header("Choose Character")]
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("Character Cat or Dog", 3, "Character Selection")]
    public CharatcerType type;

    [Header("Icon Sprite")] 
    //Uncomment these when Luna is installed
    [LunaPlaygroundAsset("Icon Image", 4, "Change Icon")]
    public Texture2D iconTex;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("Retry Count", 5, "Number Retries")]
    public int maxCount;

    private static int _curentCount;

    #region Singleton

    private void Awake()
    {
        instance = this;
    }

    #endregion
    
    private void Start()
    {
        //Creates sprite from texture to replace existing sprite
        Sprite icon = Sprite.Create(iconTex, new Rect(0.0f, 0.0f, iconTex.width, iconTex.height), new Vector2(0.5f, 0.5f));
        iconIMG.sprite = icon;
        
        //Sets Texts here
        endCardTitle.text = title;
        endDescription.text = description;
        endInstall.text = installText;
        retry.text = retryText;
        Intro.text = IntroText;
        Target.text = targetText;

        //Sets Colour here
        endCardTitle.color = textColours;
        endDescription.color = textColours;
        endInstall.color = textColours;
        //score.color = textColours;
        Intro.color = textColours;
        retry.color = textColours;
    }

    public IEnumerator GameStart()
    {
        player.SetActive(true);
        introText.SetActive(true);
        Hand.SetActive(true);
        Analytics.LogEvent(Analytics.EventType.LevelStart);
        Analytics.LogEvent(Analytics.EventType.TutorialStarted);
        yield return null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ObjectiveMessage());
        }
       
    }

    IEnumerator ObjectiveMessage()
    {
        if (!oneTime)
        {
            targetObj.SetActive(true);
            yield return new WaitForSecondsRealtime(2f);
            targetObj.SetActive(false);
            oneTime = true;
            Analytics.LogEvent(Analytics.EventType.TutorialComplete);
        }
       
    }

    public IEnumerator ShowEndCard()
    {
        endCard.SetActive(true);
        Hand.SetActive(false);
        introText.SetActive(false);
        targetObj.SetActive(false);
        Analytics.LogEvent(Analytics.EventType.EndCardShown);
        if (_curentCount >= maxCount)
        {
            EndGame();
            retryBtn.SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        player.SetActive(false);
    }

    public void restartGame()
    {
        _curentCount++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Analytics.LogEvent(Analytics.EventType.LevelRetry);
        StartCoroutine(GameStart());
    }

    public void EndGame()
    {
        //Uncomment these when Luna is installed
        Luna.Unity.Playable.InstallFullGame();
        if (!_gameEnded)
        {
            //Uncomment these when Luna is installed
            Luna.Unity.LifeCycle.GameEnded();
            _gameEnded = true;
        }
    }
    
    public void InstallGame()
    {
        //Uncomment these when Luna is installed
        Luna.Unity.Playable.InstallFullGame();
    }
}
