using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mirrors : Minigame
{

    public RectTransform container;
    public Vector2Int gridDims;

    public float gridPaddingLeft;
    public float gridPaddingRight;
    public float gridPaddingTop;
    public float gridPaddingBottom;

    public Rect gridRect;
    public Vector2 gridScale;

    public GameObject mirrorPrefab;

    public MirrorTile[] mirrorTiles;

    public Vector2Int source;
    public Vector2Int sink;

    public Vector2Int[] publicPathCoords;

    public Animator selectAnimator;
    private Vector2Int selectionCoords = new Vector2Int(0, 0);
    public EaseToOrigin selectorPosition;

    public GameObject lazer;
    public GameObject bend;
    public List<RectTransform> lazerPath;

    public Animator buttonAnimator;
    public Animator planetAnimator;

    private void Awake()
    {
        UIManager.SetNotif(
            "<HOLD> Z + WASD",
            Color.black,
            20
        );
        Initialize();
        OnGameFinish.AddListener(() => {
            DepotController.PlayerInventory.Packages.Remove("Mirrors");
            Depot.UpdatePackagesUI();
        });
        OnGameFinish.AddListener(() => Door.MinigameStarted = false);
    }

    void Start()
    {
        mirrorTiles = new MirrorTile[gridDims.x * gridDims.y];

        source = new Vector2Int(0, (int)Mathf.Floor(gridDims.y / 2));
        sink = new Vector2Int(gridDims.x - 1, (int)Mathf.Floor(gridDims.y / 2));

        for(int i = 0; i < mirrorTiles.Length; i++)
        {
            mirrorTiles[i] = null;
        }

        gridRect = new Rect(
            Mathf.Lerp(0, container.rect.width, gridPaddingLeft),
            Mathf.Lerp(0, container.rect.height, gridPaddingTop),
            Mathf.Lerp(0, container.rect.width, gridPaddingLeft) - Mathf.Lerp(0, container.rect.width, 1.0f - gridPaddingRight),
            Mathf.Lerp(0, container.rect.height, gridPaddingTop) - Mathf.Lerp(0, container.rect.height, 1.0f - gridPaddingBottom)
        );

        gridScale = new Vector2(gridRect.width / gridDims.x, gridRect.height / gridDims.y);

        Vector2Int[] pathCoords;
        do
        {
            pathCoords = CreatePath();
        } while(pathCoords.Length == 0);
        publicPathCoords = pathCoords;

        // Get the bends in the path
        for(int i = 1; i < pathCoords.Length - 1; i++)
        {
            if(pathCoords[i - 1] - pathCoords[i] != pathCoords[i] - pathCoords[i + 1])
            {
                mirrorTiles[pathCoords[i].y * gridDims.x + pathCoords[i].x] = Instantiate(mirrorPrefab).GetComponent<MirrorTile>();
                mirrorTiles[pathCoords[i].y * gridDims.x + pathCoords[i].x].rectTransform.SetParent(container, false);
            }
        }

        PositionTiles();

        lazerPath = new List<RectTransform>();
        PathLazer();
    }

    void Update()
    {
        PositionTiles();
        MoveSelection(Vector2Int.zero);

        bool left = Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame;
        bool right = Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame;
        bool up = Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame;
        bool down = Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame;

        bool cClock = Keyboard.current.jKey.wasPressedThisFrame || Keyboard.current.zKey.wasPressedThisFrame;
        bool clock = Keyboard.current.kKey.wasPressedThisFrame || Keyboard.current.xKey.wasPressedThisFrame;

        Vector2Int direction = (left ? Vector2Int.left : Vector2Int.zero) +
            (right ? Vector2Int.right : Vector2Int.zero) +
            (down ? Vector2Int.up : Vector2Int.zero) +
            (up ? Vector2Int.down : Vector2Int.zero);
        bool movement = direction.sqrMagnitude != 0;

        if(movement)
            MoveSelection(direction);

        PathLazer();
        if (clock || cClock)
        {
            MirrorTile mt = mirrorTiles[selectionCoords.y * gridDims.x + selectionCoords.x];
            if (mt != null)
            {
                AudioManager.PlaySFX("MirrorRotate");
                mt.RotateClockwise(clock);
            }
        }
    }

    void PathLazer()
    {
        foreach (var piece in lazerPath)
            Destroy(piece.gameObject);

        lazerPath = new List<RectTransform>();

        Vector2Int current = source;
        Vector2Int dir = Vector2Int.right;

        while (true)
        {
            if (current == sink + Vector2Int.right)
            {
                RectTransform endLazer = Instantiate(lazer, container).GetComponent<RectTransform>();
                endLazer.localPosition = (sink + Vector2Int.right) * gridScale + gridRect.position + (gridScale / 2f);
                endLazer.localRotation = Quaternion.Euler(0f, 0f, 0f);
                lazerPath.Add(endLazer);

                buttonAnimator.Play("ButtonGlow");
                planetAnimator.Play("PlanetDead");

                StartCoroutine(StartWin());

                break;
            }

            if (!InBounds(current))
                break;

            Vector2 worldPos = current * gridScale + gridRect.position + (gridScale / 2f);

            MirrorTile mirror = mirrorTiles[current.y * gridDims.x + current.x];
            RectTransform piece;

            if (mirror == null)
            {
                piece = Instantiate(lazer, container).GetComponent<RectTransform>();

                piece.localPosition = worldPos;
                piece.localRotation = Quaternion.Euler(0f, 0f, MirrorTile.DirectionToRotation(dir) + 90f);

                lazerPath.Add(piece);
            }
            else
            {
                piece = Instantiate(bend, container).GetComponent<RectTransform>();
                piece.gameObject.name = "(" + worldPos.x + ", " + worldPos.y + ")";

                Vector2Int newDir = mirror.GetReflection(-dir);

                if(newDir == Vector2Int.zero)
                {
                    Destroy(piece.gameObject);
                    break;
                }

                lazerPath.Add(piece);

                dir = newDir;

                piece.localPosition = worldPos;
                piece.localRotation = Quaternion.Euler(0f, 0f, MirrorTile.DirectionToRotation(mirror.direction));
            }

            if (current == sink)
            {
                RectTransform endLazer = Instantiate(lazer, container).GetComponent<RectTransform>();
                endLazer.localPosition = (sink + Vector2Int.right) * gridScale + gridRect.position + (gridScale / 2f);
                endLazer.localRotation = Quaternion.Euler(0f, 0f, 0f);
                lazerPath.Add(endLazer);

                Invoke(nameof(Finish), 1f);
                break;
            }

            current += dir;
        }

        RectTransform startLazer = Instantiate(lazer, container).GetComponent<RectTransform>();
        startLazer.localPosition = (source + Vector2Int.left) * gridScale + gridRect.position + (gridScale / 2f);
        startLazer.localRotation = Quaternion.Euler(0f, 0f, 0f);
        lazerPath.Add(startLazer);
    }

    void PositionTiles()
    {
        gridRect = new Rect(
            container.rect.width * gridPaddingLeft,
            container.rect.height * gridPaddingTop,
            container.rect.width * (1f - gridPaddingLeft - gridPaddingRight),
            container.rect.height * (1f - gridPaddingTop - gridPaddingBottom)
        );

        gridScale = new Vector2(gridRect.width / gridDims.x, gridRect.height / gridDims.y);

        for(int i = 0; i < gridDims.x; i++)
        {
            for(int j = 0; j < gridDims.y; j++)
            {
                MirrorTile mt = mirrorTiles[j * gridDims.x + i];
                if(mt == null) continue;

                mt.rectTransform.localPosition = new Vector2(
                    gridScale.x * i + gridRect.x + (gridScale.x / 2),
                    gridScale.y * j + gridRect.y + (gridScale.y / 2)
                );
            }
        }
    }

    Vector2Int[] CreatePath()
    {
        Stack<Vector2Int> pathCoords = new();

        pathCoords.Push(new Vector2Int(-1, (int)Mathf.Floor(gridDims.y / 2)));
        pathCoords.Push(new Vector2Int(0, (int)Mathf.Floor(gridDims.y / 2)));

        if(NextPath(pathCoords, Vector2Int.left))
        {
            Debug.Log("Valid path found");
            Debug.Log(pathCoords);
        }

        pathCoords.Push(new Vector2Int(gridDims.x, (int)Mathf.Floor(gridDims.y / 2)));

        return pathCoords.Reverse().ToArray();
    }

    bool NextPath(Stack<Vector2Int> pathCoords, Vector2Int from)
    {
        if(pathCoords.Peek() == sink)
            return true;

        Vector2Int[] possibleDirections = new Vector2Int[] {Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
        
        // Maybe we should heuristic the start
        possibleDirections = Shuffle(possibleDirections.Where(dir => dir != from).ToArray());

        for(int i = 0; i < possibleDirections.Length; i++)
        {
            Vector2Int next = pathCoords.Peek() + possibleDirections[i];

            if(!InBounds(next) || pathCoords.Contains(next)) 
                continue;

            pathCoords.Push(next);

            if(NextPath(pathCoords, -possibleDirections[i]))
                return true;

            pathCoords.Pop();
        }

        return false;
    }

    void MoveSelection(Vector2Int delta)
    {
        // Bound the selection to the grid dimensions
        selectionCoords = new Vector2Int(
            Math.Clamp(selectionCoords.x + delta.x, 0, gridDims.x - 1),
            Math.Clamp(selectionCoords.y + delta.y, 0, gridDims.y - 1)
        );

        selectorPosition.SetPosition(selectionCoords * gridScale + gridRect.position + (gridScale / 2f));
    }

    bool InBounds(Vector2Int coords)
    {
        return coords.x >= 0
            && coords.y >= 0
            && coords.x < gridDims.x
            && coords.y < gridDims.y;
    }

    Vector2Int[] Shuffle(Vector2Int[] list)
    {
        Vector2Int[] output = new Vector2Int[list.Length];
        Array.Copy(list, output, list.Length);
        for (int i = output.Length - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, output.Length - 1);
            (output[i], output[j]) = (output[j], output[i]);
        }
        return output;
    }

    public static Vector2Int RotateClockwise(Vector2Int direction, bool c)
    {
        if(direction == Vector2Int.up) 
            return (c ? Vector2Int.right : Vector2Int.left);
        else if(direction == Vector2Int.right)
            return (c ? Vector2Int.down : Vector2Int.up);
        else if(direction == Vector2Int.down)
            return (c ? Vector2Int.left : Vector2Int.right);
        else if(direction == Vector2Int.left)
            return (c ? Vector2Int.up: Vector2Int.down);
        return direction;
    }

    IEnumerator StartWin()
    {
        float timer = 2.5f;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Mirrors WON");
        Finish();
    } 
}