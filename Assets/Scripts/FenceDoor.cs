using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FenceDoor : MonoBehaviour
{
    private bool isPlayer;
    public Animator animator1;
    public Animator animator2;
    public BoxCollider2D fenceDoorCollider;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            isPlayer = false;
        }
    }

    private void Update() {
        if (isPlayer && Input.GetKeyDown(KeyCode.E)) {
            OpenDoors();
        }
    }

    private void OpenDoors() {
        animator1.SetTrigger("OpenDoor");
        animator2.SetTrigger("OpenDoor");
        fenceDoorCollider.enabled = false;
        StartCoroutine(CloseDoorsAfterDelay(3f)); 
    }

    private IEnumerator CloseDoorsAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator1.SetTrigger("CloseDoor");
        animator2.SetTrigger("CloseDoor");
        fenceDoorCollider.enabled = true;
    }
}
