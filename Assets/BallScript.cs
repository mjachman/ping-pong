using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public bool serving = true;
    public AudioSource audioPlayer;
    public AudioClip ping, pong;
    public bool play = false;
    public Vector2 init;
    public Rigidbody2D body;
    public float maxSpeed = 10;
    public TrailRenderer trailRenderer;
    public GameLogic gameLogic;
    // public string lastHit;
    public List<String> hits;
    // Start is called before the first frame update

    void Start()
    {

        hits = new List<String>();
        init = transform.position;
        serving = true;
        //trailRenderer.GetComponent<TrailRenderer>().enabled = tr;
    }
    private void FixedUpdate()
    {
        //Debug.Log(body.velocity.magnitude);
        //if (body.velocity.magnitude > maxSpeed)
        //{
        //    body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed);
        //}

    }


    //void OnMouseDrag()
    //{
    //    float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
    //    transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

    //}
    // Update is called once per frame
    void Update()
    {
        checkServeCorrectness();

        if (serving)
        {
            ResetBall();
        }

        if (hits.Count >= 3 && !play)
        {
            if (gameLogic.servingPlayer == 1)
            {
                if (body.position.x < 13 && hits[hits.Count - 1] == "Player2")
                {
                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 zablokowa? serw!");
                }
                else
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 zle zaserwowa?!");
                }
                resetRound();
            }
            else if (gameLogic.servingPlayer == -1)
            {
                if (body.position.x > -13 && hits[hits.Count - 1] == "Player1")
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 zablokowa? serw!");
                }
                else
                {
                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 zle zaserwowa?!");

                }
                resetRound();
            }
        }
        else if (hits.Count >= 2)
        {
            
            if (hits[hits.Count - 1] == hits[hits.Count - 2])
            {

                Debug.Log(hits[hits.Count - 1]);
                checkPoints(hits[hits.Count - 1]);
                resetRound();
                //Invoke("Serving", 1);
            }
            else if (play)
            {
                if (hits[hits.Count - 2] == "Player1" && hits[hits.Count - 1] == "Left")
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 odbil swoja polowe!");
                    resetRound();
                }
                else if (hits[hits.Count - 2] == "Player2" && hits[hits.Count - 1] == "Right")
                {
                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 odbil na swoja polowe!");
                    resetRound();
                }
                else if (hits[hits.Count - 2] == "Player1" && hits[hits.Count - 1] == "Player2")
                {
                    if (body.position.x > 14)
                    {
                        gameLogic.addScore(1);
                        Debug.Log("FAUL! Gracz 1 nie trafi? w strone przeciwnika!");
                    }
                    else
                    {
                        gameLogic.addScore(0);
                        Debug.Log("FAUL! Gracz 2 zablokowa? pilkie!");
                    }
                    resetRound();
                }
                else if (hits[hits.Count - 2] == "Player2" && hits[hits.Count - 1] == "Player1")
                {
                    if (body.position.x < -14)
                    {
                        gameLogic.addScore(0);
                        Debug.Log("FAUL! Gracz 2 nie trafi? w strone przeciwnika!");
                    }
                    else
                    {
                        gameLogic.addScore(1);
                        Debug.Log("FAUL! Gracz 1 zablokowa? pilkie!");
                    }
                    resetRound();
                }

            }

        }
        if (transform.position.y < -13)
        {
            if (!play)
            {
                if (gameLogic.servingPlayer == 1)
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 zle zaserwowal");
                }
                else if (gameLogic.servingPlayer == -1)
                {

                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 zle zaserwowal");
                }
            }
            else if (hits.Count >= 1)
            {
                if (hits[hits.Count - 1] == "Player1")
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 nie trafil w sto?");
                }
                if (hits[hits.Count - 1] == "Player2")
                {
                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 nie trafil w stol");
                }
                if (hits[hits.Count - 1] == "Left")
                {
                    gameLogic.addScore(1);
                    Debug.Log("FAUL! Gracz 1 nie odebral pi?eczki");
                }
                if (hits[hits.Count - 1] == "Right")
                {
                    gameLogic.addScore(0);
                    Debug.Log("FAUL! Gracz 2 nie odebral pi?eczki");
                }
            }
            resetRound();

        }

    }
    private void checkPoints(String hit)
    {
        if (hit == "Left")
        {
            gameLogic.addScore(1);
        }
        if (hit == "Right")
        {
            gameLogic.addScore(0);
        }
        if (hit == "Player1")
        {
            gameLogic.addScore(1);
        }
        if (hit == "Player2")
        {
            gameLogic.addScore(0);
        }

    }
    private void resetRound()
    {
        //Debug.Log("FAUL!");
        //addPoints();
        hits.Clear();
        gameLogic.changeServer();
        serving = true;
        play = false;
    }
    void Serving()
    {
        serving = true;
    }
    private void checkServeCorrectness()
    {
        //if (hits.Count < 2)
        //{
        //    if (gameLogic.servingPlayer == 1)

        //        gameLogic.addScore(1);

        //    else gameLogic.addScore(0);
        //}
        //else

        if (hits.Count >= 3)
        {
            if (hits[0] == "Player1" && hits[1] == "Left" && hits[2] == "Right")
            {
                play = true;

                //gameLogic.addScore(0);
            }
            if (hits[0] == "Player2" && hits[1] == "Right" && hits[2] == "Left")
            {
                play = true;
                //gameLogic.addScore(1);
            }
            //else
            //gameLogic.addScore(1);

        }
        //else
        //    gameLogic.addScore(1);

    }

  
    private void ResetBall(){
        GetComponent<Collider2D>().enabled = false;
        trailRenderer.GetComponent<TrailRenderer>().enabled = false;
        trailRenderer.GetComponent<TrailRenderer>().Clear();
        transform.position = gameLogic.servePosition;
        body.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void ServeBall()
    {
        serving = false;
        //body.velocity = Vector2.zero;
        //Physics.IgnoreCollision(GetComponent<Collider>(), other, false);
            GetComponent<Collider2D>().enabled = true;
            body.constraints = RigidbodyConstraints2D.None;
            body.velocity = Vector2.up * gameLogic.serveSpeed;
            //trailRenderer.GetComponent<TrailRenderer>().Clear();
            trailRenderer.GetComponent<TrailRenderer>().enabled = true;
            
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            audioPlayer.clip = ping;
            audioPlayer.Play();

        }
        if (collision.gameObject.tag == "Table")
        {
            audioPlayer.clip = pong;
            audioPlayer.Play();
            //hits.Add(collision.gameObject.name);
        }
        hits.Add(collision.gameObject.name);
        //Debug.Log(hits);
        string result = string.Join(", ", hits);
        Debug.Log(result);


    }


}

