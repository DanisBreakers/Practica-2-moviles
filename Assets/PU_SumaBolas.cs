﻿using UnityEngine;

public class PU_SumaBolas : MonoBehaviour {
    private int bolasQueSuma;

	// Use this for initialization
	void Start () {
        switch (gameObject.tag)
        {
            case "PowerUp1":
                bolasQueSuma = 1;
                break;
            case "PowerUp2":
                bolasQueSuma = 2;
                break;
            case "PowerUp3":
                bolasQueSuma = 3;
                break;
            default:
                Debug.Log("Error en el nombre del gameobject: ¡O has puesto el nombre mal, " +
                             "o este objeto no debería contener este componente!");
                Destroy(gameObject);
                break;
        }
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        Pelota aux = collision.gameObject.GetComponent<Pelota>();
        if (aux != null)
        {
            LevelManager.instance.SumaPelotasAlNumeroMaximo(bolasQueSuma);
            Destroy(this.gameObject);
        }
    }
}
