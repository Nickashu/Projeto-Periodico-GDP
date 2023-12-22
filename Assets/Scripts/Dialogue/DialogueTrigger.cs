using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public TextAsset dialogueJSON;

    public void TriggerDialogue() {
        if (!DialogueController.GetInstance().dialogueActive)
            DialogueController.GetInstance().StartDialogue(dialogueJSON);
    }
}
