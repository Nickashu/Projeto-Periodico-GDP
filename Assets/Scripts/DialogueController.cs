using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour {    //Esta classe será única para todo o projeto (singleton class)

    private static DialogueController instance;

    public GameObject ImgCharacterDialogue, DialogueBoxContainer;
    public TextMeshProUGUI txtDialogue, txtNameCharacter;
    public GameObject[] choices;
    private TextMeshProUGUI[] choicesTxt;

    private Story dialogue;

    private float textSpeed = 0.05f;
    private int indexLine;
    public bool dialogueActive { get; private set; }   //Quero que esta variável possa ser lida por outros scripts, mas não modificada
    private bool endLine = false;   //Esta variável é responsável por guardar se cada linha do diálogo já terminou ou ainda não

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
        DialogueBoxContainer.SetActive(false);
        dialogueActive = false;

        choicesTxt = new TextMeshProUGUI[choices.Length];   //O array deve ter o mesmo tamanho do número de escolhas
        int index = 0;
        foreach (GameObject choice in choices) {
            choicesTxt[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (dialogueActive)
                PassDialogue();
        }
        if (endLine) {
            endLine = false;
            if(dialogue.currentChoices.Count > 0)
                ShowChoices();
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
            endLine = true;
            txtDialogue.text = fala;
        }
        else {
            if (dialogue.currentChoices.Count == 0) {
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
    }

    //Função que printa cada linha do diálogo na caixa de diálogo
    private IEnumerator PrintDialogue() {
        ChangeCharacterDialogue();
        string fala = dialogue.currentText;    //Pegando a fala atual do diálogo

        txtDialogue.text = "";
        for (int i = 0; i < fala.Length; i++) {    //Fazendo as letras aparecerem uma de cada vez
            txtDialogue.text += fala[i];
            indexLine = i;
            yield return new WaitForSeconds(textSpeed);
        }
        endLine = true;
    }


    private void ShowChoices() {    //Função para mostrar as escolhas do diálogo
        List<Choice> choicesList = dialogue.currentChoices;   //Recuperando as escolhas do diálogo

        int index = 0;
        foreach (Choice choice in choicesList) {
            choicesTxt[index].text = choice.text;
            choices[index].SetActive(true);
            index++;
        }
        for(int i=index; i<choices.Length; i++) {   //Escondendo as escolhas que não fazem parte do diálogo
            choices[i].SetActive(false);
        }
    }

    public void MakeChoice(int choiceIndex) {    //Função para fazer uma escolha no diálogo
        dialogue.ChooseChoiceIndex(choiceIndex);
        foreach (GameObject choice in choices) {
            choice.SetActive(false);
        }

        if (!dialogue.canContinue) {     //Se estiver no final do diálogo
            DialogueBoxContainer.SetActive(false);
            dialogueActive = false;
        }
        else {
            dialogue.Continue();
            StartCoroutine(PrintDialogue());
        }
    }

    private void ChangeCharacterDialogue() {   //Função para mudar o sprite do personagem do diálogo
        List<string> tagsDialogueLine = dialogue.currentTags;   //As tags são: nome do personagem e sprite do personagem
        string characterName = "", spriteCharacter = "";
        foreach(string tag in tagsDialogueLine) {
            if (tag.Split(":")[0].Trim() == "character")
                characterName = tag.Split(":")[1].Trim();
            else if (tag.Split(":")[0].Trim() == "state")
                spriteCharacter = tag.Split(":")[1].Trim();
        }
        if(spriteCharacter != "")
            ImgCharacterDialogue.GetComponent<Animator>().Play(spriteCharacter);
        if (characterName != "")
            txtNameCharacter.text = characterName;
    }
}
