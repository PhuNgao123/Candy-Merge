using UnityEngine;

public class Dropper : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotateSpeed = 90f;
    public float dropDelay = 1f;
    public GameObject[] candyPrefabs;
    private GameObject currentCandy = null;
    private float nextSpawnTime = 0f;

    AudioManager audioManager;

    public void Awake(){
         audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        candyPrefabs= Resources.LoadAll<GameObject>("Prefab");

        if (candyPrefabs == null || candyPrefabs.Length == 0){
            Debug.LogWarning("Can not load Candy prefabs!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        DropperMove();
        DropCandy();
        
        // Spawn after delay
        if (Time.time >= nextSpawnTime && currentCandy == null)
        {
            SpawnCandy();
            nextSpawnTime = Time.time + dropDelay;
        }
    }

    public void DropperMove(){
        if(Input.GetKey(KeyCode.A)){
            Vector3 pos = transform.position;
            pos.x -= moveSpeed * Time.deltaTime;
            transform.position= pos;
            Debug.Log("Pressing A");
        }
        if(Input.GetKey(KeyCode.D)){
            Vector3 pos = transform.position;
            pos.x += moveSpeed * Time.deltaTime;
            transform.position= pos;
            Debug.Log("Pressing D");
        }
        if(Input.GetKey(KeyCode.Q)){  
            transform.Rotate(0f,0f, rotateSpeed * Time.deltaTime);
            Debug.Log("Pressing Q");
        }
        if(Input.GetKey(KeyCode.E)){
            transform.Rotate(0f,0f, -rotateSpeed * Time.deltaTime);
            Debug.Log("Pressing E");
        }
        }

    public void DropCandy(){
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.S)){
            if (currentCandy == null) return;  
            
            //Separate candy from Dropper
            currentCandy.transform.SetParent(null);
            audioManager.PlaySFX(audioManager.candyDrop);
            
            // Turn on candy physic
            Rigidbody2D rb = currentCandy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            
            //Reset current candy
            currentCandy = null;
            
            // Reset time
            nextSpawnTime = Time.time + dropDelay;
        }
    }

    public void SpawnCandy(){
        if(currentCandy != null) return; //Don't spawn if current candy is existing

        //Check if there is no prefab
        if(candyPrefabs == null || candyPrefabs.Length == 0){ 
            Debug.LogWarning("Candy Prefab not found");
            return;
        }

        //Spawn randomCandy
        GameObject prefab = RandomCandy();
        if (prefab == null)
        {
            Debug.LogWarning("RandomCandy returned null!");
            return;
        }

        Vector3 spawnPos= transform.position;
        Quaternion spawnRot = transform.rotation;

        GameObject newCandy= Instantiate(prefab, spawnPos, spawnRot);
        newCandy.transform.SetParent(transform);  //Set candy as Dropper child

        currentCandy= newCandy;
        audioManager.PlaySFX(audioManager.candySpawn);

        //Disable physic
        Rigidbody2D rb = newCandy.GetComponent<Rigidbody2D>();
        if(rb != null){
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public GameObject RandomCandy(){
        if (candyPrefabs == null || candyPrefabs.Length == 0 ){
            return null;
        }
        
        int number = Random.Range(0,100);
        float cumulative =0f;
        //Random algorithm
        for(int i = 0; i<candyPrefabs.Length; i++){
            float weight = 50f/Mathf.Pow(2f, i);
            int round= (int)Mathf.Ceil(weight);

            cumulative += round;

            if (number < cumulative){
                return candyPrefabs[i];
            }
        }
        return candyPrefabs[0];
    }
}