using Luna.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public GameObject tilePrefab; // Tile prefab reference

    private bool isCheckingMatch = false;
    private bool _gameEnded = false;
    private bool oneTime = false;
    public Transform gridParent; // Parent for grid tiles
    public Sprite[] tileSprites; // Array of tile icons
    public GameObject lineRendererPrefab;
    private LineRenderer lineRenderer;

    [SerializeField] private Square firstSelectedTile;
    [SerializeField] private Square secondSelectedTile;

    [SerializeField] private Square[,] grid;

    [SerializeField] Image iconIMG;
    [LunaPlaygroundField("Grid Width", 0, "Grid")]
    public int rows = 4; // Number of rows
    [LunaPlaygroundField("Grid Height", 0, "Grid")]
    public int columns = 4; // Number of columns
    [LunaPlaygroundField("Total pair", 0, "Grid")]
    public int totalPair = 10; // Number of rows
    [Header("Playground End Card fields")]
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Title", 0, "End Card Details")]
    public string title;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Description", 0, "End Card Details")]
    public string description;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Install Text", 0, "End Card Details")]
    public string installText;
    //Uncomment these when Luna is installed
    [LunaPlaygroundField("End Card Retry Text", 0, "End Card Details")]
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

        lineRenderer = lineRendererPrefab.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
    }

    public IEnumerator GameStart()
    {
        //player.SetActive(true);
        // introText.SetActive(true);
        // Hand.SetActive(true);
        GenerateTiles(rows, columns);
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

    public void GenerateTiles(int rows, int columns)
    {
        List<int> tileIds = new List<int>();
        grid = new Square[rows + 2, columns + 2];  // Increase the size to include borders

        // Generate pairs of tile IDs
        for (int i = 0; i < (rows * columns) / 2; i++)
        {
            tileIds.Add(i);
            tileIds.Add(i);
        }

        // Shuffle the tile IDs
        tileIds = ShuffleList(tileIds);

        int index = 0;

        // Loop through rows and columns to create tiles
        for (int row = 0; row <= rows + 1; row++)  // Loop through all rows (including borders)
        {
            for (int col = 0; col <= columns + 1; col++)  // Loop through all columns (including borders)
            {
                GameObject tileObj = Instantiate(tilePrefab, gridParent);
                var pos = new Vector3(col, -row, 0) + new Vector3(-2, 2);

                if (row == 0 || row == rows + 1 || col == 0 || col == columns + 1)
                {
                    CreateBorderTile(row, col,tileObj,pos);
                }
                else
                {
                    Square tile = tileObj.GetComponent<Square>();
                    tile.tileId = tileIds[index];
                    tile.icon.sprite = tileSprites[tileIds[index]];
                    tileObj.transform.position = pos;
                    grid[row, col] = tile; 
                    index++;

                }

            }
        }
    }
    private void CreateBorderTile(int row, int col, GameObject border, Vector3 pos)
    {
        border.GetComponent<Square>().icon.enabled = false;
        border.transform.position = pos;
        //border.GetComponent<Square>().boxCollider2D.enabled = false;
        border.GetComponent<Square>().isEmpty = true;

        grid[row, col] = null;  // Mark the border tile as null or non-interactive
    }
    private List<int> ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            int temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    public void OnTileSelected(Square selectedTile)
    {
        if (isCheckingMatch) return; // Prevent selection during match checking

        if (firstSelectedTile == null)
        {
            firstSelectedTile = selectedTile;
        }
        else if (secondSelectedTile == null)
        {
            secondSelectedTile = selectedTile;
            isCheckingMatch = true; // Block input
            StartCoroutine(CheckMatch(firstSelectedTile, secondSelectedTile));
        }
    }

    private IEnumerator CheckMatch(Square tile1, Square tile2)
    {
        yield return new WaitForSeconds(0.5f);

        Vector2Int pos1 = GetGridPosition(tile1);
        Vector2Int pos2 = GetGridPosition(tile2);

        // Adjust the positions for the grid with borders
        pos1.x += 1;  // Adjust for border offset
        pos1.y += 1;  // Adjust for border offset
        pos2.x += 1;  // Adjust for border offset
        pos2.y += 1;  // Adjust for border offset

        if (tile1.tileId == tile2.tileId && BFS(pos1, pos2))
        {
            Debug.Log("Match Found!");
            tile1.HideTile();
            tile2.HideTile();
            grid[pos1.x, pos1.y] = null;
            grid[pos2.x, pos2.y] = null;

            // Set the tiles as empty after clearing them
            tile1.isEmpty = true;
            tile2.isEmpty = true;
        }
        else
        {
            Debug.Log("No Match!");
            tile1.ResetFlag(false);
            tile2.ResetFlag(false);
        }

        firstSelectedTile = null;
        secondSelectedTile = null;
        isCheckingMatch = false;
    }

    private bool BFS(Vector2Int start, Vector2Int end)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> parentMap = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);
        parentMap[start] = start; // Root has itself as parent

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            // Debug: Log the current position being processed
            Debug.Log("Processing: " + current);

            // Check if we've reached the end
            if (current == end)
            {
                Debug.Log("Found the path!");
                List<Vector2Int> path = ReconstructPath(parentMap, start, end);
                Debug.Log("Path found: " + string.Join(" -> ", path.Select(p => p.ToString()).ToArray())); // Log path
                StartCoroutine(HighlightPath(path)); // Visual feedback
                return true;
            }

            foreach (var direction in directions)
            {
                Vector2Int neighbor = current + direction;

                // Debug: Log the neighbor being considered
                Debug.Log("Checking neighbor: " + neighbor);

                if (IsValidPosition(neighbor) && !visited.Contains(neighbor))
                {
                    // Check if the neighbor is empty or is the end position
                    if (grid[neighbor.x, neighbor.y] == null || neighbor == end)
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                        parentMap[neighbor] = current; // Track the parent for path reconstruction
                                                       // Debug: Log the new state of the queue and visited nodes
                        Debug.Log("Adding to queue: " + neighbor);
                    }
                }
            }
        }

        return false; // No valid path found
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> parentMap, Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = parentMap[current];
        }

        path.Add(start);
        path.Reverse();
        return path;
    }
    private bool IsValidPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x > rows + 1 || position.y < 0 || position.y > columns + 1)
        {
            return false; 
        }

        if (grid[position.x, position.y] != null && !grid[position.x, position.y].isEmpty)
        {
            return false; 
        }

        return true; 
    }




    private Vector2Int GetGridPosition(Square tile)
    {
        for (int x = 1; x < rows + 1; x++)  // Start from 1 to skip the border
        {
            for (int y = 1; y < columns + 1; y++)  // Start from 1 to skip the border
            {
                if (grid[x, y] == tile)
                    return new Vector2Int(x, y);  // Return position within the playable grid
            }
        }
        return Vector2Int.one * -1; // Not found
    }

    private IEnumerator HighlightPath(List<Vector2Int> path)
    {
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned.");
            yield return null; // Exit if lineRenderer is not assigned
        }

        lineRenderer.positionCount = path.Count;
        Debug.Log($"Path count: {path.Count}");

        // Go through each position in the path
        for (int i = 0; i < path.Count; i++)
        {
            Vector2Int position = path[i];
            Debug.Log($"Processing tile at position: {position}");

            // Adjust for border (assuming a border surrounds the grid)
            // +1 is for the padding for borders (e.g., grid[0,0] is the top-left, grid[rows+1, columns+1] is the bottom-right)
            Vector3 worldPosition = grid[position.x + 1, position.y + 1].transform.localPosition;
            Debug.Log($"World Position of tile: {worldPosition}");

            // Set the position in the LineRenderer
            lineRenderer.SetPosition(i, worldPosition);

            // Highlight the tile
            //grid[position.x + 1, position.y + 1].HighlightTile();
            Debug.Log($"Highlighted tile at position: {position}");

            // Optional: Add a delay between highlighting each tile
            yield return new WaitForSeconds(0.1f);
        }

        // Clear the path after a short delay
        yield return new WaitForSeconds(1f);
        lineRenderer.positionCount = 0;  // Clear the path from the LineRenderer
        Debug.Log("Cleared the LineRenderer path.");
    }



    private readonly Vector2Int[] directions =
    {
        new Vector2Int(0, 1),  // Up
        new Vector2Int(1, 0),  // Right
        new Vector2Int(0, -1), // Down
        new Vector2Int(-1, 0)  // Left
    };
}
