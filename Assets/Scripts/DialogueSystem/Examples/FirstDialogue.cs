using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{

    DialogueLine dialogueLineSO;
    DialogueReader dialogueReader;
    DialogueUiManager dialogueUiManager;
    private TaskCompletionSource<bool> dialogueAdvanceTCS;
    private CancellationTokenSource dialogueSkipCTS;
    public event Action OnDialogueStart;
    public event Action OnDialogueEnd;
    public event Action<byte> OnChoiceSelected;
    public GameObject dialogueCanvas;
    public TMP_Text dialogeTextUi;
    public TMP_Text[] choicesUiText;
    
    // Choice navigator
    private ChoiceNavigator choiceNavigator;

    void Start()
    {

        InitializeClassesAndGameObject();
        InitializeEventRelations();
        

    }

    private void HandleDialogueAdvance()
    {
        if(dialogueSkipCTS != null)
         dialogueSkipCTS?.Cancel();

        if (dialogueAdvanceTCS != null)
        {
            dialogueAdvanceTCS.TrySetResult(true);
        }
    }

    void InitializeClassesAndGameObject()
    {
        MultiWayDialogue multiWayDialogue = new("Choose from", CreateChoicesTxtList("1", "2"), new OneWayDialogue("Hello this is finale") , new OneWayDialogue("Choice 2"));

        Debug.Log(multiWayDialogue.choices == null ? "Choices null" : "Not null choices");

        dialogueLineSO = new OneWayDialogue("Come on What Do you do when you are bored zopry , fobry , lobry 7obry? -1-24-12-4124",multiWayDialogue);
       
        
        dialogeTextUi = GameObject.FindGameObjectWithTag("DialogueUiText").GetComponent<TMP_Text>();

        /////////////////////

        dialogueUiManager = new(dialogueCanvas, dialogeTextUi, choicesUiText);
        dialogueReader = new(0.01f);

    }
    
    void InitializeEventRelations()
    {
        InputSubscriber.Instance.SubscribeToVoidInputEvent(InputType.DialogueNext, HandleDialogueAdvance);        
        OnDialogueEnd += () => dialogueUiManager.ActivateOrDisableMainDialogue(false);
        OnDialogueStart += () => dialogueUiManager.ActivateOrDisableMainDialogue(true);
        OnChoiceSelected += dialogueUiManager.HightlightChoice;
        dialogueReader.OnReadingChar += dialogueUiManager.OnRecievingChar;
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            _ = ControlDialogueFlow();
        }
        
        // Handle choice navigation with arrow keys and confirm with Enter
        if (choiceNavigator != null && choiceNavigator.IsAwaitingChoice)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow))
                choiceNavigator.HandleUpInput();
            else if (Input.GetKeyDown(KeyCode.DownArrow))
                choiceNavigator.HandleDownInput();
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
                choiceNavigator.HandleLeftInput();
            else if (Input.GetKeyDown(KeyCode.RightArrow))
                choiceNavigator.HandleRightInput();
            else if (Input.GetKeyDown(KeyCode.Return))
                choiceNavigator.HandleConfirmInput();
        }
    }
    async Task ControlDialogueFlow()
    {
        Debug.Log("--- FLOW START: Entering ControlDialogueFlow ---");
        OnDialogueStart?.Invoke();

        while (dialogueLineSO != null)
        {
            Debug.Log($"LOOP START: Reading Dialogue Line: {dialogueLineSO.text.Substring(0, Math.Min(30, dialogueLineSO.text.Length))}...");

            // 1. Setup and Execute Read/Skip
            using (CancellationTokenSource localCTS = new CancellationTokenSource()) 
            {
                dialogueSkipCTS = localCTS; // Share the reference for input events
                
                Debug.Log("STATE: Awaiting ReadDialogue (Typing/Skip enabled)...");
                await dialogueReader.ReadDialogue(dialogueLineSO, localCTS.Token);
                Debug.Log("STATE: ReadDialogue Await Finished.");
                
            } // localCTS is Disposed here.
            
            // CRITICAL STEP: Set the shared field to null immediately after disposal
            dialogueSkipCTS = null;


            // 2. Introduce the Cooldown/Anti-Spam Delay
            Debug.Log("STATE: Awaiting Anti-Spam Cooldown (1.5s)...");
            await Awaitable.WaitForSecondsAsync(1.5f); 
            Debug.Log("STATE: Cooldown Finished.");
            
            // 3. Wait for the NEXT explicit Advance Input

            dialogueAdvanceTCS = null;            

            // 4. Determine Next Line
            if (dialogueLineSO is MultiWayDialogue multiWayDialogue)
            {
                Debug.Log("BRANCH: MultiWay Dialogue Detected.");

                // Read out the choices' text
                Debug.Log("STATE: Reading out Choice Options...");
                await ReadChoices(multiWayDialogue);
                Debug.Log("STATE: Choice Options Read Finished.");

                // Initialize choice navigator and wait for player to make a selection
                Debug.Log("STATE: Awaiting Player Choice Selection...");
                choiceNavigator = new ChoiceNavigator();
                choiceNavigator.Initialize(multiWayDialogue.choices.Length);
                
                // Wire the navigator's highlight event to the UI
                choiceNavigator.OnChoiceHighlighted += (choiceIndex) =>
                {
                    OnChoiceSelected?.Invoke(choiceIndex);
                };

                Debug.Log("[CHOICE] Awaiting navigation (Arrows to move, Enter to confirm)...");
                
                // Wait for player to confirm their choice
                byte selectedChoice = await choiceNavigator.GetChoiceAsync();
                
                // Advance to the next dialogue based on the selected choice
                dialogueLineSO = multiWayDialogue.GetNext(selectedChoice);
                Debug.Log($"[CHOICE] Advancing to next dialogue after choice {selectedChoice}");
                Debug.Log("STATE: Player Choice Selection Received.");
            }
            else
            {
                dialogueAdvanceTCS = new TaskCompletionSource<bool>();
            
                Debug.Log("STATE: Awaiting Player Advance Input...");
                await dialogueAdvanceTCS.Task; 
                Debug.Log("STATE: Player Advance Input Received.");
                // Advance to the next linear line
                dialogueLineSO = dialogueLineSO.GetNext();             
                Debug.Log("BRANCH: Linear Dialogue Advanced.");
            }
        
            await Awaitable.EndOfFrameAsync();
        }
        
        Debug.Log("--- FLOW END: Dialogue Chain Ended (dialogueLineSO is null) ---");
        OnDialogueEnd?.Invoke();
    }
    
   async Task ReadChoices(MultiWayDialogue multiWayDialogue)
    {
        string[] strings = multiWayDialogue.GetChoicesText();
        Debug.Log($"STATE: Starting to read {strings.Length} choice options.");
        
        for (int i = 0; i < strings.Length; i++)
        {
            Debug.Log($"LOOP: Reading Choice {i}: {strings[i]}");
            await dialogueReader.ReadChoice(1f, strings[i], (byte)i);
        }
        Debug.Log("STATE: All choices have been typed out.");
    }

    public static string[] CreateChoicesTxtList(params string[] choicestxt)
    {
        return choicestxt;
    }

}

