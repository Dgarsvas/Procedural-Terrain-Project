using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : MonoBehaviour
{
    public ButtonHandler buttonHandler;

    public float spawningInterval = 1f;

    public GameObject[] shapes;

    public Color[] colors;

    public Transform spawnZone;

    public List<Color> currentColorArray;

    public AnimationCurve speedIncrementationCurve;

    public float baseMoveSpeed = 10f;

    public float baseSpinSpeed = 2f;

    public float maxRoundsForIncrementation = 10;

    private List<GameObject> spawnedObjects;

    void Start()
    {
        spawnedObjects = new List<GameObject>();
    }

    public void PrepareObjectSpawning(int amount)
    {
        List<GameObject> prefabs = GeneratePrefabList(amount);
        currentColorArray = GenerateColorList(amount);
        StartCoroutine(SpawnNewWave(prefabs, currentColorArray));
    }

    private List<Color> GenerateColorList(int amount)
    {
        List<Color> generatedColors = new List<Color>();
        for (int i = 0; i < amount; i++)
        {
            generatedColors.Add(colors[Random.Range(0, colors.Length)]);
        }
        return generatedColors;
    }

    private List<GameObject> GeneratePrefabList(int amount)
    {
        List<GameObject> prefabs = new List<GameObject>(amount);
        for (int i = 0; i < amount; i++)
        {
            prefabs.Add(shapes[Random.Range(0, shapes.Length)]);
        }
        return prefabs;
    }


    private IEnumerator SpawnNewWave(List<GameObject> prefabs, List<Color> colors)
    {
        int amountOfObjects = prefabs.Count;
        int index = 0;
        foreach (GameObject spawnable in prefabs)
        {
            Spawn(spawnable, colors[index++], IncrementSpeed(amountOfObjects), IncrementSpin(amountOfObjects));
            yield return new WaitForSeconds(spawningInterval);
        }
        Debug.Log("Spawning done waiting 2 seconds...");
        yield return new WaitForSeconds(1.4f);
        Debug.Log("Starting button handling!");

        foreach (GameObject gmo in spawnedObjects)
        {
            Destroy(gmo);
        }
        spawnedObjects.Clear();

        buttonHandler.EvaluateButtonsByColors(colors);
    }

    private void Spawn(GameObject demonSpawn, Color color, float speed, float spin)
    {
        Vector3 pos = new Vector3(
            Random.Range(spawnZone.localScale.x * -0.5f, spawnZone.localScale.x * 0.5f),
            Random.Range(spawnZone.localScale.y * -0.5f, spawnZone.localScale.y * 0.5f),
            spawnZone.position.z);

        var clone = Instantiate(demonSpawn, pos, Quaternion.identity);
        clone.GetComponent<ObjectMover>().SetObjectColor(color);
        clone.GetComponent<ObjectMover>().Launch(speed, spin);

        Debug.Log("Spawned object " + color + " speeds: " + speed + " " + spin);
        spawnedObjects.Add(clone);

    }


    private float IncrementSpeed(float amount)
    {
        Debug.Log(speedIncrementationCurve.Evaluate(amount / maxRoundsForIncrementation));
        return baseMoveSpeed * speedIncrementationCurve.Evaluate(amount / maxRoundsForIncrementation);
    }

    private float IncrementSpin(float amount)
    {
        return baseSpinSpeed * speedIncrementationCurve.Evaluate(amount / maxRoundsForIncrementation);
    }
}
