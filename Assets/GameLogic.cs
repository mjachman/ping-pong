using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public int p1Score;
    public int p2Score;
    //public int servingPlayer;
    public int lastHit;
    public Vector2 servePosition = new Vector2(-14, -3);
    public float serveSpeed = 5;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p1sets, p2sets;

    public GameObject ball;
    public GameObject TableTop;
    public AudioSource audioPlayer;
    public AudioClip clap, chant;

    public TextMeshProUGUI winnerText;
    public Animator animator;

    public int p1SetScore = 0;
    public int p2SetScore = 0;
    public int sets = 3;

    public PaddleScript paddle1;
    public PaddleScript paddle2;

    public PaddleScript servingPaddle;

    public int servingPlayer;

    public GameObject newGameButon;

    public void addScore(int player)
    {
        if (player == 0)
        {
            p1Score++;
            p1ScoreText.text = p1Score.ToString();

        }
        else if (player == 1)
        {
            p2Score++;
            p2ScoreText.text = p2Score.ToString();
        }
    }
    public void resetScore()
    {
        p1Score = 0;
        p1ScoreText.text = p1Score.ToString();
        p2Score = 0;
        p2ScoreText.text = p2Score.ToString();
    }
    public void changeServer()
    {
        //animator.enabled = true;

        //animator.enabled = false;
        endSet();

        int totalScore = p1Score + p2Score;

        if (totalScore >= 20 || (totalScore % 2 == 0 && totalScore != 0))
        {
            SwitchServingPaddle();
            servingPlayer = -servingPlayer;
            servePosition.x = -servePosition.x;
        }
    }
    public void SwitchServingPaddle()
    {
        if (servingPaddle == paddle1)
        {
            servingPaddle = paddle2;
            paddle1.serving = false;
            paddle2.serving = true;
            
        }
        else if (servingPaddle == paddle2)
        {
            servingPaddle = paddle1;
            paddle1.serving = true;
            paddle2.serving = false;
            
        }
    }

    public void endSet()
    {
        if (p1Score >= 11 || p2Score >= 11)
        {
            if (Mathf.Abs(p1Score - p2Score) >= 2)
            {
                if (p1Score > p2Score)
                {
                    Debug.Log("Seta wygrywa gracz1");
                    winnerText.text = "Seta wygrywa gracz 1!";
                    p1SetScore++;
                    p1sets.text += "I";
                }
                else
                {
                    Debug.Log("Seta wygrywa gracz2");

                    winnerText.text = "Seta wygrywa gracz 2!";
                    p2SetScore++;
                    p2sets.text += "I";
                }

                if (p1SetScore == sets)
                {
                    winnerText.text = "Gracz 1 wygrywa mecz!";
                    audioPlayer.clip = chant;
                    newGameButon.SetActive(true);
                }
                else if (p2SetScore == sets)
                {
                    winnerText.text = "Gracz 2 wygrywa mecz!";
                    audioPlayer.clip = chant;
                    newGameButon.SetActive(true);
                    
                }
                else
                {
                    resetScore();
                    audioPlayer.clip = clap;
                }
                animator.Play("win", 0, 0);
                audioPlayer.Play();
            }
        }
    }
    public void resetGame()
    {
        newGameButon.SetActive(false);
        resetScore();
        p1SetScore = 0;
        p2SetScore = 0;
        p1sets.text = "";
        p2sets.text = "";
    }

    public bool endGame()
    {
        if (p1SetScore == 3 || p2SetScore == 3)
        {
            return true;
        }
        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
        //animator.enabled = true;
        newGameButon.SetActive(false);
        paddle1.ControlType = (ControlType)PlayerPrefs.GetInt("Player1Controls");
        paddle2.ControlType = (ControlType)PlayerPrefs.GetInt("Player2Controls");
        paddle1.side = Side.Left;
        paddle2.side = Side.Right;
        //paddle2.ControlType = ControlType.AI;

        p1Score = 0;
        p2Score = 0;
        servingPaddle = paddle1;
        paddle1.serving = true;
        servingPlayer = 1;
    }



    // Update is called once per frame
    void Update()
    {
        //endGame();
    }
}
