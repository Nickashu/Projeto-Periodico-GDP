using Ink.Parsed;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movementVector;
    private DialogueTrigger triggerDialogue;
    private Trash scriptTrash;
    private GameObject food, NPCDialogue=null;

    public int idFood = 0;
    private float speed = 3.5f;
    private bool isRunning = false, hasFood=false;
    private int contInteracoes = 0, limitInteractionsTutorial=1, lives;    //qntFood representa quantas comidas pegamos durante o jogo

    public TextMeshProUGUI txtTutorialInteractions, txtVidas;
    public GameObject transitionAfterCaught;   //Esta ser� a tela de transi��o que aparecer� depois que o gato for pego

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lives = 2;
        updateCanvasVida();
    }

    void Update() {
        if (!GameController.gameIsPaused()) {    //Se o jogo n�o estiver parado
            /*Mec�nica de corrida*/
            if (Input.GetKey(KeyCode.LeftShift)) {
                if (!isRunning)
                    speed *= 2;
                isRunning = true;
            }
            else {
                isRunning = false;
                speed = 3.5f;
            }

            if (Input.GetKeyDown(KeyCode.Z)) {
                if (NPCDialogue != null && triggerDialogue != null) {
                    if (contInteracoes <= limitInteractionsTutorial) {
                        contInteracoes++;
                        txtTutorialInteractions.gameObject.SetActive(false);
                    }

                    //Detectando se eu estou falando com o corvo pela segunda vez e tenho comida (neste caso, minha comida desaparece pois estou dando ela ao corvo):
                    if(idFood != 0) {
                        if(NPCDialogue.layer == LayerMask.NameToLayer("Corvo")) {    //Se o NPC que eu estou falando � o corvo e eu tenho comida
                            if (NPCDialogue.GetComponent<Crow>().crowVersion == 1 && ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState("falouCorvo1")).value == true)    //Se for a primeira vers�o do corvo e se eu j� tiver falado com ele uma vez
                                dropFood(true);
                        }
                        else if (NPCDialogue.layer == LayerMask.NameToLayer("Leao")) {    //Se o NPC que eu estou falando � o le�o e eu tenho comida
                            if (((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState("falouLeao1")).value == true) {
                                GameController.idComidaLeao = this.idFood;
                                dropFood(true);
                            }
                        }
                    }

                    //Acionando o di�logo:
                    triggerDialogue.TriggerDialogue();
                }
            }

            if (Input.GetKey(KeyCode.Z)) {
                if (scriptTrash != null)
                    scriptTrash.startSearchingFood();
            }

            if (Input.GetKeyUp(KeyCode.Z)) {
                if (scriptTrash != null)
                    scriptTrash.stopSearchingFood();
            }

            if (Input.GetKeyUp(KeyCode.Space))    //Barra de espa�o ser� usada para dropar a comida atual
                dropFood(false);

            //Debug:
            if (Input.GetKeyUp(KeyCode.Alpha9))
                DialogueController.GetInstance().dialogueVariablesController.CheckVariableValues();
        }

    }

    public void SetMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        if (!GameController.gameIsPaused()) {
            Vector2 vectorResult = new Vector2(0, 0);
            vectorResult = new Vector2(movementVector.x, movementVector.y).normalized;

            rb.velocity = (vectorResult * speed);

            //Anima��es do gato andando e correndo:
            if (movementVector.x != 0 || movementVector.y != 0) 
                anim.SetFloat("Speed", 5);
            else 
                anim.SetFloat("Speed", 0);
            anim.SetBool("isRunning", isRunning);
            anim.SetFloat("Horizontal", movementVector.x);
            anim.SetFloat("Vertical", movementVector.y);
        }
        else {
            anim.SetFloat("Speed", 0);
            rb.velocity = Vector2.zero;
        }
    }

    //Fun��es para checar se o player se aproximou o suficiente de um NPC para acionar o di�logo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!GameController.gameIsPaused()) {
            if (GameController.tagsInteractable.Contains(collision.tag)) {
                Debug.Log("Entrou na hitbox de intera��o");
                if (contInteracoes <= limitInteractionsTutorial)
                    txtTutorialInteractions.gameObject.SetActive(true);
            }

            if (collision.tag == "NPC") {    //Aqui ser� detectado quando a hitbox de intera��o do player enconstar no NPC
                triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();
                NPCDialogue = collision.gameObject;
            }

            if (collision.tag == "TrashClosed")    //Aqui ser� detectado quando a hitbox de intera��o do player encostar em uma lixeira
                scriptTrash = collision.gameObject.GetComponent<Trash>();

            if (GameController.tagsFoods.Contains(collision.tag)) {    //Aqui ser� detectado quando o player tocar na comida, j� que foi configurado para que n�o haja detec��o entre colis�o do hitbox de di�logo com comidas
                if (!hasFood && !(collision.gameObject.layer == LayerMask.NameToLayer("FoodDropped"))) {   //Se j� n�o tiver comida e se n�o tiver acabdo de dropar
                    GameObject objFood = collision.gameObject;
                    hasFood = true;
                    this.food = objFood;
                    this.idFood = objFood.GetComponent<Food>().idFood;
                    changeAnimationFood(collision.gameObject, false);
                    objFood.SetActive(false);
                    //objFood.GetComponent<Food>().tag = "ComidaBoca";   //Esta ser� a tag tempor�ria do objeto. Ao ser dropada, a comida ativar� sua anima��o e voltar� a ter sua tag original
                    //Alternado vari�vel de di�logo:
                    int randomValueFoodDialogue = randFoodInt(this.idFood);
                    DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComida", randomValueFoodDialogue);
                }
                Debug.Log("Tocou comida!!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!GameController.gameIsPaused()) {
            if (GameController.tagsInteractable.Contains(collision.tag)) {
                if (contInteracoes <= limitInteractionsTutorial)
                    txtTutorialInteractions.gameObject.SetActive(false);
                Debug.Log("Saiu da hitbox de intera��o");
            }
            if (collision.tag == "NPC") {
                triggerDialogue = null;
                NPCDialogue = null;
            }
            if (collision.tag == "TrashClosed") {
                scriptTrash.stopSearchingFood();
                scriptTrash = null;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("FoodDropped"))   //Depois de dropar a comida, ela fica com a tag "FoodDropped". Quando sa�mos da �rea de colis�o, a comida volta a ter sua tag original
                collision.gameObject.layer = LayerMask.NameToLayer("Food");
        }
    }


    private void dropFood(bool disappear) {           //TENHO QUE VOLTAR AQUI DEPOIS (N�O EST� FINALIZADO)   o par�metro disappear define se a comida vai desaparecer ou vai ficar no ch�o
        if (!GameController.gameIsPaused()) {
            if (this.food != null) {
                changeAnimationFood(null, true);    //Chamando de volta a anima��o sem comida
                hasFood = false;
                this.idFood = 0;
                if (!disappear) {  //Se eu estiver dropando a comida no ch�o:
                    DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComida", 0);   //Alterando vari�vel de di�logo
                    this.food.layer = LayerMask.NameToLayer("FoodDropped");
                    //Definindo onde a comida aparecer� ao ser dropada:
                    Vector2 positionFood = new Vector2(movementVector.x, movementVector.y).normalized;
                    this.food.transform.position = (Vector2)gameObject.transform.position - positionFood * 1.2f;
                    this.food.gameObject.GetComponent<Food>().isLixo = false;
                    this.food.SetActive(true);
                    Debug.Log("Dropou a comida!");
                }
                this.food = null;
            }
            else
                Debug.Log("N�o tem comida!");
        }
    }
    
    private void changeAnimationFood(GameObject objFood, bool drop) {   //Se drop for true, quer dizer que o jogador est� tentando dropar a comida atual
        if(drop) {   //Entra aqui se estiver dropando a comida
            Debug.Log("Dropou a comida!");
            anim.SetInteger("food", 0);
        }
        else {
            if(objFood != null) {
                int idFood = objFood.GetComponent<Food>().idFood;
                anim.SetInteger("food", idFood);
            }
        }
    }

    private int randFoodInt(int indexFood) {
        int returnValue=0;
        System.Random random = new System.Random();
        if (GameController.idsNotMeat.Contains(indexFood))   //Se a comida pega n�o for carne
            returnValue = random.Next(1, 4);
        else
            returnValue = random.Next(4, 7);

        return returnValue;
    }

    public void looseLife() {    //Esta fun��o ser� chamada pelo script do guarda quando o gato for pego
        lives--;
        updateCanvasVida();
        transitionAfterCaught.SetActive(false);
        transitionAfterCaught.SetActive(true);
        if(lives == 0)
            Debug.Log("Morreu!");
    }

    public void returnToInitialPosition() {
        Vector3 newPosition = new Vector3(GameController.comecoMapaX, GameController.comecoMapaY);
        transform.position = newPosition;
    }

    private void updateCanvasVida() {
        txtVidas.text = lives.ToString();
    }
}
