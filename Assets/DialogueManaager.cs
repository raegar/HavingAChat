using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text dialogueText; // Assign in the inspector
    public Button choicePrefab; // Assign a prefab with a Button component in the inspector
    public Transform choicePanel; // The parent panel for choice buttons

    private Queue<DialogueNode> nodesQueue = new Queue<DialogueNode>(); // Queue to handle the order of dialogue nodes
    private DialogueNode currentNode;

    // Call this method to start a dialogue
    public void StartDialogue(Dialogue dialogue)
    {
        nodesQueue.Clear();

        // Initialize with the default start node
        DialogueNode startNode = dialogue.startNode;

        // Loop through all alternative dialogues
        foreach (var alternative in dialogue.alternativeDialogues)
        {
            // Check if the flag for this alternative is set
            if (!string.IsNullOrEmpty(alternative.flagRequired) &&
                GameStateManager.Instance.GetFlag(alternative.flagRequired))
            {
                // If the flag is set, use the alternative start node
                startNode = alternative.alternativeStartNode;
                break; // Break if you want only the first matching alternative, otherwise remove the break to let the last matching alternative take precedence
            }
        }

        // Enqueue the determined start node
        nodesQueue.Enqueue(startNode);


        // Start the dialogue
        DisplayNextNode();
    }

    // Call this method to end the dialogue
    public void EndDialogue()
    {
        dialogueText.text = "The end of the conversation.";
        // Here, you could also trigger any events that should happen after a dialogue ends
    }

    // This method handles displaying the next node in the queue
    public void DisplayNextNode()
    {
        if (nodesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        currentNode = nodesQueue.Dequeue();
        dialogueText.text = currentNode.dialogueText;

        // First, clear the previous choices
        foreach (Transform child in choicePanel)
        {
            Destroy(child.gameObject);
        }

        // Then, create new choice buttons
        foreach (DialogueChoice choice in currentNode.choices)
        {
            Button choiceButton = Instantiate(choicePrefab, choicePanel);
            choiceButton.GetComponentInChildren<Text>().text = choice.choiceText;
            choiceButton.onClick.AddListener(delegate { MakeChoice(choice); });
        }
    }

    // This method is called when a player makes a choice
    private void MakeChoice(DialogueChoice choice)
    {
        // Example of setting a flag when a choice is made
        if (!string.IsNullOrEmpty(choice.flagToSet))
        {
            GameStateManager.Instance.SetFlag(choice.flagToSet, choice.flagValue);
        }

        // Enqueue the next node based on the player's choice
        if (choice.nextNode != null)
        {
            nodesQueue.Enqueue(choice.nextNode);
        }
        else
        {
            // If there is no next node, then the conversation ends
            EndDialogue();
            return;
        }

        // Continue to the next node
        DisplayNextNode();
    }
}
