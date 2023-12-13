using UnityEngine;

public class Food : MonoBehaviour {

    private Animator anim;

    //A tag e o id da comida serão inicializados quando a comida spawnar
    public string tagFood;
    public int idFood;

    public bool isLixo=true;

    private void Start() {
        anim = GetComponent<Animator>();
        anim.SetBool("isLixo", isLixo);
    }

    private void endAnimationFood() {   //Quando a animação da comida saindo da lixeira acabar, a tag e layer "Item" serão adicionadas ao objeto
        gameObject.tag = tagFood;
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
