using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject canvasLives, canvasTimer, canvasLionBar;

    public static List<string> tagsInteractable = new List<string>() { "NPC" };    //Esta lista armazena todas as tags de objetos poss�veis de se intergair no jogo
    public static List<string> tagsFoods = new List<string>() { "Donut", "Espeto", "Pipoca", "Racao", "Salsichao", "Sorvete" };    //Esta lista armazena todas as tags de comida
    public static Dictionary<int, string> dicIdFood = new Dictionary<int, string> { { 1, "Donut" }, { 2, "Espeto" }, { 3, "Pipoca" }, { 4, "Racao" }, { 5, "Salsichao" }, { 6, "Sorvete" } };
    public static Dictionary<int, int> dicIdFoodPoints = new Dictionary<int, int> { { 1, -5 }, { 2, 20 }, { 3, 1 }, { 4, 5 }, { 5, 15 }, { 6, -1 } };
    //Criando uma lista para identificar os ids das comidas que n�o s�o carne:
    public static List<int> idsNotMeat = new List<int>() { 1, 3, 4, 6 };

    public static bool terminouDeFalarCorvo1 = false, falouLeao1=false;    //Aqui ficar�o algumas vari�veis de di�logo que ter�o efeitos no jogo
    public static bool acabouTutorial = false, gamePaused = false, changingLionBar = false, playerCaught = false;
    public static float comecoMapaX=0, comecoMapaY=0;
    public static int idComidaLeao = 0;

    private bool beginTimer = false;
    private float timerGame=300f;   //Aqui est� o tempo do timer em segundos
    private int timerSeconds, timerMinutes;
    private static int numComidasLeao = 0;

    public static void checkVariablesDialogue(Dictionary<string, Ink.Runtime.Object> dictionaryVariables) {    //Este m�todo ser� respons�vel por checar algumas vari�veis de di�logos que ter�o efeito no jogo
        foreach (KeyValuePair<string, Ink.Runtime.Object> pair in dictionaryVariables) {
            string chave = pair.Key;

            //Aqui eu fa�o a verifica��o manual de determinadas vari�veis importantes:
            if (chave == "terminouDeFalarCorvo1" && !terminouDeFalarCorvo1) {
                bool value = ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState(chave)).value;
                if (value)
                    terminouDeFalarCorvo1 = true;
            }
            if (chave == "falouLeao1" && !falouLeao1) {
                bool value = ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState(chave)).value;
                if (value)
                    falouLeao1 = true;
            }

            if (chave == "numComidasLeao") {
                int value = ((Ink.Runtime.IntValue)DialogueController.GetInstance().GetVariableState(chave)).value;
                if (value != numComidasLeao) {
                    numComidasLeao = value;
                    Debug.Log("Deu comida pro le�o! Barra ser� afetada");
                    changingLionBar = true;
                }
            }
        }
    }

    private void Update() {
        if (acabouTutorial && !canvasLives.activeSelf)
            canvasLives.SetActive(true);

        if (falouLeao1 && !beginTimer) {    //Depois de falar com o le�o, um timer aparece no jogo
            canvasTimer.SetActive(true);
            canvasLionBar.SetActive(true);
            beginTimer = true;
        }

        if (beginTimer && !gameIsPaused())
            updateTimer();   //Atualizando o timer

        if (changingLionBar)
            canvasLionBar.GetComponent<Lion_Bar>().changeBar(dicIdFoodPoints[idComidaLeao]);   //Fazendo a barra de comida do le�o variar

        if (Input.GetKeyDown(KeyCode.Escape))   //Apertar esc para sair do jogo
            Application.Quit();
    }

    private void updateTimer() {
        timerGame -= Time.deltaTime;
        timerSeconds = Mathf.FloorToInt(timerGame % 60);
        timerMinutes = Mathf.FloorToInt(timerGame / 60);
        canvasTimer.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", timerMinutes, timerSeconds);   //Mostrando os minutos e segundos formatados
    }

    public static bool gameIsPaused() {    //Fun��o usada para verificar quando o jogo est� pausado
        if (gamePaused || DialogueController.GetInstance().dialogueActive)
            return true;
        return false;
    }
}
