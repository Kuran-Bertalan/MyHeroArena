using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VegetationGenerator : MonoBehaviour
{
    public int vegetationNumber;
    public int spaceDisturbOfVegetation;
    public float xPosition;
    public float zPosition;
    public float yPosition;
    public Vegetation[] vegetationArray;

    private void Start()
    {
        
        for (int i = 0; i < vegetationNumber; i += spaceDisturbOfVegetation)
        {
            for (int z = 0; z < vegetationNumber; z += spaceDisturbOfVegetation)
            {
                Vegetation vegetation = vegetationArray[0];

                Vector3 positionOfVegetation = new Vector3(i, 0.2f, z);
                Vector3 vegetationShift = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                Vector3 rotationofVegetation = new Vector3(0f, Random.Range(0, 360f), 0f);
                Vector3 scaleOfVegetation = Vector3.one * Random.Range(0.5f, 1.5f);
                Vector3 positionToDisplay = new Vector3(xPosition, yPosition, zPosition);
                GameObject newVegetation = Instantiate(vegetation.RandomPrefabs());

                newVegetation.transform.SetParent(transform);
                newVegetation.transform.position = (positionOfVegetation + vegetationShift) + positionToDisplay;
                newVegetation.transform.eulerAngles = rotationofVegetation;
                newVegetation.transform.localScale = scaleOfVegetation;
            }
        }
    }
}

[System.Serializable]
public class Vegetation
{
    public string nameOfVegetation;
    public GameObject[] vegetationPrefabs;
    public GameObject RandomPrefabs()
    {
        return vegetationPrefabs[Random.Range(0, vegetationPrefabs.Length)];
    }

}
