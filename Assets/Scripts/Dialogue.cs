using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour {

    public GameObject DialogueBox, DialogueBoxContainer;
    private TextMeshProUGUI txtDialogue;
    private string[] linhas;
    private float textSpeed = 0.05f;
    private int indexDialogue, indexLine;
    private bool dialogueActive = false;

    void Start() {
        txtDialogue = DialogueBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Z)) {
            if (!dialogueActive)
                StartDialogue("teste");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (dialogueActive)
                PassDialogue();
        }
    }

    private void StartDialogue(string nomeArquivo) {
        dialogueActive = true;
        TextAsset arquivoTextAsset = Resources.Load<TextAsset>(nomeArquivo);    //Carregando o arquivo da pasta Resources
        if (arquivoTextAsset != null)
            linhas = arquivoTextAsset.text.Split("\n");
        else
            Debug.LogError("Arquivo não encontrado: " + nomeArquivo);

        DialogueBoxContainer.SetActive(true);
        indexDialogue = 0;
        StartCoroutine(PrintDialogue());
    }

    private void PassDialogue() {
        string fala = linhas[indexDialogue].Split("] -")[1];

        if (indexLine < fala.Length - 1) {         //Se não estiver no final da fala
            StopAllCoroutines();
            indexLine = linhas[indexDialogue].Length - 1;
            txtDialogue.text = fala;
        }
        else {
            if (indexDialogue >= linhas.Length - 1) {     //Se estiver no final do diálogo
                DialogueBoxContainer.SetActive(false);
                dialogueActive = false;
            }
            else {
                if (linhas[indexDialogue + 1][0] == '*')
                    indexDialogue += 2;
                else
                    indexDialogue++;
                StartCoroutine(PrintDialogue());
            }
        }
    }

    private IEnumerator PrintDialogue() {
        string fala = linhas[indexDialogue].Split("] -")[1];
        txtDialogue.text = "";
        for (int i=0; i < fala.Length; i++) {    //Fazendo as letras aparecerem uma de cada vez
            txtDialogue.text += fala[i];
            indexLine = i;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
