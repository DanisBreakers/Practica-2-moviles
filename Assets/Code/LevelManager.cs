﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LectorTXT))]

public class LevelManager : MonoBehaviour
{

    #region Attributes

    //ATRIBUTOS DE LEVELMANAGER
    LectorTXT lectorNivel;
    bool gameOver;

    int numMaxPelotas;                                      //Numero (máximo actual) de pelotas que va a generar el spawner.
    int numPelotasAct;                                      //Numero de pelotas por el tablero
    public Pelota PelotaPrefab;                             //Prefab de la pelota
    List<Pelota> ListaPelotas;                             //Array de pelotas
    public Button vueltaCasa;                               //Boton de vuelta a casa

    List<Bloque> ListaBloques;                              //Lista de Bloques
    public Bloque Bloque_1;                                 //Prefab del bloque 1
    public Bloque Bloque_2;                                 //Prefab del bloque 2
    public Bloque Bloque_3;                                 //Prefab del bloque 3
    public Bloque Bloque_4;                                 //Prefab del bloque 4
    public Bloque Bloque_5;                                 //Prefab del bloque 5
    public Bloque Bloque_6;                                 //Prefab del bloque 6

    public GameObject PU_sumaPelotas1;                      //Prefab del powerup
    public GameObject PU_sumaPelotas2;                      //Prefab del powerup
    public GameObject PU_sumaPelotas3;                      //Prefab del powerup


    public Spawner spawner;                                 //Spawner del nivel
    bool llegadaPrimeraPelota;                              //Bool que determina si ha llegado la primera serpiente
    Vector3 spawnerPosition;                                //Posicion siguiente/actual del spawner
    bool puedeInstanciar;                                   //Determina si puede generar bolas o no

    public DeathZone deathZone;                             //Deathzone del nivel
    public GameObject warning;                              //Warning de que estás a punto de morir




    LineRenderer shootLine;                                 //Marca la trayectoria de disparo


    #endregion

    #region Singleton
    public static LevelManager instance;

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    //Use this for init
    void Start()
    {
        gameOver = false;

        lectorNivel = GetComponentInChildren<LectorTXT>();
        lectorNivel.LoadLevel(420);

        puedeInstanciar = true;
        spawnerPosition = spawner.gameObject.transform.position;

        ListaPelotas = new List<Pelota>();

        shootLine = GetComponentInChildren<LineRenderer>();

        numMaxPelotas = 10;         //Valor inicial
        numPelotasAct = 0;


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!gameOver)
        {
            //ACTUALIZA TEXTO PUNTOS BLA BLA BLA


            //INPUT

            if (Input.GetMouseButtonDown(0) && puedeInstanciar)
            {
                shootLine.enabled = true;
            }

            if (Input.GetMouseButtonUp(0) && puedeInstanciar)
            {
                shootLine.enabled = false;
                numPelotasAct = numMaxPelotas;          //Establecemos el nº de pelotas en el tablero
                spawner.GeneraPelotas(numPelotasAct, PelotaPrefab);

                puedeInstanciar = false;
            }
        }
    }

    /// <summary>
    ///Metodo encargado de situar el spawner en la nueva posición,
    ///de bajar todos los muros 1 posicion hacia abajo -> Y comprobar si se ha acabado la partida!
    ///establecer puntos, nuevo numero max de pelotas y de algo más que no recuerdo ahora mismo.
    /// </summary>
    private void PreparaSiguienteGameRound()
    {
        Debug.Log("*PREPARNDO NUEVO GAME ROUND*");

        int it = 0;
        while (!gameOver && it < ListaBloques.Count)
        {
            ListaBloques[it].transform.position -= new Vector3(0, 1, 0);

            if ((ListaBloques[it].transform.position.y - spawner.transform.position.y) <= 1)
            {
                gameOver = true;
            }
            //Si algún bloque está lo suficientemente cerca del Spawner activamos el warning
            else if ((ListaBloques[it].transform.position.y - spawner.transform.position.y) <= 4 && !warning.activeSelf)
            {
                warning.SetActive(true);
            }

            it++;
        }

        //ACTUALIZA SPAWNER
        puedeInstanciar = true;
        llegadaPrimeraPelota = false;

        //ACTUALIZA PUNTOS Y DEMÁS MIERDAS
        numPelotasAct = 0;
        //numMaxPelotas += 10;    
    }

    /// <summary>
    /// Determina la posicion nueva del spawner si es la primera bola
    /// Llama a la bola para avisarla de que modifique su comportamiento
    /// </summary>
    /// <param name="pelota">Pelota del deathzone</param>
    public void LlegadaPelota(Pelota pelota)
    {

        if (!llegadaPrimeraPelota)
        {
            llegadaPrimeraPelota = true;
            spawnerPosition.x = pelota.gameObject.transform.position.x;
            spawner.ActualizaPosicionSpawner(spawnerPosition);

        }

        pelota.GoToSpawner(10, RestaPelota);

    }


    /// <summary>
    /// Suma n pelotas al numero maximo de pelotas que tienes en este nivel
    /// </summary>
    /// <param name="n">pelotas a sumar</param>
    public void SumaPelotasAlNumeroMaximo(int n) {
        numMaxPelotas += n;
    }

    public void Recogida()
    {
        foreach (Pelota p in ListaPelotas)
        {
            // p.SetVueltaACasa();
        }

    }

    /// <summary>
    /// Instancia en la escena el power up del tipo dado.
    /// </summary>
    /// <param name="tipo">tipo del powerup</param>
    public void CreaPowerUp(int x, int y, int tipo)
    {
        switch (tipo)
        {
            case 21:
                Instantiate(PU_sumaPelotas1, new Vector3(x, y, 0), Quaternion.identity);
                break;
            case 22:
                Instantiate(PU_sumaPelotas2, new Vector3(x, y, 0), Quaternion.identity);
                break;
            case 23:
                Instantiate(PU_sumaPelotas3, new Vector3(x, y, 0), Quaternion.identity);
                break;
            default:
                Debug.Log("tipo no registrado! No se crea nada");
                break;
        }
    }

    #region  Methods Bloque
    /// <summary>
    /// Crea una instancia del prefab del Bloque
    /// según el tipo del mismo y lo introduce en la lista de Bloques
    /// </summary>
    /// <param name="x">Posicion X en el mundo</param>
    /// <param name="y">Posicion Y en el mundo</param>
    /// <param name="tipo">Tipo del bloque</param>
    /// <param name="vida">Vida del bloque</param>
    public void CreaBloque(int x, int y, int tipo, int vida)
    {
        if (ListaBloques == null)
        {
            ListaBloques = new List<Bloque>();
        }

        Bloque bloque = null;
        switch (tipo)
        {

            case 0:
                Debug.Log("No debería haber un tipo 0");
                break;
            case 1:
                bloque = Instantiate(Bloque_1); 
                break;
            case 2:
                bloque = Instantiate(Bloque_2);
                break;
            case 3:
                bloque = Instantiate(Bloque_3);
                break;
            case 4:
                bloque = Instantiate(Bloque_4);
                break;
            case 5:
                bloque = Instantiate(Bloque_5);
                break;
            case 6:
                bloque = Instantiate(Bloque_6);
                break;

            default:
                Debug.Log("TIPO NO REGISTRADO. Crea un caso en el switch o revisa el txt dado.");
                break;
        }

        //Configuramos el bloque y lo metemos en el vector
        bloque.ConfiguraBloque(x, y, vida);
        ListaBloques.Add(bloque);

    }

    public void RestaBloque(Bloque bloqueQuitado)
    {
        ListaBloques.Remove(bloqueQuitado);

        Destroy(bloqueQuitado.gameObject);

        //A sumar puntos o lo que sea
    }
    #endregion

    //Métodos de la pelota para la gestion de nivel
    #region Methods Pelota

    public void SumaPelota(Pelota nuevaPelota)
    {
        ListaPelotas.Add(nuevaPelota);
    }



    //GM es notificado de que ha llegado una pelota
    //Si es la ultima, reset del bool de posicion del Spawner
    public void RestaPelota(Pelota pelotaQuitada)
    {
        //La sacamos de la lista
        numPelotasAct--;

        ListaPelotas.Remove(pelotaQuitada);

        Destroy(pelotaQuitada.gameObject);

        spawner.SumaContadorSpawner();

        if (numPelotasAct <= 0) //Si han llegado todas las pelotas
        {
            PreparaSiguienteGameRound();
        }
    }

    #endregion


    //Métodos del spawner para la gestion de nivel
    #region Methods Spawner

    public int GetBolasAct()
    {
        return numPelotasAct;
    }

    public Vector3 GetSpawnerPosition()
    {
        return spawner.gameObject.transform.position;
    }
    #endregion



}