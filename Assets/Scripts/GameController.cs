using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour {
    public GameObject canvasLives, canvasTimer, canvasLionBar;

    public static List<string> tagsInteractable = new List<string>() { "NPC" };    //Esta lista armazena todas as tags de objetos possíveis de se intergair no jogo
    public static List<string> tagsFoods = new List<string>() { "Donut", "Espeto", "Pipoca", "Racao", "Salsichao", "Sorvete" };    //Esta lista armazena todas as tags de comida
    public static Dictionary<int, string> dicIdFood = new Dictionary<int, string> { { 1, "Donut" }, { 2, "Espeto" }, { 3, "Pipoca" }, { 4, "Racao" }, { 5, "Salsichao" }, { 6, "Sorvete" } };
    public static Dictionary<int, int> dicIdFoodPoints = new Dictionary<int, int> { { 1, -5 }, { 2, 20 }, { 3, 1 }, { 4, 5 }, { 5, 15 }, { 6, -1 } };
    //Criando uma lista para identificar os ids das comidas que não são carne:
    public static List<int> idsNotMeat = new List<int>() { 1, 3, 4, 6 };

    public static bool terminouDeFalarCorvo1 = false, falouLeao1=false;    //Aqui ficarão algumas variáveis de diálogo que terão efeitos no jogo
    public static bool acabouTutorial = false, gamePaused = false, changingLionBar = false, playerCaught = false, beginTimer=false;
    public static float comecoMapaX=0, comecoMapaY=0;
    public static int idComidaLeao = 0, idEnding = -1;

    private float timerGame = 10f;   //Aqui está o tempo do timer em segundos
    private int timerSeconds, timerMinutes;
    private static int numComidasLeao = 0;
    private static bool completedLionBar = false;

    public enum FinaisJogo {
        FinalBom = 0,
        FinalRuim = 1,
        GameOver = 2
    }

    public static void checkVariablesDialogue(Dictionary<string, Ink.Runtime.Object> dictionaryVariables) {    //Este método será responsável por checar algumas variáveis de diálogos que terão efeito no jogo
        foreach (KeyValuePair<string, Ink.Runtime.Object> pair in dictionaryVariables) {
            string chave = pair.Key;

            //Aqui eu faço a verificação manual de determinadas variáveis importantes:
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
                    Debug.Log("Deu comida pro leão! Barra será afetada");
                    changingLionBar = true;
                }
            }

            if (chave == "leaoSatisfeito") {
                bool value = ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState(chave)).value;
                if (value)   //Se conseguimos encher a barra de fome do leão e terminamos o último diálogo
                    completedLionBar = true;
                
            }
        }
    }

    private void Update() {
        if (acabouTutorial && !canvasLives.activeSelf)
            canvasLives.SetActive(true);

        if (falouLeao1 && !beginTimer) {    //Depois de falar com o leão, um timer aparece no jogo
            canvasTimer.SetActive(true);
            canvasLionBar.SetActive(true);
            beginTimer = true;
            SoundController.GetInstance().PlaySound("OST_trilha1_timer", null);
        }

        if (beginTimer && !gameIsPaused()) {
            if (timerGame <= 0) {    //Se o tempo acabar
                gamePaused = true;
                beginTimer = false;
                timerGame = 0;
                updateTimer();
                StartCoroutine(endGame(true));
            }
            else
                updateTimer();   //Atualizando o timer
        }

        if (changingLionBar)
            canvasLionBar.GetComponent<Lion_Bar>().changeBar(dicIdFoodPoints[idComidaLeao]);   //Fazendo a barra de comida do leão variar

        if (completedLionBar) {   //Se conseguimos encher a barra de fome do leão
            completedLionBar = false;
            gamePaused = true;
            StartCoroutine(endGame(false));
        }


        if (Input.GetKeyDown(KeyCode.Escape))   //Apertar esc para sair do jogo
            Application.Quit();
    }

    private IEnumerator endGame(bool timeOver) {    //Esta co-rotina será chamada quando terminamos o jogo
        float timeWait = 2f;
        yield return new WaitForSeconds(timeWait);
        if (timeOver) {
            SoundController.GetInstance().PlaySound("palhaco_rindo", null);
            yield return new WaitForSeconds(timeWait);
            TransitionsController.GetInstance().LoadLastScene((int)FinaisJogo.FinalRuim);
        }
        else {
            TransitionsController.GetInstance().LoadLastScene((int)FinaisJogo.FinalBom);
        }
    }

    private void updateTimer() {
        timerGame -= Time.deltaTime;
        timerSeconds = Mathf.FloorToInt(timerGame % 60);
        timerMinutes = Mathf.FloorToInt(timerGame / 60);
        if(timerGame > 0)
            canvasTimer.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", timerMinutes, timerSeconds);   //Mostrando os minutos e segundos formatados
        else
            canvasTimer.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}:{1:00}", 0, 0);
    }

    public static bool gameIsPaused() {    //Função usada para verificar quando o jogo está pausado
        if (gamePaused || DialogueController.GetInstance().dialogueActive)
            return true;
        return false;
    }

    public static void resetGame() {   //Método para resetar todas as variáveis importantes para o jogo
        terminouDeFalarCorvo1 = false;
        falouLeao1 = false;
        beginTimer = false;
        acabouTutorial = false;
        gamePaused = false;
        changingLionBar = false;
        playerCaught = false;
        idComidaLeao = 0;
        idEnding = -1;
        numComidasLeao = 0;
        completedLionBar = false;
        DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("resetVariables", null);
    }
}
