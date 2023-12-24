using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionsController : MonoBehaviour {
    private static TransitionsController instance;
    private Animator animTransitionScenes, animTransitionCutscenes;
    private float transistionTimeCutscenes = 1f, transistionTimeScenes = 2f;
    private int idCutscene = 0;
    private bool isLoadingCutscene=false;

    public List<GameObject> cutscenes;
    public GameObject endings;

    public static TransitionsController GetInstance() {
        return instance;
    }

    private void Start() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        transform.GetChild(2).gameObject.SetActive(false);   //Este ser� o objeto respons�vel pela transi��o que ocorre quando o personagem � capturado no jogo

        if (!SceneManager.GetActiveScene().name.Contains("Principal")) {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            if (SceneManager.GetActiveScene().name.Contains("Final")) {  //Se tivermos ido para a cena final
                GameObject objTelaFinal = endings.transform.GetChild(GameController.idEnding).gameObject;
                objTelaFinal.SetActive(true);   //Ativando a tela final respectiva
                if (GameController.idEnding != (int)GameController.FinaisJogo.GameOver) {
                    for (int i = 0; i < objTelaFinal.transform.childCount; i++) {
                        cutscenes.Add(objTelaFinal.transform.GetChild(i).gameObject);
                    }
                }

                GameController.resetGame();
            }
            else if (SceneManager.GetActiveScene().name.Contains("Menu")) {
                if (SoundController.GetInstance().numTimesMenu != 0) {
                    GameController.resetGame();
                    Debug.Log("resetou vari�veis!");
                }
                SoundController.GetInstance().numTimesMenu++;
            }

            if (cutscenes.Count > 0)   //Se tiverem cutscenes na cena
                cutscenes[idCutscene].SetActive(true);
            animTransitionScenes = transform.GetChild(0).GetComponent<Animator>();
            animTransitionCutscenes = transform.GetChild(1).GetComponent<Animator>();
        }
        else {
            GameController.idEnding = -1;   //A cada vez que o jogo come�a, o idEnding � resetado
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);   //Desativando a transi��o de cutscenes
            animTransitionScenes = transform.GetChild(0).GetComponent<Animator>();
        }

        playSceneMusic();   //Toca a m�sica correspondente ao mudar de cena
    }

    private void playSceneMusic() {
        if (SceneManager.GetActiveScene().name.Contains("Menu")) {
            SoundController.GetInstance().PlaySound("OST_menu", null);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Inicial")) {
            SoundController.GetInstance().PlaySound("OST_safe", null);
        }
        else if (SceneManager.GetActiveScene().name.Contains("Final")) {
            SoundController.GetInstance().PlaySound("OST_menu", null);
        }
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name.Contains("Inicial") || SceneManager.GetActiveScene().name.Contains("Final")) {
            if (SceneManager.GetActiveScene().name.Contains("Final")) {
                if (GameController.idEnding != (int)GameController.FinaisJogo.GameOver) {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                        LoadNextCutscene();
                }
            }
            else {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                    LoadNextCutscene();
            }
        }
    }

    public void LoadNextCutscene() {
        if(idCutscene == cutscenes.Count - 1) {
            Debug.Log("Acabou cutscenes!");
            LoadNextScene();
        }
        else {
            if(!isLoadingCutscene)
                StartCoroutine(LoadCutscene(idCutscene + 1));   //Carregando a pr�xima cutscene
        }
    }

    public void LoadNextScene() {
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1) {    //Se estivermos na �ltima cena
            //Debug.Log("Jogo terminado!");
            StartCoroutine(LoadScene(0));   //Carregando a primeira cena novamente
        }
        else
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));   //Carregando a pr�xima cena
    }

    public void LoadLastScene(int idEnding) {   //0 - Final Bom; 1 - Final Ruim; 2 - GameOver
        GameController.idEnding = idEnding;
        LoadNextScene();
    }

    public void LoadMainScene() {
        StartCoroutine(LoadScene(1));   //Carregando a cena principal
    }
    public void LoadMenu() {
        StartCoroutine(LoadScene(0));   //Carregando o menu
        //GameController.resetGame();
    }


    public void TransitionAfterCaught() {
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(true);
    }

    private IEnumerator LoadScene(int sceneIndex) {
        animTransitionScenes.SetBool("StartFade", true);
        yield return new WaitForSeconds(transistionTimeScenes);
        SceneManager.LoadScene(sceneIndex);
    }
    private IEnumerator LoadCutscene(int newIdCutscene) {
        isLoadingCutscene = true;
        animTransitionCutscenes.SetBool("Pass", true);
        yield return new WaitForSeconds(transistionTimeCutscenes);
        cutscenes[idCutscene].SetActive(false);
        cutscenes[newIdCutscene].SetActive(true);
        yield return new WaitForSeconds(transistionTimeCutscenes);
        animTransitionCutscenes.SetBool("Pass", false);
        idCutscene++;
        isLoadingCutscene = false;
    }
}
