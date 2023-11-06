using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    public string dialogueText;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string choiceText;
    public DialogueNode nextNode;
    public string flagToSet; // The flag that will be set when this choice is made
    public bool flagValue; // The value to set the flag to
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    public DialogueNode startNode; // Starting point of the dialogue
    public DialogueNode alternativeStartNode; // Alternative starting point if a flag is set
    public string flagRequiredForAlternative; // The flag that triggers the alternative dialogue
}

