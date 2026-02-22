using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;

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

    private Vector2Int source;
    private Vector2Int sink;

    public Vector2Int[] publicPathCoords;

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

        Vector2Int[] pathCoords = CreatePath();
        publicPathCoords = pathCoords;

        // Get the bends in the path
        Debug.Log(source);
        for(int i = 1; i < pathCoords.Length - 1; i++)
        {
            Debug.Log(pathCoords[i - 1]);
            if(pathCoords[i - 1] - pathCoords[i] != pathCoords[i] - pathCoords[i + 1])
            {
                Debug.Log("Corner found");
                mirrorTiles[pathCoords[i].y * gridDims.x + pathCoords[i].x] = Instantiate(mirrorPrefab).GetComponent<MirrorTile>();
                mirrorTiles[pathCoords[i].y * gridDims.x + pathCoords[i].x].rectTransform.SetParent(container, false);
            }
        }
        Debug.Log(sink);

        PositionTiles();
    }

    void Update()
    {
        PositionTiles();
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
}