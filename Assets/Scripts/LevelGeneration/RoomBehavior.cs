using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomBehavior : MonoBehaviour
{
    [SerializeField] GameObject[] walls;
    [SerializeField] GameObject[] doors;
    [SerializeField] public bool[] status = new bool[4]; //up down right left
    
    //fuer astern nachbarstuff
    public DungeonCreator.Cell cell;

    [SerializeField] public bool bChildrenOfChildren = true;

    [SerializeField] GameObject[] specialObjects;
    [SerializeField] public GameObject DebugAstar;
    [SerializeField] GameObject[] brokenWalls;
    [SerializeField] public GameObject[] pressurePlates;

    [SerializeField] GameObject keyGO;
    [SerializeField] GameObject kugelGO;

    NavMeshSurface[] Surfaces;
    [SerializeField] public bool bCanHaveKey = true;
    [SerializeField] public bool bCanHaveKugel = true;
    [SerializeField] GameObject[] EnemySpawns;
    GameObject[] activeSpawns;
    [SerializeField] public bool bCanEnemys= true;
    
    //fuer blaues licht
    Enemy[] enemyArray;
    bool bLightBlue = false;
    //private NavMeshBuilder _navMeshBuilder;
    GameObject[] LightSourcesNearDebugAstar;
    GameObject[] LightsInRoom;
    [SerializeField] private GameObject BlueTorchPrefab;

    // Mischt eine Liste
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    private void Awake() //awake sonst klappt spawnen nicht ?XD
    {
        //foreach (GameObject obj in brokenWalls)
        //{
        //    obj.gameObject.SetActive(false);   
        //}
        //Surfaces = this.GetComponentsInChildren<NavMeshSurface>();
        //for (int i = 0; i < Surfaces.Length; i++)
        //{
        //    Surfaces[i].BuildNavMesh();
        //}
        foreach (GameObject obj in specialObjects)
        {
            //erstmal alle deaktivieren
          
            if(bChildrenOfChildren)
                DeactivateAllChildren(obj.transform);
            else 
            {
                foreach (Transform child in obj.transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
            activateRndObjects(obj);
        }

        foreach (GameObject obj in EnemySpawns)
        {
            //erstmal alle deaktivieren

            foreach (Transform child in obj.transform)
            {
            child.gameObject.SetActive(false);
            }
            activateRandomSpawns(obj);
        }
         //enemyArray = new Enemy[activeSpawns.Length];


    }

    private void Update()
    {
        //if dragon hat this.DebugAstarTarget angegriffen -> ComputeNearLightSources()



        if (bLightBlue == false)
        {
            //zum testen 
            if(DebugAstar != null)
            {

            //ComputeNearLightSources();
            //StartCoroutine(ChangeToBlueLight(this.gameObject));
            }
            //für gegner
            //if (activeSpawns != null)
            //{
               
            //    int j = 0;
            //    for (int i = 0; i < enemyArray.Length; i++)
            //        if (enemyArray[i].IsDestroyed())
            //            j++;
            //    if (j == enemyArray.Length)
            //    {
            //        Debug.LogWarning("alle enemys in dem raum dead");
            //        //ComputeNearLightSources();
            //    }

            //}
        }


    }
    //Blaues licht machen
    IEnumerator ChangeToBlueLight()
    {
        yield return new WaitForSeconds(1f); // Wartezeit von einer Sekunde
        for(int i = 0; i < LightSourcesNearDebugAstar.Length; i++)
        {
            //LightSourcesNearDebugAstar[i].GetComponentInChildren<Light>().color = Color.cyan;
            
            LightSourcesNearDebugAstar[i].SetActive(false);
            //instantiate new Prefab
            Instantiate(BlueTorchPrefab, LightSourcesNearDebugAstar[i].transform.position, LightSourcesNearDebugAstar[i].transform.rotation, this.transform);
            Destroy(LightSourcesNearDebugAstar[i]);
        }

        bLightBlue = true;

    }

    public void ComputeNearLightSources() 
    {
        //LightsInRoom = GetComponentsInChildren<Light>();
        LightsInRoom = GameObject.FindGameObjectsWithTag("DoorTorch"); //rechenaufwendig weil alle gefunden -> nur kinder -> dk wie
        //LightsInRoom = gameObject.GetComponentsInChildren<GameObject>().Where(go => go.CompareTag("DoorTorch")).ToArray();
        LightSourcesNearDebugAstar = new GameObject[2];
        float smallestDistance = 1000;
        float secondSmallestDistance = 1000;
        int smallestIndex = 0;
        int smallestIndex2nd= 0;
        for (int j = 0; j < LightsInRoom.Length; j++)
        {
            var distance = Vector3.Distance(DebugAstar.transform.position, LightsInRoom[j].transform.position);
            //if (distance < smallestDistance)
            //{
            //    smallestIndex = j; 
            //    smallestDistance = Distance;
            //}
            if (distance < smallestDistance)
            {
                smallestIndex2nd = smallestIndex;
                secondSmallestDistance = smallestDistance;

                smallestIndex = j;
                smallestDistance = distance;
            }
            else if (distance < secondSmallestDistance)
            {
                smallestIndex2nd = j;
                secondSmallestDistance = distance;
            }
        }

        LightSourcesNearDebugAstar[0] = LightsInRoom[smallestIndex].gameObject;
        LightSourcesNearDebugAstar[1] = LightsInRoom[smallestIndex2nd].gameObject;
        //tag für den drachen
        //LightSourcesNearDebugAstar[0].gameObject.tag = "WayPoint";
        //LightSourcesNearDebugAstar[1].gameObject.tag = "WayPoint";
        //nur debug hier schon starten -> sonst erst wenn drache angreift

        for (int i = 0; i < LightSourcesNearDebugAstar.Length; i++)
        {
            //LightSourcesNearDebugAstar[i].GetComponentInChildren<Light>().color = Color.cyan;
            
            LightSourcesNearDebugAstar[i].SetActive(false);
            //instantiate new Prefab
            Instantiate(BlueTorchPrefab, LightSourcesNearDebugAstar[i].transform.position, LightSourcesNearDebugAstar[i].transform.rotation, this.transform);
            Destroy(LightSourcesNearDebugAstar[i]);
        }
        
        // StartCoroutine(ChangeToBlueLight());
    }

    public void MonsterHaveBeenSpawned()
    {
        //for (int i = 0; i < activeSpawns.Length; i++)
        //{
        //    enemyArray[i] = activeSpawns[i].GetComponentInChildren<Enemy>(true);
        //}
    }
    public void UpdateRoom(bool[] newStatus)
    {
        
        status = newStatus;
        for (int i = 0; i < newStatus.Length; i++)
        {
            doors[i].SetActive(newStatus[i]);
            walls[i].SetActive(!newStatus[i]);
        }

        if(pressurePlates.Length == 2)
        {
            if (status[0] == true)
                pressurePlates[0].SetActive(true);
            else
                pressurePlates[0].SetActive(false);
           
            if (status[3] == true)
                pressurePlates[1].SetActive(true);
            else
                pressurePlates[1].SetActive(false);


        }
    }



    private void activateRandomSpawns(GameObject gameObject)
    {
        if (gameObject.transform.childCount > 0)
        {
            int childCount = gameObject.transform.childCount;
            if (childCount == 1)
            {
                activeSpawns = new GameObject[0];
                return;
            }
            int activeChildCount = UnityEngine.Random.Range(1, 4);
            activeSpawns = new GameObject[activeChildCount];
            // Erstelle eine Liste der Indizes aller Kindobjekte
            List<int> childIndices = new List<int>();
            for (int i = 0; i < childCount; i++)
            {
                childIndices.Add(i);
            }

            // Mische die Liste der Kindindizes
            ShuffleList(childIndices);

            // Aktiviere eine zufaellige Anzahl von Kindobjekten
            for (int i = 0; i < activeChildCount; i++)
            {
                int randomIndex = childIndices[i];

                // Aktiviere das Kindobjekt anhand des zufaelligen Indexes
                Transform child = gameObject.transform.GetChild(randomIndex);
                child.gameObject.SetActive(true);
                activeSpawns[i] = child.gameObject;

            }
        }
    }

    public GameObject[] getactiveSpawnableChildrenPositions()
    {
        if (activeSpawns != null)
        {

            return activeSpawns;
        }
        else
            return EnemySpawns;
    }
    //für raum reset/change wenn zb bossraum ausgetauscht wurde.
    public void resetEnemySpawn()
    {
        activeSpawns = null;
    }
    private void activateRndObjects(GameObject gameObject)
    {

        if (gameObject.transform.childCount > 0)
        {
            //int minActiveChildren = 3;
            //int maxActiveChildren = 7;
            int childCount = gameObject.transform.childCount;
            //int activeChildCount = Random.Range(minActiveChildren, Mathf.Min(maxActiveChildren, childCount) + 1);
            int activeChildCount = Random.Range(1, childCount);

            // Erstelle eine Liste der Indizes aller Kindobjekte
            List<int> childIndices = new List<int>();
            for (int i = 0; i < childCount; i++)
            {
                childIndices.Add(i);
            }

            // Mische die Liste der Kindindizes
            ShuffleList(childIndices);

            // Aktiviere eine zufaellige Anzahl von Kindobjekten
            for (int i = 0; i < activeChildCount; i++)
            {
                int randomIndex = childIndices[i];

                // Aktiviere das Kindobjekt anhand des zufaelligen Indexes
                Transform child = gameObject.transform.GetChild(randomIndex);
                child.gameObject.SetActive(true);

                // Überprüfe, ob das Kindobjekt weitere Kinder hat wenn gefragt
                if (bChildrenOfChildren)
                {

                    if (child.childCount > 0)
                    {
                        // Aktiviere die weiteren Kinder des Kindobjekts
                        activateRndObjects(child.gameObject);
                    }
                }

            }

        }
    }

    private void DeactivateAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // Deaktiviere das Kindobjekt
            child.gameObject.SetActive(false);

            // Überprüfe, ob das Kindobjekt weitere Kinder hat
            if (child.childCount > 0)
            {
                // Deaktiviere auch die untergeordneten Kinder rekursiv
                DeactivateAllChildren(child);
            }
        }
    }
    public void setKeyActive(bool b)
    {
        keyGO.SetActive(b);
        if (b == true)
            keyGO.tag = "activeKey";
        else
            keyGO.tag = "inactiveKey";
    }

    public void setBrokenWalls(bool b)
    {

        int i = 0;
        foreach (GameObject obj in brokenWalls)
        {

            if (status[i] == true)
            {
                obj.gameObject.SetActive(b);
                //return; //nur eine tür
            }
            else
                obj.gameObject.SetActive(false);


            i++;
        }
    }
    public void setKugelActive(bool b)
    {
        kugelGO.SetActive(b);
    }
    public void activateDebugAStar() 
    {
        DebugAstar.gameObject.SetActive(true);
    }
    public void deactivateDebugAStar()
    {
        DebugAstar.gameObject.SetActive(false);
    }
    public void moveObjectToNeighbour()
    {
    }
}

