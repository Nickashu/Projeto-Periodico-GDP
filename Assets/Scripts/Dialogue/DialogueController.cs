using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueController : MonoBehaviour {    //Esta classe será única para todo o projeto (singleton class)
    private static DialogueController instance;

    public TextAsset variablesJSON;    //Este é o arquivo JSON do ink que contém todas as variáveis de diálogo
    public GameObject ImgCharacterDialogue, DialogueBoxContainer;
    public TextMeshProUGUI txtDialogue, txtNameCharacter;
    public GameObject[] choices;
    public DialogueVariablesController dialogueVariablesController { get; private set; }

    private TextMeshProUGUI[] choicesTxt;
    private Story dialogue;

    private bool endLine = false;   //Esta variável é responsável por guardar se cada linha do diálogo já terminou ou ainda não
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

        //Ao carregar pela primeira vez, precisamos carregar as variáveis criadas no ink para o código. Faço isso chamando o próprio construtor da classe DialogueVariablesController:
        dialogueVariablesController = new DialogueVariablesController(variablesJSON);
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
        if (dialogueActive) {
            if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
                PassDialogue();
        }
        if (endLine) {
            endLine = false;
            if(dialogue.currentChoices.Count > 0)
                ShowChoices();
        }
    }

    public void StartDialogue(TextAsset dialogueJSON) {
        dialogue = new Story(dialogueJSON.text);        //Carregando o diálogo a partir do arquivo JSON passado de parâmetro
        dialogueActive = true;
        DialogueBoxContainer.SetActive(true);
        dialogueVariablesController.StartListening(dialogue);  //Para detectar as mudanças de variáveis no diálogo

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
                if (!dialogue.canContinue)     //Se estiver no final do diálogo
                    EndDialogue();
                else {
                    SoundController.GetInstance().PlaySound("skip_dialogo", null);
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

    private void EndDialogue() {   //Método chamado ao fim do diálogo
        txtDialogue.text = "";
        DialogueBoxContainer.SetActive(false);
        dialogueActive = false;
        dialogueVariablesController.StopListening(dialogue);  //Para parar de detectar as mudanças de variáveis no diálogo
        GameController.checkVariablesDialogue(dialogueVariablesController.variablesValues);    //Fazendo as checagens de variáveis importantes que podem ter mudado após um diálogo
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

        if (!dialogue.canContinue)     //Se estiver no final do diálogo
            EndDialogue();
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
                characterName = tag.Split(":")[1].Trim().ToUpper();
            else if (tag.Split(":")[0].Trim() == "state")
                spriteCharacter = tag.Split(":")[1].Trim();
        }
        if(spriteCharacter != "")
            ImgCharacterDialogue.GetComponent<Animator>().Play(spriteCharacter);
        if (characterName != "")
            txtNameCharacter.text = characterName;
    }

    public Ink.Runtime.Object GetVariableState(string variableName) {    //Esta função servirá para recuperar o estado de determinada variável de diálogo
        Ink.Runtime.Object variableValue = null;
        dialogueVariablesController.variablesValues.TryGetValue(variableName, out variableValue);
        if (variableValue == null) {
            Debug.Log("Não foi possível recuperar o valor da variável de diálogo informada.");
            return null;
        }
        return variableValue;
    }
}
