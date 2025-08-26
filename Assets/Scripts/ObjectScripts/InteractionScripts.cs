using UnityEngine;

public class InteractionScripts : MonoBehaviour
{
    

    private void OnEnable()
    {
        InteractionScript.interactionEvent += InteractionTriggered;
    }
    private void OnDisable()
    {
        InteractionScript.interactionEvent -= InteractionTriggered;
    }




    public bool playerFound = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerFound = true;
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerFound = false;
        }
        
    }

    private void InteractionTriggered()
    {

    }
}
