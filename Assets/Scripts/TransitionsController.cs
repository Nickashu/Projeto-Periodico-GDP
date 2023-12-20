using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionsController : MonoBehaviour {
    private static TransitionsController instance;
    private Animator animTransitionScenes, animTransitionCutscenes, animTransitionGame;
    private float transistionTimeCutscenes = 1f, transistionTimeScenes = 2f;
    private int idCutscene = 0;
    private bool isLoadingCutscene=false;

    public GameObject[] cutscenes;

    public static TransitionsController GetInstance() {
        return instance;
    }

    private void Start() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        //DontDestroyOnLoad(gameObject);



        if(!SceneManager.GetActiveScene().name.Contains("Principal")) {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);
            //transform.GetChild(2).gameObject.SetActive(false);   //Desativando a transição do game
            if (cutscenes.Length > 0)   //Se tiverem cutscenes na cena
                cutscenes[idCutscene].SetActive(true);
            animTransitionScenes = transform.GetChild(0).GetComponent<Animator>();
            animTransitionCutscenes = transform.GetChild(1).GetComponent<Animator>();
        }
        else {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);   //Desativando a transição de cutscenes
            //transform.GetChild(2).gameObject.SetActive(true);
            animTransitionScenes = transform.GetChild(0).GetComponent<Animator>();
            //animTransitionGame = transform.GetChild(2).GetComponent<Animator>();
        }
    }

    private void Update() {
        if(SceneManager.GetActiveScene().name.Contains("Inicial") || SceneManager.GetActiveScene().name.Contains("Final")) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
                LoadNextCutscene();
        }
    }

    public void LoadNextCutscene() {
        if(idCutscene == cutscenes.Length - 1) {
            Debug.Log("Acabou cutscenes!");
            LoadNextScene();
        }
        else {
            if(!isLoadingCutscene)
                StartCoroutine(LoadCutscene(idCutscene + 1));   //Carregando a próxima cutscene
        }
    }

    public void LoadNextScene() {
        if(SceneManager.GetActiveScene().buildIndex == SceneManager.sceneCountInBuildSettings - 1) {    //Se estivermos na última cena
            Application.Quit();   //Aqui podemos sair do jogo ou voltar ao menu
        }
        else
            StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));   //Carregando a próxima cena
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
