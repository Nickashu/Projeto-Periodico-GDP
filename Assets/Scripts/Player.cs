using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movementVector;
    private DialogueTrigger triggerDialogue;
    private Trash scriptTrash;
    private GameObject food;

    private int idFood=0;
    private float speed = 3.5f;
    private bool isRunning = false, isMoving=false, hasFood=false;
    private int contInteracoes = 0, limitInteractionsTutorial=5;

    public TextMeshProUGUI txtTutorialInteractions;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update() {
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

        if (!DialogueController.GetInstance().dialogueActive) {    //Se o gato n�o estiver no meio de um di�logo
            if (Input.GetKeyDown(KeyCode.Z)) {
                if (triggerDialogue != null) {
                    if (contInteracoes <= limitInteractionsTutorial) {
                        contInteracoes++;
                        txtTutorialInteractions.gameObject.SetActive(false);
                    }
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

            if (Input.GetKeyUp(KeyCode.Space) && !isMoving) {    //Barra de espa�o ser� usada para dropar a comida atual (somente quando estivermos parados)
                dropFood();
            }
        }

    }

    public void SetMovement(InputAction.CallbackContext value) {
        movementVector = value.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        Vector2 vectorResult = new Vector2(0, 0);
        if (!DialogueController.GetInstance().dialogueActive)    //Se n�o estiver no meio de um di�logo
            vectorResult = new Vector2(movementVector.x, movementVector.y).normalized;

        rb.velocity = (vectorResult * speed);

        //Anima��es do gato andando e correndo:
        if (movementVector.x != 0 || movementVector.y != 0) {
            this.isMoving = true;   //Siginifica que o gato est� se movendo
            anim.SetFloat("Speed", 5);
        }
        else {
            this.isMoving = false;
            anim.SetFloat("Speed", 0);
        }
        anim.SetBool("isRunning", isRunning);
        anim.SetFloat("Horizontal", movementVector.x);
        anim.SetFloat("Vertical", movementVector.y);
        //Debug.Log("X: " + movementVector.x + "  Y: " + movementVector.y);
    }

    //Fun��es para checar se o player se aproximou o suficiente de um NPC para acionar o di�logo:
    private void OnTriggerEnter2D(Collider2D collision) {
        if (GameController.tagsInteractable.Contains(collision.tag)) {
            Debug.Log("Entrou na hitbox de intera��o");
            if (contInteracoes <= limitInteractionsTutorial)
                txtTutorialInteractions.gameObject.SetActive(true);
        }

        if(collision.tag == "NPC")    //Aqui ser� detectado quando a hitbox de intera��o do player enconstar no NPC
            triggerDialogue = collision.gameObject.GetComponent<DialogueTrigger>();

        if (collision.tag == "TrashClosed")    //Aqui ser� detectado quando a hitbox de intera��o do player encostar em uma lixeira
            scriptTrash = collision.gameObject.GetComponent<Trash>();

        if (GameController.tagsFoods.Contains(collision.tag)) {    //Aqui ser� detectado quando o player tocar na comida, j� que foi configurado para que n�o haja detec��o entre colis�o do hitbox de di�logo com comidas
            if (collision.gameObject.name == "comidaCorvo")
                DialogueController.GetInstance().dialogueVariablesController.ChangeSpecificVariable("updateComidaCorvo");

            if (!hasFood) {
                changeAnimationFood(collision.gameObject, false);
                hasFood = true;
                GameObject objFood = collision.gameObject;
                objFood.SetActive(false);
                objFood.GetComponent<Food>().tag = "ComidaBoca";   //Esta ser� a tag tempor�ria do objeto. Ao ser dropada, a comida ativar� sua anima��o e voltar� a ter sua tag original
                objFood.GetComponent<Food>().isLixo = false;
                this.food = objFood;
                this.idFood = objFood.GetComponent<Food>().idFood;
            }
            Debug.Log("Tocou comida!!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (GameController.tagsInteractable.Contains(collision.tag)) {
            if (contInteracoes <= limitInteractionsTutorial)
                txtTutorialInteractions.gameObject.SetActive(false);
            Debug.Log("Saiu da hitbox de intera��o");
        }
        if (collision.tag == "NPC")
            triggerDialogue = null;
        if (collision.tag == "TrashClosed") {
            scriptTrash.stopSearchingFood();
            scriptTrash = null;
        }
    }


    
    private void dropFood() {           //TENHO QUE VOLTAR AQUI DEPOIS (N�O EST� FINALIZADO)
        if (this.food != null) {
            //Debug.Log("Tem comida!");
            changeAnimationFood(null, true);    //Chamando de volta a anima��o sem comida
            this.food = null;
            this.idFood = 0;
            hasFood = false;


            //this.food.gameObject.transform.parent = transform;
            //this.food.gameObject.transform.SetParent(null);
            //Vector3 posicaoComida = this.food.gameObject.transform.position;
            //posicaoComida.x = transform.position.x + 5;
            //posicaoComida.y = transform.position.y;
            //this.food.gameObject.transform.position = posicaoComida;
            //this.food.gameObject.SetActive(true);
            //Debug.Log(posicaoComida.x + "  " + posicaoComida.y);
            //this.food.gameObject.transform.position = posicaoComida;
        }
        else
            Debug.Log("N�o tem comida!");
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
}
