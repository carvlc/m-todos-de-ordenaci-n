using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class BubbleSort : MonoBehaviour
{
    [SerializeField] private Element element = null;
    [SerializeField] private Button buttonSort = null;
    [SerializeField] private Button buttonShuffle = null;
    [SerializeField] private Button buttonRandom = null;
    [SerializeField] private Text txtTimer = null;
    
    private Element[] array = new Element[10];
    
    private float startPosX = -7.8f;
    private const float posY = 2.5f;
    private float Scale = 3.0f;
    private const float offsetScale = 1.14f;
    
    private bool clickTwoElements = false;
    private byte firstClickElementID = 0;
    private byte numElementsMoving = 0;
    
    private float timeSorting = 0.0f;
    private bool timerSorting = false;

    void OnEnable()
    {
        Element.elementClick += ClickElementEvent;
        Element.elementMove += isMoving;

        buttonShuffle.onClick.AddListener(ShuffleArray);
        buttonSort.onClick.AddListener(SortArray);
        buttonRandom.onClick.AddListener(InitRandomArray);
    }

    void OnDisable()
    {
        Element.elementClick -= ClickElementEvent;
        Element.elementMove -= isMoving;
    }

    void Start()
    {
        InitArray();
        ShowArray();
    }
       
    private void InitArray(bool random = false)
    {
        float componentColor = 0.0f;
        System.Random rnd = new System.Random();

        for (byte i = 0; i < array.Length; i++)
        {
            if (random) Destroy(array[i].gameObject);

            array[i] = Instantiate(this.element, new Vector3(startPosX, posY, 0.0f), Quaternion.identity) as Element;
            array[i].elementID = i;
            array[i].transform.localScale = new Vector3(Scale, Scale, 1.0f);

            componentColor = 0.1f * i;
            array[i].InitColor = new Color(componentColor, componentColor, componentColor);

            if (random) Scale = rnd.Next(3, 10); else Scale *= offsetScale;
        }
    }

    private void ShowArray()
    {
        float elementPosX = startPosX;
        float offsetPosX = Math.Abs(startPosX * 2) / (array.Length - 1);

        for (int i = 0; i < array.Length; i++)
        {
            array[i].transform.localPosition = new Vector3(elementPosX, posY, 0.0f);
            elementPosX += offsetPosX;
        }
    }

    private void SwapElementsArray(ref Element element1, ref Element element2)
    {
        byte tempID = element1.elementID;
        element1.elementID = element2.elementID;
        element2.elementID = tempID;

        Element temp = element1;
        element1 = element2;
        element2 = temp;
    }

    private void ShuffleArray()
    {
        if (numElementsMoving == 0)
        {
            System.Random rnd = new System.Random();
            int n = array.Length;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                SwapElementsArray(ref array[n], ref array[k]);
            }
            ShowArray();
        }
    }

    void InitRandomArray()
    {
        if (numElementsMoving == 0)
        {
            InitArray(true);
            ShowArray();
        }
    }

    private void SortArray()
    {
        StartCoroutine(StartSortingTimer());
        StartCoroutine(BubbleSortArray());
    }

    void isMoving(byte elementID, bool isMoving)
    {
        numElementsMoving = isMoving ? ++numElementsMoving : --numElementsMoving;
        if (timerSorting == false)
        {
            if (numElementsMoving != 0)
            {
                buttonRandom.interactable = false;
                buttonShuffle.interactable = false;
                buttonSort.interactable = false;
            }
            else
            {
                buttonRandom.interactable = true;
                buttonShuffle.interactable = true;
                buttonSort.interactable = true;
                ShowArray();
            }
        }
    }
    private void ClickElementEvent(byte elementID)
    {
        if (numElementsMoving == 0 && timerSorting == false)
        {
            if (!clickTwoElements)
            {
                firstClickElementID = elementID;
                clickTwoElements = true;
            }
            else
            {
                clickTwoElements = false;

                if (firstClickElementID != elementID)
                {
                    array[firstClickElementID].MoveTo(array[elementID].transform.localPosition);
                    array[elementID].MoveTo(array[firstClickElementID].transform.localPosition);

                    SwapElementsArray(ref array[firstClickElementID], ref array[elementID]);
                }
            }
        }
    }
   
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        float milliSeconds = (timeToDisplay % 1) * 1000;

        txtTimer.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);

        if (minutes == 10) timerSorting = false; 
    }

    private IEnumerator StartSortingTimer()
    {
        timerSorting = true;
        timeSorting = 0;
        while (timerSorting)
        {
            timeSorting += Time.deltaTime;
            DisplayTime(timeSorting);
            yield return null;
        }
    }
    

    private IEnumerator BubbleSortArray()
{
    buttonRandom.interactable = false;
    buttonShuffle.interactable = false;
    buttonSort.interactable = false;

    for (int i = 0; i < array.Length - 1; i++)
    {
        bool swapped = false; // Variable para verificar si se ha realizado un intercambio

        for (int j = 0; j < array.Length - 1 - i; j++)
        {
            array[j].SetColor(Color.green); // Color verde para el elemento actual
            array[j + 1].SetColor(Color.green); // Color verde para el siguiente elemento
            yield return new WaitForSeconds(1); // Espera un segundo para visualización

            // Comparar los elementos adyacentes
            if (array[j].transform.localScale.x > array[j + 1].transform.localScale.x)
            {
                // Intercambiar elementos si están en el orden incorrecto
                array[j].MoveTo(array[j + 1].transform.localPosition);
                array[j + 1].MoveTo(array[j].transform.localPosition);

                while (numElementsMoving != 0)
                {
                    yield return null; // Espera a que los elementos se muevan
                }

                SwapElementsArray(ref array[j], ref array[j + 1]); // Intercambiar los elementos en el array
                ShowArray(); // Mostrar el estado actual del array
                swapped = true; // Se realizó un intercambio
            }
            else
            {
                // Restablecer el color inicial si no se intercambia
                array[j].InitColor = array[j].InitColor;
                array[j + 1].InitColor = array[j + 1].InitColor;
            }
        }

        // Si no se realizó ningún intercambio, el array ya está ordenado
        if (!swapped) break;

        // Color azul para el elemento final de la pasada
        array[array.Length - 1 - i].SetColor(Color.blue);
    }

    // Color azul para el último elemento
    array[0].SetColor(Color.blue);

    // Restablecer los colores iniciales
    for (int j = 0; j < array.Length; j++)
    {
        array[j].InitColor = array[j].InitColor;
        yield return new WaitForSeconds(0.1f); // Esperar un poco
    }

    buttonRandom.interactable = true;
    buttonShuffle.interactable = true;
    buttonSort.interactable = true;

    timerSorting = false;
}

}

