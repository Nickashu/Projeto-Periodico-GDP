using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movementVector;
    private DialogueTrigger triggerDialogue;
    private Trash scriptTrash;
    private GameObject food, NPCDialogue=null, barStamina;

    public int idFood = 0;
    private float speed = 3.5f, maxStamina, currentStamina;
    private bool isRunning = false, hasFood = false, isMoving = false, recoveringStamina = false, isInOfficerArea=false;
    private int contInteracoes = 0, limitInteractionsTutorial=1, lives;    //qntFood representa quantas comidas pegamos durante o jogo

    public TextMeshProUGUI txtTutorialInteractions;
    public GameObject outerBarStamina, canvasLives;

    //recoverStaminaTax e loseStaminaTax representam, respectivamente, a taxa de recuperação e perda de stamina por segundo
    [SerializeField]
    private float recoverStaminaTax = 20f;
    [SerializeField]
    private float loseStaminaTax = 15f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        barStamina = outerBarStamina.transform.GetChild(0).gameObject;
        maxStamina = outerBarStamina.GetComponent<RectTransform>().rect.width;
        currentStamina = barStamina.GetComponent<RectTransform>().rect.width;
        lives = 7;
    }

    void Update() {
        if (!GameController.gameIsPaused()) {    //Se o jogo não estiver parado
            /*Mecânica de corrida*/
            if (Input.GetKey(KeyCode.LeftShift) && !recoveringStamina) {
            //if (Input.GetKey(KeyCode.LeftShift)) {  //Debug
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
                        if(NPCDialogue.layer == LayerMask.NameToLayer("Corvo")) {    //Se o NPC que eu estou falando é o corvo e eu tenho comida
                            if (NPCDialogue.GetComponent<Crow>().crowVersion == 1 && ((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState("falouCorvo1")).value == true)    //Se for a primeira versão do corvo e se eu já tiver falado com ele uma vez
                                dropFood(true);
                        }
                        else if (NPCDialogue.layer == LayerMask.NameToLayer("Leao")) {    //Se o NPC que eu estou falando é o leão e eu tenho comida
                            if (((Ink.Runtime.BoolValue)DialogueController.GetInstance().GetVariableState("falouLeao1")).value == true) {
                                GameController.idComidaLeao = this.idFood;
                                dropFood(true);
                            }
                        }
                    }

                    //Acionando o diálogo:
                    triggerDialogue.TriggerDialogue();
                }
            }

            if (Input.GetKey(KeyCode.Z)) {
                if (scriptTrash != null) {
                    if(!GameController.gameIsPaused())
                        scriptTrash.startSearchingFood();
                    else
                        scriptTrash.stopSearchingFood();
                }
            }

            if (Input.GetKeyUp(KeyCode.Z)) {
                if (scriptTrash != null)
                    scriptTrash.stopSearchingFood();
            }

            if (Input.GetKeyUp(KeyCode.C))    //Tecla que será usada para dropar a comida atual
                dropFood(false);

            //Debug:
            //if (Input.GetKeyUp(KeyCode.Alpha9))
            //    DialogueController.GetInstance().dialogueVariablesController.CheckVariableValues();


            updateStamina();
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

            //Animações do gato andando e correndo:
            if (movementVector.x != 0 || movementVector.y != 0) {
                isMoving = true;
                anim.SetFloat("Speed", 5);
            }
            else {
                isMoving = false;
                anim.SetFloat("Speed", 0);
            }
            anim.SetBool("isRunning", isRunning);
            anim.SetFloat("Horizontal", movementVector.x);
            anim.SetFloat("Vertical", movementVector.y);
        }
        else {
            anim.SetFloat("Speed", 0);
            rb.velocity = Vector2.zero;
        }
    }

    //Funções para checar se o player se aproximou o suficiente de um NPC para acionar o diálogo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!GameController.gameIsPaused()) {
            if (GameController.tagsInteractable.Contains(collision.tag)) {
                //Debug.Log("Entrou na hitbox de interação");
                if (contInteracoes <= limitInteractionsTutorial)
                    txtTutorialInteractions.gameObject.SetActive(true);
            }

            if (collision.tag == "NPC") {    //Aqui será detectado quando a hitbox de interação do player enconstar no NPC
                triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();
                NPCDialogue = collision.gameObject;
            }

            if (collision.tag == "TrashClosed")    //Aqui será detectado quando a hitbox de interação do player encostar em uma lixeira
                scriptTrash = collision.gameObject.GetComponent<Trash>();

            if (GameController.tagsFoods.Contains(collision.tag)) {    //Aqui será detectado quando o player tocar na comida, já que foi configurado para que não haja detecção entre colisão do hitbox de diálogo com comidas
                if (!hasFood && !(collision.gameObject.layer == LayerMask.NameToLayer("FoodDropped"))) {   //Se já não tiver comida e se não tiver acabdo de dropar
                    GameObject objFood = collision.gameObject;
                    hasFood = true;
                    this.food = objFood;
                    this.idFood = objFood.GetComponent<Food>().idFood;
                    changeAnimationFood(collision.gameObject, false);
                    objFood.SetActive(false);
                    SoundController.GetInstance().PlaySound("pega_comida", null);
                    //Alternado variável de diálogo:
                    int randomValueFoodDialogue = randFoodInt(this.idFood);
                    DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComida", randomValueFoodDialogue);
                }
                //Debug.Log("Tocou comida!!");
            }

            if(collision.tag == "AreaGuardas") {   //Se entramos na área que tem guardas
                isInOfficerArea = true;
                if (!GameController.beginTimer)
                    SoundController.GetInstance().PlaySound("OST_trilha1", null);
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Leao")) {   //Se entramos na hitbox do leão
                SoundController.GetInstance().PlaySound("leao_rosnando", collision.transform.parent.GetChild(1).gameObject);   //Pegando o objeto do leão e tocando o som
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (!GameController.gameIsPaused()) {
            if (GameController.tagsInteractable.Contains(collision.tag)) {
                if (contInteracoes <= limitInteractionsTutorial)
                    txtTutorialInteractions.gameObject.SetActive(false);
                //Debug.Log("Saiu da hitbox de interação");
            }
            if (collision.tag == "NPC") {
                triggerDialogue = null;
                NPCDialogue = null;
            }
            if (collision.tag == "TrashClosed") {
                scriptTrash.stopSearchingFood();
                scriptTrash = null;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("FoodDropped"))   //Depois de dropar a comida, ela fica com a tag "FoodDropped". Quando saímos da área de colisão, a comida volta a ter sua tag original
                collision.gameObject.layer = LayerMask.NameToLayer("Food");


            if (collision.tag == "AreaGuardas") {   //Se saímos da área que tem guardas
                isInOfficerArea = false;
                if (!GameController.beginTimer)
                    SoundController.GetInstance().PlaySound("OST_safe", null);
            }
        }
    }

    public void playMusicAfterChase() {
        if (isInOfficerArea)
            SoundController.GetInstance().PlaySound("OST_trilha1", null);
        else
            SoundController.GetInstance().PlaySound("OST_safe", null);
    }


    private void updateStamina() {   //Esta função é responsável pora tualizar a stamina do gato
        float newStamina = 0f;
        if (isMoving && isRunning) {
            newStamina = currentStamina - loseStaminaTax * Time.deltaTime;
            if (newStamina < 0)
                newStamina = 0;
        }
        else {
            newStamina = currentStamina + recoverStaminaTax * Time.deltaTime;
            if (newStamina > maxStamina)
                newStamina = maxStamina;
        }
        if (newStamina == 0)
            recoveringStamina = true;
        if (recoveringStamina && newStamina >= maxStamina / 2)   //Se usou toda a stamina, só vai poder voltar a correr quando a barra chegar na metade
            recoveringStamina = false;

        barStamina.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Lerp(currentStamina, newStamina, 0.1f), barStamina.GetComponent<RectTransform>().sizeDelta.y);
        currentStamina = newStamina;
    }

    private void dropFood(bool disappear) {           //TENHO QUE VOLTAR AQUI DEPOIS (NÃO ESTÁ FINALIZADO)   o parâmetro disappear define se a comida vai desaparecer ou vai ficar no chão
        if (!GameController.gameIsPaused()) {
            if (this.food != null) {
                changeAnimationFood(null, true);    //Chamando de volta a animação sem comida
                hasFood = false;
                this.idFood = 0;
                if (!disappear) {  //Se eu estiver dropando a comida no chão:
                    DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComida", 0);   //Alterando variável de diálogo
                    this.food.layer = LayerMask.NameToLayer("FoodDropped");
                    //Definindo onde a comida aparecerá ao ser dropada:
                    Vector2 positionFood = new Vector2(movementVector.x, movementVector.y).normalized;
                    this.food.transform.position = (Vector2)gameObject.transform.position - positionFood * 1.5f;
                    this.food.gameObject.GetComponent<Food>().isLixo = false;
                    this.food.SetActive(true);
                    SoundController.GetInstance().PlaySound("drop_comida", null);
                    //Debug.Log("Dropou a comida!");
                }
                this.food = null;
            }
        }
    }
    
    private void changeAnimationFood(GameObject objFood, bool drop) {   //Se drop for true, quer dizer que o jogador está tentando dropar a comida atual
        if(drop) {   //Entra aqui se estiver dropando a comida
            //Debug.Log("Dropou a comida!");
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
        if (GameController.idsNotMeat.Contains(indexFood))   //Se a comida pega não for carne
            returnValue = random.Next(1, 4);
        else
            returnValue = random.Next(4, 7);

        return returnValue;
    }

    public void looseLife() {    //Esta função será chamada pelo script do guarda quando o gato for pego
        lives--;
        updateCanvasVida();
        if(lives == 0) {
            //Debug.Log("Morreu");
            TransitionsController.GetInstance().LoadLastScene((int)GameController.FinaisJogo.GameOver);
        }
        else
            TransitionsController.GetInstance().TransitionAfterCaught();
    }

    public void returnToInitialPosition() {
        Vector3 newPositionPlayer = new Vector3(GameController.comecoMapaX, GameController.comecoMapaY, transform.position.z);
        Vector3 newPositionCamera = new Vector3(GameController.comecoMapaX, GameController.comecoMapaY, Camera.main.transform.position.z);
        transform.position = newPositionPlayer;
        Camera.main.transform.position = newPositionCamera;
    }

    private void updateCanvasVida() {
        canvasLives.GetComponent<Animator>().SetInteger("vidas", lives);   //Atualizando a animação das vidas
    }
}
