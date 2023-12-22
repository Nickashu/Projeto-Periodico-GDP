using UnityEngine;

public class Trash : MonoBehaviour {

    private Animator anim;

    [SerializeField]
    private GameObject food;
    [SerializeField]
    private SearchBar searchBar;
    [SerializeField]
    private int idFood=1;    //Este campo definir� qual comida estr� na lixeira

    private void Start() {
        anim = GetComponent<Animator>();
        searchBar.endSearchingFoodEvent.AddListener(endSearchFood);   //Quando a barra de busca chegar ao fim, ser� ativado o m�todo endSearchFood
    }

    public void startSearchingFood() {     //Este m�todo ser� chamado quando o jogador interagir com a lixeira
        if (gameObject.tag == "TrashClosed")
            searchBar.gameObject.SetActive(true);
    }

    public void stopSearchingFood() {     //Este m�todo ser� chamado quando o jogador solatar a tecla Z ao interagir com a lixeira
        if (gameObject.tag == "TrashClosed")
            searchBar.gameObject.SetActive(false);
    }

    public void endSearchFood() {
        searchBar.gameObject.SetActive(false);
        if(anim != null)   //Se n�o for a lixeira de palha�o (que n�o tem anima��o ao ser aberta)
            anim.SetBool("open", true);
        gameObject.tag = "TrashOpened";
        SoundController.GetInstance().PlaySound("lixo_aberto", null);
        food.GetComponent<Food>().tagFood = GameController.dicIdFood[this.idFood];    //Adicionando a tag espec�fica na comida
        food.GetComponent<Food>().idFood = this.idFood;    //Adicionando o id espec�fico na comida
        food.SetActive(true);
    }
}
