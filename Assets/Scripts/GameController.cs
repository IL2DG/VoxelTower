using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePaceSpeed = 0.5f;
    public Transform cubeToPlace;
    private int prevCountMaxHorizontal;
    public GameObject allCubes, vfx;
    public GameObject[] cubesToCreate;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;
    private float camMoveToYPosition, camMoveSpeed = 2f;
    public Text scoreText, bestScoreText;
    public GameObject Score;
    private bool IsLose, firstCube;
    public GameObject restartButton;

    public int deathCount = 0;

    public Material skyOne, skyTwo, skyThree;

    private List<GameObject> posibleCubesToCreate = new List<GameObject>();

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1),
        new Vector3(1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(1, 0, -1),
    };

    private Transform mainCam;
    private Coroutine showCubePlace;

    private void Start()
    {
        if (PlayerPrefs.GetInt("score") < 5)
            posibleCubesToCreate.Add(cubesToCreate[0]);
        else if (PlayerPrefs.GetInt("score") < 10)
            AddPossibleCubes(2);
        else if (PlayerPrefs.GetInt("score") < 15)
            AddPossibleCubes(3);
        else if (PlayerPrefs.GetInt("score") < 25)
            AddPossibleCubes(4);
        else if (PlayerPrefs.GetInt("score") < 40)
            AddPossibleCubes(5);
        else if (PlayerPrefs.GetInt("score") < 60)
            AddPossibleCubes(6);
        else if (PlayerPrefs.GetInt("score") < 80)
            AddPossibleCubes(7);
        else if (PlayerPrefs.GetInt("score") < 100)
            AddPossibleCubes(8);
        else if (PlayerPrefs.GetInt("score") < 150)
            AddPossibleCubes(9);
        else
            AddPossibleCubes(10);

        if (Advertisement.isSupported)
            Advertisement.Initialize("4109925", false);

        bestScoreText.text = "<size=40>Best score:</size>" + PlayerPrefs.GetInt("score");
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + nowCube.y - 1f;
        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
        deathCount = PlayerPrefs.GetInt("deathcount");
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if (!UNITY_EDITOR || !UNITY_ANDROID)
            if (Input.GetTouch(0).phase != TouchPhase.Began) return;
#endif
            if (!firstCube)
            {
                firstCube = true;
                foreach (GameObject obj in canvasStartPage)
                    Destroy(obj);
            }

            GameObject createCube = null;
            if (posibleCubesToCreate.Count == 1)
                createCube = posibleCubesToCreate[0];
            else
                createCube = posibleCubesToCreate[UnityEngine.Random.Range(0, posibleCubesToCreate.Count)];

            Score.SetActive(true);
            GameObject newCube = Instantiate(createCube, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPositions.Add(nowCube.GetVector());

            GameObject newVfx = Instantiate(vfx, cubeToPlace.position, Quaternion.identity);
            Destroy(newVfx, 1.5f);

            if (PlayerPrefs.GetString("music") != "No")
                GetComponent<AudioSource>().Play();

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;
            SpawnPositions();
            MoveCamera();
        }

        if(!IsLose && allCubesRb.velocity.magnitude > 0.1f)
        {
            deathCount++;
            PlayerPrefs.SetInt("deathcount", deathCount);
            if (Advertisement.IsReady() && deathCount % 3 == 0)
            {
                Advertisement.Show();
            }
            Destroy(cubeToPlace.gameObject);
            restartButton.SetActive(true);
            IsLose = true;
            StopCoroutine(showCubePlace);
        }

        mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition, new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z), camMoveSpeed * Time.deltaTime);
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(cubeChangePaceSpeed);
        }
    }

    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));
        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0) return false; 

        foreach(Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z) return false;
        }
        return true;
    }

    private void MoveCamera()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;

        foreach(Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX)
                maxX = Convert.ToInt32(pos.x);

            if (Convert.ToInt32(pos.y) > maxY)
                maxY = Convert.ToInt32(pos.y);

            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ)
                maxZ = Convert.ToInt32(pos.z);
        }

        if (PlayerPrefs.GetInt("score") < maxY - 1)
            PlayerPrefs.SetInt("score", maxY - 1);

        bestScoreText.text = "<size=40>Best score:</size>" + PlayerPrefs.GetInt("score");

        camMoveToYPosition = 5.9f + nowCube.y - 1f;
        scoreText.text = "" + (maxY - 1);
        maxHor = maxX > maxZ ? maxX : maxZ;
        if(maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor)
        {
            mainCam.localPosition += new Vector3(0, 0, -2.5f);
            prevCountMaxHorizontal = maxHor;
        }

        if (maxY >= 100)
            RenderSettings.skybox = skyThree;
        else if (maxY >= 50)
            RenderSettings.skybox = skyTwo;
        else
            RenderSettings.skybox = skyOne;
    }

    private void AddPossibleCubes(int till)
    {
        for (int i = 0; i < till; i++)
            posibleCubesToCreate.Add(cubesToCreate[i]);
    }

}

struct CubePos
{
    public int x, y, z;
    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector()
    {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}