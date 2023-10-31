using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public TextAsset dialogueJSON;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            if (!DialogueController.GetInstance().dialogueActive)
                DialogueController.GetInstance().StartDialogue(dialogueJSON);
        }
    }
}
