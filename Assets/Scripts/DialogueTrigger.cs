using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

    public TextAsset inkJSON;

    void Update() {
        if(Input.GetKeyDown(KeyCode.Z) && !DialogueController.GetInstance().dialogueActive) {
            if (!DialogueController.GetInstance().dialogueActive)
                DialogueController.GetInstance().StartDialogue(inkJSON);
        }
    }
}
