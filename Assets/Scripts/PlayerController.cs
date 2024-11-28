using UnityEngine;
using Luna.Unity;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public GameManager _manager;
    public float moveSpeed  = 10f;

    private float deltaX;
    private float deltaY;
    private bool isPressed;
    
    public GameObject hand;
    public GameObject introText;
    public GameObject targetObj;
    
    private Rigidbody2D rb;
    private SpriteRenderer ren;
    public Sprite[] player;

    public Transform camera;

    [SerializeField] float ClampledValue = 2f;
   
    private float lastPosition;
    private float maxHeight;

    public float offScreen;

    private bool isFalling;

    [SerializeField] Animator anim;

    private void Awake()
    {
        ren = GetComponent<SpriteRenderer>();
        if (_manager.type == GameManager.CharatcerType.Dog)
        {
            ren.sprite = player[0];
        }
        else
        {
            ren.sprite = player[1];
        }
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position.y;
        maxHeight = lastPosition;

        StartCoroutine(GameManager.instance.GameStart());
    }
    
    void Update()
    {
        TouchInput();
        if (transform.position.y <= camera.transform.position.y - offScreen)
        {
            endGame();
            Analytics.LogEvent(Analytics.EventType.LevelFailed);
        }

        float curPos = transform.position.y - lastPosition;

        if(curPos > maxHeight)
        {
            int Distance = Mathf.RoundToInt(curPos);
            maxHeight = curPos;
            GameManager.instance.score.text = Distance.ToString();
        }

        ClampPositions();
    }

    void FixedUpdate()
    {
        if (rb.velocity.y < -0.2)
        {
            isFalling = true;
            anim.Play(_manager.type == GameManager.CharatcerType.Dog ? "DogFall" : "CatFall");
        }
        else
        {
            isFalling = false;
            anim.Play(_manager.type == GameManager.CharatcerType.Dog ? "DogJump" : "CatJump");
        }
    }


    void TouchInput()
    {
        Vector3 inputPosition;

        if (Input.touchCount > 0) // Touch input
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = Camera.main.ScreenToWorldPoint(touch.position);
        }
        else if (Input.GetMouseButtonDown(0)) // Mouse input
        {
            inputPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            return; // Exit if no valid input
        }

        inputPosition.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(inputPosition, Vector2.zero);

        if (hit.collider != null)
        {
            Square square = hit.collider.GetComponent<Square>();

            if (square != null && !square.isEmpty)
            {
                square.RevealTile();
            }
        }
    }


    void ClampPositions()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -ClampledValue, ClampledValue);
        transform.position = clampedPosition;
    }
    
    private void endGame()
    {
        gameObject.SetActive(false);
        GameManager.instance.StartCoroutine("ShowEndCard");
    }
}
