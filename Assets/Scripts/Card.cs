using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{

    public static bool DO_NOT = false;

    [SerializeField]
    private int _state;
    [SerializeField]
    private bool _initialized = false;
    [SerializeField]
    private int _cardValue;
    private bool _selected = false;

    private Sprite _cardBack;
    private Sprite _cardFace;
    private Sprite _cardEmpty;
    private Sprite _cardSelected;


    private GameObject _manager;

    void Start()
    {
        _state = 1;
        _manager = GameObject.FindGameObjectWithTag("Manager");
    }

    public void setupGraphics()
    {
        _cardBack = _manager.GetComponent<GameManager>().getCardBack();
        _cardFace = _manager.GetComponent<GameManager>().getCardFace(_cardValue);
        _cardEmpty = _manager.GetComponent<GameManager>().getCardEmpty();

        flipCard();
    }

    public void flipCard()
    {
        if (CardValue >= 0)
        {
            if (_state == 0)
                _state = 1;
            else if (_state == 1)
                _state = 0;
            if (_state == 0 && !DO_NOT)
                GetComponent<Image>().sprite = _cardBack;
            else if (_state == 1 && !DO_NOT)
                GetComponent<Image>().sprite = _cardFace;
        }
    }

    public void ShowEmptyCard()
    {
        GetComponent<Image>().sprite = _cardEmpty;
    }

    public int CardValue
    {
        get { return _cardValue; }
        set { _cardValue = value; }
    }

    public int State
    {
        get { return _state; }
        set { _state = value; }
    }

    public bool Selected
    {
        get { return _selected; }
        set { if (_state == 1) _selected = value; }
    }

    public bool Initialized
    {
        get { return _initialized; }
        set { _initialized = value; }
    }

    public void falseCheck()
    {
        StartCoroutine(pause());
    }

    IEnumerator pause()
    {
        yield return new WaitForSeconds(1);
        if (_state == 0)
            GetComponent<Image>().sprite = _cardBack;
        else if (_state == 1)
            GetComponent<Image>().sprite = _cardFace;
        DO_NOT = false;
    }
}