using UnityEngine;
using UnityEngine.Events;

public class SearchBar : MonoBehaviour {

    public UnityEvent endSearchingFoodEvent;

    private void searchAnimationEnd() {
        endSearchingFoodEvent.Invoke();    //Ativando o evento ao fim da anima��o (por meio de um animation event)
    }

}
