using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gems : Minigame
{
    public RectTransform container;
    public GameObject[] gemPrefabs;

    public int gridDims;

    public float gridPaddingLeft;
    public float gridPaddingRight;
    public float gridPaddingTop;
    public float gridPaddingBottom;

    public Rect gridRect;
    private Vector2 spacingScale;

    public enum GemType
    {
        Dexter,
        Blue,
        Purple,
        Heart,
        Green
    };
    public List<Gem> gems;

    public RectTransform selector;

    private Vector2Int selectionCoords;
    public Gem selectedGem;

    private void Awake()
    {
        Initialize();

        gridRect = new Rect(
            Mathf.Lerp(0, container.rect.width, gridPaddingLeft),
            Mathf.Lerp(0, container.rect.height, gridPaddingTop),
            container.rect.width * (1f - gridPaddingLeft + gridPaddingRight),
            container.rect.height * (1f - gridPaddingTop + gridPaddingBottom)
        );
    }

    void Start()
    {
        gems = new List<Gem>();

        for(int i = 0; i < gridDims; i++)
        {   
            for(int j = 0; j < gridDims; j++)
            {
                gems.Add(Instantiate(gemPrefabs[i], container.transform).GetComponent<Gem>());
            }
        }

        ShuffleGems();

        selectionCoords = new Vector2Int(0, 0);
        MoveSelection(Vector2Int.zero);
        selectedGem = null;

        PositionGems();
    }

    void Update()
    {
        PositionGems();

        bool left = Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame;
        bool right = Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame;
        bool up = Keyboard.current.sKey.wasPressedThisFrame || Keyboard.current.downArrowKey.wasPressedThisFrame;
        bool down = Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame;

        Vector2Int direction = (left ? Vector2Int.left : Vector2Int.zero) +
            (right ? Vector2Int.right : Vector2Int.zero) +
            (down ? Vector2Int.up : Vector2Int.zero) +
            (up ? Vector2Int.down : Vector2Int.zero);
        bool movement = direction.sqrMagnitude != 0;

        bool select = Keyboard.current.jKey.isPressed || Keyboard.current.zKey.isPressed;

        if (select)
        {
            selectedGem = gems[selectionCoords.y * gridDims + selectionCoords.x];

            if (movement)
            {
                bool successfulSwap = SwapGems(direction);

                if (successfulSwap)
                {
                    bool successfulRemoval = RemoveLine();
                    if (successfulRemoval)
                    {
                        if (RemoveLine())
                        {
                            if (!BoardClear())
                            {
                                Debug.Log("The player has one");
                            }
                        }
                    }

                    MoveSelection(direction);
                }

                selectedGem = null;
            }
        }
        else
        {
            if(movement)
                MoveSelection(direction);
        }
    }
	
	void PositionGem()
	{
		
	}

    void PositionGems()
    {
        spacingScale = new Vector2(gridRect.width / gridDims, gridRect.height / gridDims);

        // Calculate the bounds of the array of gems grid
        gridRect = new Rect(
            Mathf.Lerp(0, container.rect.width, gridPaddingLeft),
            Mathf.Lerp(0, container.rect.height, gridPaddingTop),
            container.rect.width * (1f - gridPaddingLeft - gridPaddingRight),
            container.rect.height * (1f - gridPaddingTop - gridPaddingBottom)
        );

        // Arrange all of the gems
        for(int i = 0; i < gridDims; i++)
        {   
            for(int j = 0; j < gridDims; j++)
            {
                Gem g = gems[i * gridDims + j];

                RectTransform gemRect = g.GetComponent<RectTransform>();
                gemRect.localPosition = new Vector2(
                    gridRect.x + spacingScale.x * j + spacingScale.x / 2,
                    gridRect.y + spacingScale.y * i + spacingScale.y / 2
                );
            }
        }
    }

    void MoveSelection(Vector2Int delta)
    {
        // Bound the selection to the grid dimensions
        selectionCoords = new Vector2Int(
            Math.Clamp(selectionCoords.x + delta.x, 0, gridDims - 1),
            Math.Clamp(selectionCoords.y + delta.y, 0, gridDims - 1)
        );

        // Move selection above new gem
        selector.SetParent(gems[selectionCoords.y * gridDims + selectionCoords.x].transform);
        selector.localPosition = Vector2.zero;
    }

    bool SwapGems(Vector2Int delta)
    {
        Vector2Int swapTarget = selectionCoords + delta;
        
        // If swap target is out of bounds
        if(!InBounds(swapTarget))
        {
            return false;
        }

        // Swap the two gems
        Gem t = gems[selectionCoords.y * gridDims + selectionCoords.x];
        gems[selectionCoords.y * gridDims + selectionCoords.x] = gems[swapTarget.y * gridDims + swapTarget.x];
        gems[swapTarget.y * gridDims + swapTarget.x] = t;

        gems[swapTarget.y * gridDims + swapTarget.x].SetMovement(new Vector2(delta.x * spacingScale.x, delta.y * spacingScale.y));
        gems[selectionCoords.y * gridDims + selectionCoords.x].SetMovement(new Vector2(delta.x * -spacingScale.x, delta.y * -spacingScale.y));

        PositionGems();

        return true;
    }

    // Return true if line is remove successfuly
    bool RemoveLine()
    {
        for (int i = 0; i < gridDims; i++)
        {
            Gem horizGem = gems[i * gridDims];
            Gem vertGem = gems[i];

            int horizMatch = 0;
            int vertMatch = 0;

            for (int j = 0; j < gridDims; j++) 
            {
                if (horizGem != null)
                {
                    Gem t = gems[i * gridDims + j];
                    if (t != null && !t.collected && t.type == horizGem.type) {
                        horizMatch++;
                    }
                }

                if (vertGem != null)
                {
                    Gem t = gems[j * gridDims + i];
                    if (t != null && !t.collected && t.type == vertGem.type) {
                        vertMatch++;
                    }
                }
            }

            if (horizMatch == gridDims)
            {
                for (int k = 0; k < gridDims; k++)
                {
                    gems[i * gridDims + k].collected = true;
                    gems[i * gridDims + k].GetComponent<Image>().enabled = false;
                }
                return true;
            }
            else if (vertMatch == gridDims)
            {
                for (int k = 0; k < gridDims; k++)
                {
                    gems[k * gridDims + i].collected = true;
                    gems[k * gridDims + i].GetComponent<Image>().enabled = false;
                }
                return true;
            }
        }

        return false;
    }

    bool InBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridDims && pos.y >= 0 && pos.y < gridDims;
    }

    bool BoardClear()
    {   
        for(int i = 0; i < gridDims; i++)
        {   
            for(int j = 0; j < gridDims; j++)
            {
                if(!gems[i * gridDims + j].collected)
                    return false;
            }
        }

        return true;
    }

    void ShuffleGems()
    {
        int count = gems.Count;
        int last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            int r = UnityEngine.Random.Range(i, count);
            Gem tmp = gems[i];
            gems[i] = gems[r];
            gems[r] = tmp;
        }
    }
}
