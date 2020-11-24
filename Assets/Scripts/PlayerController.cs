using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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
    private float myCalc;

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
        myCalc = Math.Abs(20f * lastPosition);
    
        rb = GetComponent<Rigidbody2D>();
        lastPosition = transform.position.y;
        maxHeight = lastPosition;
        
        GameManager.instance.StartCoroutine("GameStart");
    }
    
    void Update()
    {
        TouchInput();
        if (transform.position.y <= camera.transform.position.y - offScreen)
        {
            endGame();
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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if (Input.GetMouseButtonDown(0))
        {
            hand.SetActive(false);
            introText.SetActive(false);
            
            deltaX = mousePos.x - transform.position.x;
            deltaY = mousePos.y - transform.position.y;
            
            isPressed = true;
        }

        if (Input.GetMouseButton(0) && isPressed)
        {
            rb.transform.position = new Vector2((mousePos.x - deltaX), transform.position.y);
           
            if (transform.position.x > 0.1)
            {
                ren.flipX = false;
            }
            else if (transform.position.x < -0.1)
            {
                ren.flipX = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isPressed = false;
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
