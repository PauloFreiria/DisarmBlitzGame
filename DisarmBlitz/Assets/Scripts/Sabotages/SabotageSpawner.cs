using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SabotageSpawner : MonoBehaviour
{
    public GameObject[] objectPrefabs;
    private Transform playerTransform;
    [SerializeField] TextMeshProUGUI objectNameText;
    [SerializeField] GameObject backgroundText;
    [SerializeField] Transform spawnBlind;
    [SerializeField] Transform spawn;

    private GameObject currentObject; // Objeto escolhido ao encostar
    private bool objectChosen = false; // Indica se um objeto foi escolhido


    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (objectChosen && Input.GetKeyDown(KeyCode.Q))
        {
            SpawnObject(currentObject);
            objectChosen = false;
            backgroundText.gameObject.SetActive(false);
        }
    }

    public void ChooseRandomObject()
    {
        if (!objectChosen)
        {
            int randomIndex = Random.Range(0, objectPrefabs.Length);
            currentObject = objectPrefabs[randomIndex];
            objectNameText.text = currentObject.name;
            backgroundText.gameObject.SetActive(true);

            objectChosen = true;
        }
    }

    public void SpawnObject(GameObject objectToSpawn)
    {
        if (objectToSpawn == objectPrefabs[1])
        {
            Instantiate(objectToSpawn, spawnBlind.position, Quaternion.identity);
            objectNameText.text = "";
        }
        else
        {
            Instantiate(objectToSpawn, spawn.position, Quaternion.identity);
            objectNameText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ChooseRandomObject();
        }
    }
}
