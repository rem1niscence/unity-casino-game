using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public Sprite[] cardFace;
    public Sprite cardBack;
    public Sprite cardEmpty;
    public GameObject[] cards;
    public Text matchText;
    public GameObject[] playerOneDeck;
    public GameObject[] playerTwoDeck;
    public GameObject[] boardDeck;
    // private GameObject POneSelectedCard;
    // private GameObject PTwoSelectedCard;
    private GameObject selectedCard;

    private enum Decks { PlayerOne, PlayerTwo, Board };

    private bool _init = false;
    private int _matches = 13;
    private int cardNum = 0;

    // Update is called once per frame
    void Update()
    {
        if (!_init)
            InitializeCards();
        if (Input.GetMouseButtonUp(0))
        {
            CheckCard();
        }
    }

    void InitializeCards()
    {
        for (int id = 0; id < 13; id++)
        {
            for (int i = 0; i < 4; i++)
            {
                bool test = false;
                int choice = 0;
                while (!test)
                {
                    choice = Random.Range(0, cards.Length);
                    test = !(cards[choice].GetComponent<Card>().Initialized);
                }
                cards[choice].GetComponent<Card>().CardValue = id;
                cards[choice].GetComponent<Card>().Initialized = true;
            }
        }

        foreach (GameObject c in cards)
        {
            c.GetComponent<Card>().setupGraphics();

            if (!_init)
            {
                _init = true;
            }
        }

        FillDeck(Decks.PlayerOne);
        FillDeck(Decks.PlayerTwo);
        FillDeck(Decks.Board);
    }

    public Sprite getCardBack()
    {
        return cardBack;
    }

    public Sprite getCardEmpty()
    {
        return cardEmpty;
    }

    public Sprite getCardFace(int i)
    {
        return cardFace[i];
    }

    void CheckCard()
    {
        List<int> c = new List<int>();
        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i].GetComponent<Card>().State == 1)
            {
                c.Add(i);
            }
        }

        if (c.Count == 2)
        {
            CardComparison(c);
        }
    }

    void CardComparison(List<int> c)
    {
        Card.DO_NOT = true;
        int x = 0;
        if (cards[c[0]].GetComponent<Card>().CardValue ==
            cards[c[1]].GetComponent<Card>().CardValue)
        {
            x = 2;
            _matches--;
            matchText.text = "Number of Matches: " + _matches;
            if (_matches == 0)
                SceneManager.LoadScene("Menu");
        }

        for (int i = 0; i < c.Count; i++)
        {
            cards[c[i]].GetComponent<Card>().State = x;
            cards[c[i]].GetComponent<Card>().falseCheck();
        }
    }

    void FillDeck(Decks option)
    {
        GameObject[] deck = { };

        if (option == Decks.PlayerOne)
            deck = playerOneDeck;
        else if (option == Decks.PlayerTwo)
            deck = playerTwoDeck;
        else if (option == Decks.Board)
            deck = boardDeck;

        for (int i = 0; i < 4; i++)
        {
            if (cardNum > 51)
                break;
            Debug.Log(i);
            deck[i].GetComponent<Card>().CardValue = cards[cardNum]
                .GetComponent<Card>().CardValue;
            deck[i].GetComponent<Card>().setupGraphics();

            // if (option == Decks.Board)
                deck[i].GetComponent<Card>().flipCard();

            cardNum++;

        }
    }

    public void SelectPlayerCard(string data)
    {
        // If player == 0 then it's player One consecu and 
        // accordingly if player == 1 then it's player two.
        int player = data[0] - '0';
        // Represent a player card of the deck, ranging from 0 to 3 left to right.
        int cardSelected = data[1] - '0';
        GameObject card = null;

        if (player == 0)
            card = playerOneDeck[cardSelected];
        else if (player == 1)
            card = playerTwoDeck[cardSelected];

        if (card.GetComponent<Card>().State != 0)
        {
            // To clean all previosuly selected cards so only one card can
            // be selected at a time.
            for (int i = 0; i < 4; i++)
            {
                if (i == cardSelected)
                    continue;

                if (player == 0)
                {
                    playerOneDeck[i].GetComponent<Card>().Selected = false;
                    HightLightCard(playerOneDeck[i], false);
                }
                else if (player == 1)
                {
                    playerTwoDeck[i].GetComponent<Card>().Selected = false;
                    HightLightCard(playerTwoDeck[i], false);
                }
            }

            // So the user is able to unselect the card by clicking again on it.
            bool isSelected = card.GetComponent<Card>().Selected;
            isSelected = !isSelected;
            card.GetComponent<Card>().Selected = isSelected;

            if (isSelected)
                HightLightCard(card, true);
            else
                HightLightCard(card, false);
        } 
    }

    public void SelectBoardCard(int index)
    {
        GameObject card = boardDeck[index];
        bool isSelected = card.GetComponent<Card>().Selected;
        isSelected = !isSelected;
        card.GetComponent<Card>().Selected = isSelected;
        HightLightCard(card, isSelected);
    }

    void HightLightCard(GameObject card, bool hightlight)
    {
        if(hightlight)
        {
            card.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
            Debug.Log(card.GetComponent<Card>().CardValue+1);
        } else
        {
            card.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }
}
