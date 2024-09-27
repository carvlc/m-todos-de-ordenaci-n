using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnScript : MonoBehaviour
{
    public void MostrarBurbuja(){
        SceneManager.LoadScene(2);
    }

    public void MostrarSeleccion(){
        SceneManager.LoadScene(1);
    }

    public void Volver(){
        SceneManager.LoadScene(0);
    }
}
