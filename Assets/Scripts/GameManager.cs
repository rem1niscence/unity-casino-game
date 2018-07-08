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
    [SerializeField]
    private GameObject[] playerOneDeck;
    [SerializeField]
    private GameObject[] playerTwoDeck;
    [SerializeField]
    private GameObject[] boardDeck;
    private GameObject selectedCard;
    public GameObject endGamePanel;

    [SerializeField]
    private Text[] gameTexts;

    private int POneScore = 0;
    private int PTwoScore = 0;

    private enum Decks { PlayerOne, PlayerTwo, Board };
    private enum InGameText { Match, POneScore, PTwoScore, RemainingCards,
        Turn, CardValue, Winner };

    private bool _init = false;
    private bool checkForEndGame = false;
    private bool gameOver = false;
    private int cardNum = 0;

    // Refers to the last Player who took cards from the board
    private int lastPlay = -1;

    // 0 means it's player one turn and 1 means player two turn.
    private int turn;

    // Get a reference of all the selectedCards on the board
    public List<GameObject> SelectedCards
    {
        get
        {
            List<GameObject> selectedBoardCards = new List<GameObject>();
            for (int i = 0; i < boardDeck.Length; i++)
            {
                if (boardDeck[i].GetComponent<Card>().Selected)
                    selectedBoardCards.Add(boardDeck[i]);
            }

            return selectedBoardCards;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_init)
        {
            InitializeCards();
            turn = Random.Range(0, 2);
            for (int i = 0; i < 4; i++)
                if (turn == 0)
                    playerOneDeck[i].GetComponent<Card>().flipCard();
                else if (turn == 1)
                    playerTwoDeck[i].GetComponent<Card>().flipCard();
            WriteTextOnScreen(InGameText.Turn, "Turno de: Jugador " + (turn + 1));
        }
        
        if (!gameOver) {
            if (checkForEndGame) {
                EndGame();
            } else {
                CheckEmptyDeck(Decks.PlayerOne);
                CheckEmptyDeck(Decks.PlayerTwo);
                CheckEmptyDeck(Decks.Board);
            }
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
                // Debug.Log(id+(13*i));
                cards[choice].GetComponent<Card>().ID = id+(13*i);
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
            if (cardNum > 51) {
                checkForEndGame = true;
                break;
            }
            deck[i].GetComponent<Card>().CardValue = cards[cardNum]
                .GetComponent<Card>().CardValue;
            deck[i].GetComponent<Card>().ID = cards[cardNum]
                .GetComponent<Card>().ID;
            deck[i].GetComponent<Card>().setupGraphics();

            if (option == Decks.Board)
                deck[i].GetComponent<Card>().flipCard();

            cardNum++;

        }

        // All of this is to prevent a bug when a next turn occurs after a 
        // player card has ran out, the board appears with all cards facing upwards.
        int upsideCards = 0;
        for (int i = 0; i < 4; i++)
        {
            if (playerOneDeck[i].GetComponent<Card>().State == 1)
                upsideCards++;
            if (playerTwoDeck[i].GetComponent<Card>().State == 1)
                upsideCards++;
        }

        if (upsideCards == 8)
        {
            deck = (turn == 0) ? playerTwoDeck : playerOneDeck;
            for (int i = 0; i < 4; i++)
            {
                deck[i].GetComponent<Card>().flipCard();
            }
        }

        // To print on the screen how many cards are remaining
        string cardsLeftTxt = "Cartas restantes: " + (52 - cardNum);
        WriteTextOnScreen(InGameText.RemainingCards, cardsLeftTxt);
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
            {
                selectedCard = card;
                HightLightCard(card, true);
            }
            else
            {
                selectedCard = null;
                HightLightCard(card, false);
            }
        }
    }

    public void SelectBoardCard(int index)
    {
        GameObject card = boardDeck[index];
        bool isSelected = card.GetComponent<Card>().Selected;
        isSelected = !isSelected;
        card.GetComponent<Card>().Selected = isSelected;
        HightLightCard(card, isSelected);
        string msg = string.Format("Valor de la carta: {0}" + 
        "\nCantidad de cartas: {1}", 
            card.GetComponent<Card>().CardValue + 1,
            card.GetComponent<Card>().Quantity);
        WriteTextOnScreen(InGameText.CardValue, msg);
    }

    void HightLightCard(GameObject card, bool hightlight)
    {
        if (hightlight)
        {
            card.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f);
        }
        else
        {
            card.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
    }

    public void ClearCard(GameObject card)
    {
        card.GetComponent<Card>().CardValue = -1;
        card.GetComponent<Card>().Selected = false;
        card.GetComponent<Card>().Initialized = false;
        card.GetComponent<Card>().Quantity = 1;
        card.GetComponent<Card>().ShowEmptyCard();
        HightLightCard(card, false);
    }

    void CheckEmptyDeck(Decks option)
    {
        GameObject[] deck = { };
        if (option == Decks.PlayerOne)
            deck = playerOneDeck;
        else if (option == Decks.PlayerTwo)
            deck = playerTwoDeck;
        else if (option == Decks.Board)
            deck = boardDeck;

        int emptySlots = 0;
        for (int i = 0; i < deck.Length; i++)
        {
            int cardValue = deck[i].GetComponent<Card>().CardValue;
            if (cardValue < 0)
                emptySlots++;
        }
        if (emptySlots == deck.Length)
            FillDeck(option);
    }

    void WriteTextOnScreen(InGameText where, string msg)
    {
        gameTexts[(int)where].text = msg;
    }

    public void ChangeTurn()
    {
        selectedCard = null;
        if (turn == 1)
            turn = 0;
        else if (turn == 0)
            turn = 1;

        for (int i = 0; i < 4; i++)
        {
            playerOneDeck[i].GetComponent<Card>().flipCard();
            playerTwoDeck[i].GetComponent<Card>().flipCard();
        }
    }

    // Plays the normal game, check if sum of the selected cards on the board
    // is equal to the seletec card.
    public void Play()
    {
        int sum = 0;
        List<GameObject> cards = SelectedCards;
        foreach (GameObject card in cards)
        {
            sum += card.GetComponent<Card>().CardValue;
            sum++;
        }

        if (selectedCard != null)
        {
            if ((selectedCard.GetComponent<Card>().CardValue + 1) == sum)
            {
                ClearCard(selectedCard);
                selectedCard = null;
                int points = 0;
                foreach (GameObject card in cards)
                {
                    points += card.GetComponent<Card>().Quantity;
                    ClearCard(card);
                }
                if (turn == 0)
                {
                    POneScore += points;
                    string msg = "Jugad@r 1: " + POneScore + " puntos";
                    WriteTextOnScreen(InGameText.POneScore, msg);
                }
                else if (turn == 1)
                {
                    PTwoScore += points;
                    string msg = "Jugad@r 2: " + PTwoScore + " puntos";
                    WriteTextOnScreen(InGameText.PTwoScore, msg);
                }
                WriteTextOnScreen(InGameText.Match, "");
                lastPlay = turn;
                ChangeTurn();
            }
            else
            {
                string msg = "No suman lo mismo";
                WriteTextOnScreen(InGameText.Match, msg);
            }
        }
        else
        {
            WriteTextOnScreen(InGameText.Match, "Selecciona una carta");
        }
    }

    // Place a selected card on the board
    public void Place()
    {
        if (selectedCard != null)
        {
            int i = 0;
            for (; i < boardDeck.Length; i++)
            {
                if (boardDeck[i].GetComponent<Card>().CardValue < 0)
                    break;
            }
            boardDeck[i].GetComponent<Card>().CardValue =
                selectedCard.GetComponent<Card>().CardValue;
            boardDeck[i].GetComponent<Card>().Initialized = true;
            boardDeck[i].GetComponent<Card>().setupGraphics();
            boardDeck[i].GetComponent<Card>().flipCard();

            ClearCard(selectedCard);
            selectedCard = null;
            ChangeTurn();
        }
        else
        {
            WriteTextOnScreen(InGameText.Match, "Selecciona una carta");
        }
    }

    public void BuildComb()
    {
        List<GameObject> cards = SelectedCards;
        if (selectedCard != null)
        {
            if (cards.Count == 1)
            {
                int sum = cards[0].GetComponent<Card>().CardValue +
                    selectedCard.GetComponent<Card>().CardValue + 2;
                GameObject[] deck = (turn == 0) ? playerOneDeck : 
                playerTwoDeck;
                bool canBuild = false;
                for (int i = 0; i < 4; i++)
                {
                    
                    if (deck[i].GetComponent<Card>().CardValue+1 == sum)
                    {
                        canBuild = true;
                        break;
                    }
                }

                if (canBuild)
                {
                    cards[0].GetComponent<Card>().CardValue = sum-1;
                    cards[0].GetComponent<Card>().Quantity++;
                    ClearCard(selectedCard);
                    selectedCard = null;
                    string msg = "Contruyendo " + sum;
                    WriteTextOnScreen(InGameText.Match, msg);
                    ChangeTurn();
                } else
                {
                    WriteTextOnScreen(InGameText.Match, "Construccion no posible");
                }
            }
            else
            {
                WriteTextOnScreen(InGameText.Match,
                    "Solo puedes seleccionar una carta del tablero para construir");
            }
        } else
        {
            WriteTextOnScreen(InGameText.Match, "Selecciona una carta");
        }
    }

    public void CallComb() {
        List<GameObject> cards = SelectedCards;
        if (selectedCard != null)
        {
            if (cards.Count == 1)
            {
                GameObject[] deck = (turn == 0) ? playerOneDeck : playerTwoDeck;
                bool canCall = false;
                for (int i = 0; i < 4; i++)
                {
                    
                    if (deck[i].GetComponent<Card>().CardValue == 
                    cards[0].GetComponent<Card>().CardValue)
                    {
                        canCall = true;
                        break;
                    }
                }

                if (canCall)
                {
                    
                    cards[0].GetComponent<Card>().Quantity++;
                    string msg = "Llamando " + (selectedCard.GetComponent<Card>()
                    .CardValue+1);
                    WriteTextOnScreen(InGameText.Match, msg);
                    ClearCard(selectedCard);
                    selectedCard = null;
                    ChangeTurn();
                } else
                {
                    WriteTextOnScreen(InGameText.Match, "Llamada no posible");
                }
            }
            else
            {
                WriteTextOnScreen(InGameText.Match,
                    "Solo puedes seleccionar una carta del tablero para Llamar");
            }
        } else
        {
            WriteTextOnScreen(InGameText.Match, "Selecciona una carta");
        }
    }

    void EndGame() {
        
        int pOneCardsLeft = 4;
        int pOTwoCardsLeft = 4;
        for (int i = 0; i < 4; i++) {
            if (playerOneDeck[i].GetComponent<Card>().CardValue < 0)
                pOneCardsLeft--;
            if (playerTwoDeck[i].GetComponent<Card>().CardValue < 0)
                pOTwoCardsLeft--;
        }
        if (pOneCardsLeft == 0 && pOTwoCardsLeft == 0) {
            int remainingPoints = 0;
            for (int i = 0; i < boardDeck.Length; i++) 
                if (boardDeck[i].GetComponent<Card>().Initialized)
                    remainingPoints += 
                        boardDeck[i].GetComponent<Card>().Quantity;
            
            if (lastPlay == 0) 
                POneScore += remainingPoints;
            else if (lastPlay == 1)
                PTwoScore += remainingPoints;
            
            string winner = "";
            if (POneScore > PTwoScore) 
                winner = "JUGAD@R 1 HA GANADO CON " + POneScore + " PUNTOS!";
            else if (POneScore < PTwoScore) 
                winner = "JUGAD@R 2 HA GANADO CON " + PTwoScore + " PUNTOS!";
            else if (POneScore == PTwoScore) 
                winner = "EMPATE!";
            
            WriteTextOnScreen(InGameText.Winner, winner);
            gameOver = true;
            endGamePanel.SetActive(true);
        }
    }
}