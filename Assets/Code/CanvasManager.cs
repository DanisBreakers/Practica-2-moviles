﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CanvasManager : MonoBehaviour
{

    //Paneles
    public GameObject panelCompra;
    public GameObject panelGanador;
    public GameObject panelPerdedor;
    public GameObject panelFinNiveles;

    //Puntos
    public Image BarraPuntos;
    public float PuntosMaximos;

    //Textos
    public Text textoPuntuacion;
    public Text textoDiamantes;
    public Text textoPago;
    public Text textoPagoInsuficiente;
    public Text textoPelotas;

    //Botones
    public Button botonAceptarPago;
    public Button botonCancelarPago;
    public Button botonAceptarGenerico;                 //Acepta la accion para desactivar el panel

    public Button botonRegresarSpawner;                 //Boton para regresar las bolas

    //Estrellas
    private GameObject[] estrellasJuego;
    public GameObject estrellaBase;
    public GameObject estrellaMedio;
    public GameObject estrellaFinal;


    //Sprites
    public Sprite estrellaConseguida;
    public SpriteRenderer avanceRapido;

    public static CanvasManager instance;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);
    }


    // Use this for initialization
    void Start()
    {
        setReturnSpawnActive(false);

        estrellasJuego = new GameObject[3];
        estrellasJuego[0] = estrellaBase;
        estrellasJuego[1] = estrellaMedio;
        estrellasJuego[2] = estrellaFinal;
    }

    // Update is called once per frame
    void Update()
    {
        //Actualiza puntuacion
        float puntuacionAct = LevelManager.instance.GetPuntuacionActual();
        textoPuntuacion.text = puntuacionAct.ToString();
        BarraPuntos.fillAmount = (puntuacionAct / PuntosMaximos);

        textoDiamantes.text = GameManager.instance.GetDiamantes().ToString();

        int pelotasActuales = LevelManager.instance.GetPelotasSpawner();
        textoPelotas.text = "Brillos: " + pelotasActuales;
    }

    //Métodos de las puntuaciones
    #region Puntuacion


    /// <summary>
    /// Enciende la estrella nEstrella en la interfaz
    /// </summary>
    /// <param name="nEstrella"></param>
    public void EnciendeEstrella(int nEstrella)
    {
        for (int i = 0; i < nEstrella; i++)
        {
            estrellasJuego[i].GetComponent<Image>().sprite = estrellaConseguida;
        }
    }

    //Efecto visual para el icono de avance
    public void ParpadeaIconoAvance()
    {
        StartCoroutine(Parpadeo());
    }
    //Parpadea la imagen
    IEnumerator Parpadeo()
    {
        for (int i = 0; i < 2; i++)
        {
            avanceRapido.color = new Color(avanceRapido.color.r, avanceRapido.color.g, avanceRapido.color.b, 1);
            yield return new WaitForSeconds(0.2f);
            avanceRapido.color = new Color(avanceRapido.color.r, avanceRapido.color.g, avanceRapido.color.b, 0);
            yield return new WaitForSeconds(0.2f);
        }
    
        yield break;
    }

    /// <summary>
    /// Método que es llamado para asignar la puntuacion máxima que quieres en el nivel
    /// </summary>
    /// <param name="maxValue"></param>
    public void SetMaxPuntuacion(float maxValue)
    {
        PuntosMaximos = maxValue;
        Debug.Log("PUNTOS MAXIMOS: " + PuntosMaximos);
    }
    #endregion

    #region paneles
    /// <summary>
    /// Establece el panel a los valores por defecto,
    /// tiene que ser llamado cuando quieras quitar los popup en "Panel_Cancelar" 
    /// y reanudar la partida
    /// </summary>
    private void SetPanelToDefault()
    {
        textoPagoInsuficiente.gameObject.SetActive(false);
        botonAceptarGenerico.gameObject.SetActive(false);
        botonAceptarPago.gameObject.SetActive(true);
        botonCancelarPago.gameObject.SetActive(true);

    }

    /// <summary>
    /// Activar el panel que se muestra cuando has ganado la partida
    /// </summary>
    public void ActivaPanelGanador()
    {
        panelGanador.SetActive(true);
    }

    public void ActivaPanelPerdedor()
    {
        panelPerdedor.SetActive(true);
    }


    public void ActivaPanelFinNivel()
    {
        panelGanador.SetActive(false);
        panelFinNiveles.SetActive(true);
    }

    #endregion //Paneles

    //CALLBACKS DE LOS BOTONES

    /// <summary>
    /// Se llama desde el panel de nivel superado 
    /// y te permite desbloquear y empezar el siguiente nivel
    /// </summary>
    public void IrAlSiguienteNivel()
    {
        LevelManager.instance.SiguienteNivel();
    }

    /// <summary>
    /// Método que te permite cargar la escena del menu principal
    /// desde cualquier otra
    /// </summary>
    public void IrAlMenuPrincipal()
    {
        LevelManager.instance.CargaMenuPrincipal();
    }

    /// <summary>
    /// Activa y desactiva el botón del spawner
    /// </summary>
    public void setReturnSpawnActive(bool state)
    {
        botonRegresarSpawner.interactable = state;
        botonRegresarSpawner.gameObject.SetActive(state);

    }

    public void Reiniciar()
    {
        LevelManager.instance.ReiniciaNivel();
    }

    public void Panel_Confirmacion()
    {
        panelCompra.SetActive(true);
        LevelManager.instance.Pausa = true;
    }

    public void Panel_Aceptar()
    {


        if (!GameManager.instance.RestaDiamantes(25))
        {
            botonAceptarPago.gameObject.SetActive(false);
            botonCancelarPago.gameObject.SetActive(false);

            textoPagoInsuficiente.gameObject.SetActive(true);
            botonAceptarGenerico.gameObject.SetActive(true);
        }
        else
        {
            //LLamar al levelManager para que active el power up

            LevelManager.instance.ColocaPowerUpLasers(4);
            panelCompra.SetActive(false);
            LevelManager.instance.Pausa = false;
        }
    }

    public void Panel_Cancelar()
    {
        SetPanelToDefault();
        panelCompra.SetActive(false);
        LevelManager.instance.Pausa = false;
    }

}