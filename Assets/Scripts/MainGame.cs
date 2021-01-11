using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainGame : MonoBehaviour
{
    [Serializable]
    class CustomEndGameException : Exception
    {
        string Message = "STOP RIGHT THERE, CRIMINAL SCUM!\n" +
            "YOU VIOLATED THE LAW! PAY THE COURT A FINE OR SERVE YOUR SENTENCE!\n" +
            "YOUR STOLEN GOODS ARE NOW FORFEIT!\n";
        public CustomEndGameException()
        {
            Console.Write(Message);
        }

        public CustomEndGameException(string message)
            : base(String.Format("{0}", message))
        {

        }

    }



    [SerializeField] Text textElement;
    [SerializeField] Text leftOption;
    [SerializeField] Text rightOption;
    [SerializeField] Text upOption;
    [SerializeField] Text downOption;
    [SerializeField] Text spaceOption;
    [SerializeField] Text timeCounter;
    [SerializeField] State startState;
    State prePauseState;
    State state;
    //for inventory management
    int inventoryCapacity;
    int currentInventory;

    //for risk management with winning / losing
    int totalActionsTaken;
    int totalActionLimit;
    const string GAME_WIN_TEXT =
        "I survived. It was a tough night, but I made enough good decisions that I " +
        "survived. Maybe now I can see whats hapening in the rest of the world. Are there other" +
        " survivors, or am I the only one...?";

    bool hasMatches;
    bool hasGun;
    bool hasLantern;
    bool hasKnife;
    bool hasGear;
    void Start()
    {
        state = startState;
        prePauseState = state;
        textElement.text = state.GetStateText();

        totalActionLimit = 60;
        totalActionsTaken = 0;
        inventoryCapacity = 2;
        currentInventory = 0;

        hasMatches = false;
        hasGun = false;
        hasLantern = false;
        hasKnife = false;
        hasGear = false;
    }

    // Update is called once per frame
    void Update()
    {
        GameMain();
    }

    //respond to user input / key presses
    private void GameMain()
    {
        State[] statesArray = state.GetOtherStates();
        leftOption.text = statesArray.Length >= 1 ? statesArray[0].GetNameValue() : "";
        rightOption.text = statesArray.Length > 2 ? statesArray[1].GetNameValue() : "";
        upOption.text = statesArray.Length > 4 ? statesArray[2].GetNameValue() : "";
        downOption.text = statesArray.Length > 5 ? statesArray[3].GetNameValue() : "";
        spaceOption.text = statesArray.Length > 3 ? statesArray[statesArray.Length - 2].GetNameValue() : "";


        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
                {
                    state = statesArray[0];
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
                {
                    if (statesArray.Length > 2)
                    {
                        state = statesArray[1];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                {
                    if (statesArray.Length > 4)
                    {
                        state = statesArray[2];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                {
                    if (statesArray.Length > 5)
                    {
                        state = statesArray[3];
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (statesArray.Length > 2)
                    {
                        state = statesArray[statesArray.Length - 2];
                    }

                }

                if (state == startState)
                {
                    Start();
                }

                prePauseState = state;

                textElement.text = state.GetStateText();

                // decision counter
                totalActionsTaken += state.GetActionValue();
                //Game Win
                if (totalActionsTaken >= totalActionLimit)
                {
                    totalActionsTaken = 60;
                    statesArray = EndGame(GAME_WIN_TEXT);
                }
                timeCounter.text = (totalActionLimit - totalActionsTaken).ToString();

                try
                {
                    TriggerInventory();
                }
                catch (Exception e)
                {
                    statesArray = EndGame(e.Message);
                }
            }
            catch (IndexOutOfRangeException e)
            {
                state = prePauseState;
                textElement.text = state.GetStateText();
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                state = prePauseState;
                textElement.text = state.GetStateText();
                Console.WriteLine(e.Message);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            //This actually works but //overload comparison operators for states here
            if (state != prePauseState)
            {
                state = prePauseState;
                textElement.text = state.GetStateText();
            }
            else
            {
                state = statesArray[statesArray.Length - 1];
                textElement.text = state.GetStateText();
                //statesArray[0] = prePauseState;
            }
        }
    }

    private void TriggerInventory()
    {
        currentInventory += state.GetItemAddValue();

        if (currentInventory > inventoryCapacity)
        {
            //currentInventory--;
            //KYS because youre greedy ending
            throw new CustomEndGameException("I grab it but my arms are full. I'm having trouble carrying " +
                "everything. I remember my training and begin to juggle. As I begin to walk, I think about" +
                " how my life would have been if I had only finished clown school. I lose my balance as the " +
                "knife flies through the air. I trip and break my nose on the hard wood floor. The knife falls" +
                " blade first into the back of my skull.");
        }

        if (state.TriggerGun() || state.TriggerKnife() || state.TriggerLantern())
        { 
            if (hasGun && state.TriggerGun())
            {
                //shooter ending
                throw new CustomEndGameException("I pull the trigger. Easier than I thought. This is manageable." +
                    " I think I should be able to defend myself until morning. I hear the hordes approaching. " +
                    "Lock and Load...");
            }
            else if (state.TriggerGun())
            {
                //get gun
                hasGun = true;
            }
            if (hasLantern && state.TriggerLantern())
            {
                if (hasMatches && state.TriggerMatches())
                {
                    //light the lamp
                    hasMatches = false;
                }
                else if (state.TriggerMatches())
                {
                    //Youre the real match ending
                    throw new CustomEndGameException("I reach into my pocket. There are no matches." +
                        "I pause for a moment and think. I sit and meditate in the darkness. As I reach" +
                        " enlightenment, I feel a fire burning inside. I concentrate. Flames come forth from " +
                        "my eyes. I am the match. I channel my energy. I am consumed in flame...");
                }
            }
            else if (state.TriggerLantern())
            {
                //Become the light
                throw new CustomEndGameException("I have nothing to light the lantern with. My " +
                    "brain freezes. I sit in the darkness wondering what the point is. My sense " +
                    "of self worth has completely crubmbled. There's nothing left to live for. I throw myself " +
                    "from the top of the stairs...");
            }
            if (hasGear && state.TriggerKnife())
            {
                //chainsaw badassery
                throw new CustomEndGameException("I rev up the chainsaw and turn it towards to hoarde. Hail to" +
                    " the King, baby. I jump into the crowd slicing as much as I can. I spin and slash like a " +
                    "tornado of death. At the end of the crowd, I cut the head clean off of my last enemy. The hunted" +
                    " has become the hunter...");
            }
            else if (hasKnife && state.TriggerKnife())
            {
                //fight off monsters with knife
                throw new CustomEndGameException("I thrust my knife deep into the belly of the beast. It screams " +
                    "as I twist and pull. I stab again and again. The monster slumps to the ground with a large" +
                    "THUD. The scream surely attracted more. I have to go...");
            }
            else if (state.TriggerKnife())
            {
                //You have no weapons to use this way
                throw new CustomEndGameException();
            }
        }
        if (state.TriggerMatches() || state.TriggerGear())
        {
            if (hasMatches && state.TriggerMatches())
            {
                //light match
                hasMatches = false;
            }
            else if (state.TriggerMatches())
            {
                //get match
                hasMatches = true;
            }

            if (hasGear && state.TriggerGear())
            {
                //you wore hockey pads like batman and won
                throw new CustomEndGameException("");
            }
            else if (state.TriggerGear())
            {
                //gear get
                hasGear = true;
            }
        }
    }

    private State[] EndGame(string endText)
    {
        State[] restart = new State[] { startState };
        leftOption.text = restart[0].name;
        rightOption.text = "";
        upOption.text = "";
        downOption.text = "";
        spaceOption.text = "";

        textElement.text = endText;

        return restart;
    }
}
