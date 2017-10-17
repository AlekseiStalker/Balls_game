using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverDetect : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Block")
        {
            GameManager.instance.gameOverPanel.SetActive(true);
        } 
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlusBall" || col.tag == "Scatter" || col.tag == "Coint")
        {
            GameManager.instance.listGameObjects.Remove(col.gameObject);
            Destroy(col.gameObject);
        }
    }
}
