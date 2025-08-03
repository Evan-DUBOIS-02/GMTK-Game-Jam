using UnityEngine;
using Random = UnityEngine.Random;

public class SeedManager : MonoBehaviour
{
    public static SeedManager Instance;
    public int _seed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    public void GenerateRandomizer(int seed)
    {
        Random.InitState(seed);
        _seed = seed;
    }
}