using PathCreation;
using PathCreation.Examples;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static EnnemieCreator;
public class EnnemieCreator : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject vehicle;
    public GameObject[] prefabs;

    // Random Path Creator
    public GameObject[] prefab;
    public Int32 beatsBetweenSpawn;
    public Int32 spawnGroupSize;
    public Int32 beatsBetweenGroup;
    public Int32 beatsBeforeSpawning;

    // Saves Manager
    public string fileName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        SaveObject saveObject = new SaveObject();

        saveObject.ennemies = new();
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject prefab = transform.GetChild(i).gameObject;
            Ennemie ennemie = prefab.GetComponent<Ennemie>();

            if (ennemie != null)
            {
                EnnemieData item = new EnnemieData
                {
                    type = ennemie.type,
                    beat = ennemie.beat,
                    offset = ennemie.offset,
                    heightOffset = ennemie.heightOffset,
                    scale = ennemie.transform.localScale
                };

                saveObject.ennemies.Add(item);
            }
        }

        PathFollower saveVehicle = vehicle.GetComponent<PathFollower>();
        if (saveVehicle != null)
        {
            VehicleData vehicleData = new VehicleData
            {
                speed = saveVehicle.speed,
                widthOffset = saveVehicle.widthOffset,
                heightOffset = saveVehicle.heightOffset,
                endOfPathInstruction = saveVehicle.endOfPathInstruction
            };

            saveObject.vehicleData = vehicleData;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer != null )
        {
            BeatData beatData = new BeatData
            {
                audioClip = beatAnalyzer.musicClip,
                audioBpm = beatAnalyzer.songBpm,
                firstBeatOffset = beatAnalyzer.firstBeatOffset
            };

            saveObject.beatData = beatData;
        }

        string json = JsonUtility.ToJson(saveObject);
        File.WriteAllText(Application.dataPath + "/Saves/" + fileName + ".json", json);

    }

    public void Load()
    {
        string filePath = Application.dataPath + "/Saves/" + fileName + ".json";
        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);

            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

            GeneratePathFromData(saveObject);

        }
    }

    private void GeneratePathFromData(SaveObject saveObject)
    {
        VehicleData vehicleData = saveObject.vehicleData;
        BeatData beatData = saveObject.beatData;

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatData.audioBpm;

        float baseDistance = beatData.firstBeatOffset * vehicleData.speed;

        Dictionary<EnnemieType, GameObject> ennemieByType = new();
        foreach (GameObject coll in prefabs)
        {
            Ennemie ennemieScript = coll.GetComponent<Ennemie>();
            if (ennemieScript != null)
            {
                ennemieByType.Add(ennemieScript.type, coll);
            }
        }


        foreach (EnnemieData ennemieData in saveObject.ennemies)
        {
            float distance = baseDistance + secPerBeat * vehicleData.speed * ennemieData.beat;
            // Spawn the collectible
            Vector3 spawnPosition = new Vector3();
            Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, vehicleData.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);

            GameObject ennemieBase;
            ennemieByType.TryGetValue(ennemieData.type, out ennemieBase);

            if (ennemieBase != null)
            {
                GameObject ennemie = Instantiate(ennemieBase, spawnPosition, spawnRotation);
                ennemie.transform.localScale = ennemieData.scale;
                ennemie.transform.position = pathCreator.path.GetPointAtDistance(distance, vehicleData.endOfPathInstruction) + (ennemie.transform.right * ennemieData.offset) + (ennemie.transform.up * ennemieData.heightOffset);
                ennemie.transform.parent = transform;

                Ennemie ennemieScript = ennemie.GetComponent<Ennemie>();

                if (ennemieScript != null)
                {
                    ennemieScript.type = ennemieData.type;
                    ennemieScript.heightOffset = ennemieData.heightOffset;
                    ennemieScript.beat = ennemieData.beat;
                    ennemieScript.offset = ennemieData.offset;
                }
            }

        }
    }

    public void GenerateRandomPath()
    {
        PathFollower currentVehicle = vehicle.GetComponent<PathFollower>();
        if (currentVehicle == null)
        {
            return;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer == null)
        {
            
            return;
        }
        
        GameObject content = prefab[UnityEngine.Random.Range(0, prefab.Length)]; 

        Ennemie ennemie = content.GetComponent<Ennemie>();
        if (ennemie == null)
        {
            return;
        }

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        // Initialize base distance and offsets
        float distance = beatAnalyzer.firstBeatOffset * currentVehicle.speed;
        Int32 currentGroupSize = 0;
        float[] offsets = { -currentVehicle.widthOffset, 0f, currentVehicle.widthOffset };
        // Random offset
        float spawnOffset = offsets[UnityEngine.Random.Range(0, offsets.Length)];

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatAnalyzer.songBpm;
        Int32 totalBeats = (Int32)(beatAnalyzer.musicClip.length / secPerBeat);

        Int32 startingBeat = Mathf.Min(Mathf.Max(beatsBeforeSpawning, 0), totalBeats - 1);
        distance += secPerBeat * currentVehicle.speed * startingBeat;

        // Loop through all the beats of the music
        for (var i = startingBeat; i < totalBeats; i++)
        {
            content = prefab[UnityEngine.Random.Range(0, prefab.Length)];
            
            if (distance < pathCreator.path.length)
            {
                Debug.Log(distance);
                // Spawn collectible
                Vector3 spawnPosition = new Vector3();
                Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, currentVehicle.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);

                GameObject cube = Instantiate(content, spawnPosition, spawnRotation);
                cube.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                cube.transform.position = pathCreator.path.GetPointAtDistance(distance, currentVehicle.endOfPathInstruction) + (cube.transform.right * spawnOffset) + (cube.transform.up * ennemie.heightOffset);
                cube.transform.parent = transform;

                Ennemie ennemieSpawned = cube.GetComponent<Ennemie>();
                if (ennemieSpawned != null)
                {
                    ennemieSpawned.beat = i;
                    ennemieSpawned.offset = spawnOffset;
                    ennemieSpawned.heightOffset = ennemie.heightOffset;
                }

                currentGroupSize++;
            }


            Int32 multiplier = 1;

            // End of group
            if (currentGroupSize >= spawnGroupSize)
            {
                // Prepare gap between groups
                i = i + beatsBetweenGroup;
                multiplier = beatsBetweenGroup + 1;
                spawnOffset = offsets[UnityEngine.Random.Range(0, offsets.Length)];

                currentGroupSize = 0;
            }
            // Prepare gap between spawn if group is not finish
            else if (beatsBetweenSpawn > 0)
            {
                i = i + beatsBetweenSpawn;
                multiplier = beatsBetweenSpawn + 1;
            }

            distance += secPerBeat * currentVehicle.speed * multiplier;
        }
    }

    public void updateEnnemie(Ennemie inEnnemie)
    {
        PathFollower currentVehicle = vehicle.GetComponent<PathFollower>();
        if (currentVehicle == null)
        {
            return;
        }

        BeatAnalyzer beatAnalyzer = GetComponent<BeatAnalyzer>();
        if (beatAnalyzer == null)
        {
            return;
        }

        //Calculate the number of seconds in each beat
        float secPerBeat = 60f / beatAnalyzer.songBpm;

        float baseDistance = beatAnalyzer.firstBeatOffset * currentVehicle.speed;

        float distance = baseDistance + secPerBeat * currentVehicle.speed * inEnnemie.beat;

       
        // Spawn the collectible
        Quaternion spawnRotation = pathCreator.path.GetRotationAtDistance(distance, currentVehicle.endOfPathInstruction) * Quaternion.Euler(0, 0, 90);


        GameObject ennemie = inEnnemie.gameObject;  
        ennemie.transform.localScale = inEnnemie.transform.localScale;
        ennemie.transform.rotation = spawnRotation;
        ennemie.transform.position = pathCreator.path.GetPointAtDistance(distance, currentVehicle.endOfPathInstruction) + (ennemie.transform.right * inEnnemie.offset) + (ennemie.transform.up * inEnnemie.heightOffset);
        ennemie.transform.parent = transform;
    }

    [System.Serializable]
    public struct SaveObject
    {
        public VehicleData vehicleData;
        public BeatData beatData;
        public List<EnnemieData> ennemies;
    }

    [System.Serializable]
    public struct EnnemieData
    {
        public EnnemieType type;
        public Int32 beat;
        public float offset;
        public float heightOffset;
        public Vector3 scale;
    }

    [System.Serializable]
    public struct BeatData
    {
        public AudioClip audioClip;
        public float audioBpm;
        public float firstBeatOffset;
    }

    [System.Serializable]
    public struct VehicleData
    {
        public float speed;
        // Offset of the vehicle when switching lane
        public float widthOffset;
        // Height offset of spawing vehicle
        public float heightOffset;
        public EndOfPathInstruction endOfPathInstruction;
    }
}
