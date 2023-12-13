using UnityEngine;

public class Food : MonoBehaviour {

    private Animator anim;

    //A tag e o id da comida ser�o inicializados quando a comida spawnar
    public string tagFood;
    public int idFood;

    public bool isLixo=true;

    private void Start() {
        anim = GetComponent<Animator>();
        anim.SetBool("isLixo", isLixo);
    }

    private void endAnimationFood() {   //Quando a anima��o da comida saindo da lixeira acabar, a tag e layer "Item" ser�o adicionadas ao objeto
        gameObject.tag = tagFood;
        gameObject.layer = LayerMask.NameToLayer("Item");
    }
}
