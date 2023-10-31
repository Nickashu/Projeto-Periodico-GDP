using Ink.Runtime;
using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour {    //Esta classe será única para todo o projeto (singleton class)

    private static DialogueController instance;
    public GameObject DialogueBox, DialogueBoxContainer;
    private TextMeshProUGUI txtDialogue;
    private Story dialogue;

    private float textSpeed = 0.05f;
    private int indexLine;
    public bool dialogueActive { get; private set; }   //Quero que esta variável possa ser lida por outros scripts, mas não modificada

    public static DialogueController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }

    void Start() {
        txtDialogue = DialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        DialogueBoxContainer.SetActive(false);
        dialogueActive = false;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (dialogueActive)
                PassDialogue();
        }
    }

    public void StartDialogue(TextAsset dialogueJSON) {
        dialogueActive = true;
        dialogue = new Story(dialogueJSON.text);        //Carregando o diálogo a partir do arquivo JSON passado de parâmetro
        DialogueBoxContainer.SetActive(true);

        if (dialogue.canContinue) {
            dialogue.Continue();
            StartCoroutine(PrintDialogue());
        }
    }

    private void PassDialogue() {
        string fala = dialogue.currentText;

        if (indexLine < fala.Length - 1) {         //Se não estiver no final da fala
            StopAllCoroutines();
            indexLine = fala.Length - 1;
            txtDialogue.text = fala;
        }
        else {
            if (!dialogue.canContinue) {     //Se estiver no final do diálogo
                DialogueBoxContainer.SetActive(false);
                dialogueActive = false;
            }
            else {
                dialogue.Continue();
                StartCoroutine(PrintDialogue());
            }
        }
    }

    private IEnumerator PrintDialogue() {
        string fala = dialogue.currentText;    //Pegando a fala atual do diálogo

        txtDialogue.text = "";
        for (int i = 0; i < fala.Length; i++) {    //Fazendo as letras aparecerem uma de cada vez
            txtDialogue.text += fala[i];
            indexLine = i;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
