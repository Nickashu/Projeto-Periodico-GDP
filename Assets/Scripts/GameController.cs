using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject canvasLives;

    public static List<string> tagsInteractable = new List<string>() { "NPC" };    //Esta lista armazena todas as tags de objetos possíveis de se intergair no jogo
    public static List<string> tagsFoods = new List<string>() { "Donut", "Espeto", "Pipoca", "Racao", "Salsichao", "Sorvete" };    //Esta lista armazena todas as tags de comida
    public static Dictionary<int, string> dicIdFood = new Dictionary<int, string> { { 1, "Donut" }, { 2, "Espeto" }, { 3, "Pipoca" }, { 4, "Racao" }, { 5, "Salsichao" }, { 6, "Sorvete" } };

    public static bool terminouDeFalarCorvo1 = false;    //Aqui ficarão algumas variáveis de diálogo que terão efeitos no jogo
    public static bool acabouTutorial = false, gamePaused = false;
    public static float comecoMapaX=13, comecoMapaY=-20;

    public static void checkVariablesDialogue(Dictionary<string, Ink.Runtime.Object> dictionaryVariables) {    //Este método será responsável por checar algumas variáveis de diálogos que terão efeito no jogo
        foreach (KeyValuePair<string, Ink.Runtime.Object> pair in dictionaryVariables) {
            string chave = pair.Key;

            //Aqui eu faço a verificação manual de determinadas variáveis importantes:
            if (chave == "terminouDeFalarCorvo1" && !terminouDeFalarCorvo1) {
                bool value = ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState(chave)).value;
                if (value)
                    terminouDeFalarCorvo1 = true;
            }
        }
    }

    private void Update() {
        if (acabouTutorial && !canvasLives.activeSelf) {
            canvasLives.SetActive(true);
        }
    }
}
