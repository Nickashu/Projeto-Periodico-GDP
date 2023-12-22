using UnityEngine;

public class Trash : MonoBehaviour {

    private Animator anim;

    [SerializeField]
    private GameObject food;
    [SerializeField]
    private SearchBar searchBar;
    [SerializeField]
    private int idFood=1;    //Este campo definirá qual comida estrá na lixeira

    private void Start() {
        anim = GetComponent<Animator>();
        searchBar.endSearchingFoodEvent.AddListener(endSearchFood);   //Quando a barra de busca chegar ao fim, será ativado o método endSearchFood
    }

    public void startSearchingFood() {     //Este método será chamado quando o jogador interagir com a lixeira
        if (gameObject.tag == "TrashClosed")
            searchBar.gameObject.SetActive(true);
    }

    public void stopSearchingFood() {     //Este método será chamado quando o jogador solatar a tecla Z ao interagir com a lixeira
        if (gameObject.tag == "TrashClosed")
            searchBar.gameObject.SetActive(false);
    }

    public void endSearchFood() {
        searchBar.gameObject.SetActive(false);
        if(anim != null)   //Se não for a lixeira de palhaço (que não tem animação ao ser aberta)
            anim.SetBool("open", true);
        gameObject.tag = "TrashOpened";
        SoundController.GetInstance().PlaySound("lixo_aberto", null);
        food.GetComponent<Food>().tagFood = GameController.dicIdFood[this.idFood];    //Adicionando a tag específica na comida
        food.GetComponent<Food>().idFood = this.idFood;    //Adicionando o id específico na comida
        food.SetActive(true);
    }
}
