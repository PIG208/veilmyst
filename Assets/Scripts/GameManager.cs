using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    // Camera related vars
    public Camera MainCam;
    public Camera MenuCam;
    PixelPerfectCamera PPCam;
    public Vector3 CPPos;
    public Vector2 CamPos { get; set; }
    public Vector3 CamResetPos;
    public int PixelPerUnit = 16;
    public CheckPoint ActiveSpawnPoint { get; private set; }
    public PhaseManager PhaseManager;

    private bool camLocked = false;

    private float currentPixelPerUnit = 16;

    public float speed;

    // Player-related vars
    public GameObject player;
    public Movement playMovScript;

    // Misc
    public int DeathCount;
    public GameObject EffectsPrefab;

    [Tooltip("UI")]
    public Menu PauseMenu;
    public InputActionReference PauseAction;

    public void MoveCam(Vector3 pos, bool locked = false)
    {
        if (camLocked) return;
        camLocked = locked;
        CamPos = pos;
    }

    public void SetSpawn(CheckPoint checkPoint)
    {
        playMovScript.spawnLocation = checkPoint.transform.position;
        ActiveSpawnPoint = checkPoint;
        CamResetPos = checkPoint.CamPos;
    }

    private void Awake()
    {
        MainCam = Camera.main;
        CamResetPos = MainCam.transform.position;
        PPCam = MainCam.GetComponent<PixelPerfectCamera>();
        currentPixelPerUnit = PixelPerUnit = PPCam.assetsPPU;
        DeathCount = 0;

        player = GameObject.FindGameObjectWithTag("Player");
        playMovScript = player.GetComponent<Movement>();
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector2)MainCam.transform.position != CamPos)
        {
            var pos = Vector3.Lerp(MainCam.transform.position, CamPos, speed * Time.deltaTime);
            pos.z = MainCam.transform.position.z;
            MainCam.transform.position = pos;
        }

        if (PixelPerUnit != (int)currentPixelPerUnit)
        {
            currentPixelPerUnit = Mathf.Lerp(currentPixelPerUnit, PixelPerUnit, speed * Time.deltaTime);
            PPCam.assetsPPU = (int)currentPixelPerUnit;
        }

        if (PauseAction.action.triggered)
        {
            if (Menu.FocusedMenu == null) PauseMenu.SetShow(true);
            else Menu.FocusedMenu?.Toggle();
        }

        MainCam.gameObject.SetActive(Menu.FocusedMenu == null);
        MenuCam.gameObject.SetActive(Menu.FocusedMenu != null);
    }
}
