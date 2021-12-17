using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    //public float horizontalFOV = 40f;

    public Transform topWall;
    public Transform bottomWall;
    public Transform rightWall;
    public Transform leftWall;

    [Space]
    public float coneOffset;
    public Transform rightCone;
    public Transform leftCone;

    [Space]
    public float objectOffset;
    public Transform rightExplosionObject;
    public Transform leftExplosionObject;

    [Space]
    public float spawnerOffset;
    public Transform rightSpawner;
    public Transform leftSpawner;

    Camera _camera;
    Transform myTransform;

    void Start()
    {
        //Camera.main.fieldOfView = CalcVertivalFOV(horizontalFOV, Camera.main.aspect);

        _camera = Camera.main;
        myTransform = transform;

        SetWallsPositionToFitScreen();
    }

    void SetWallsPositionToFitScreen()
    {
        Vector3 verticalWallPosition = _camera.ScreenToWorldPoint(new Vector3((Screen.width / 2), Screen.height, (-myTransform.position.z)));
        Vector3 horizontalWallPosition = _camera.ScreenToWorldPoint(new Vector3(Screen.width, (Screen.height / 2), (-myTransform.position.z)));

        topWall.position = new Vector3(verticalWallPosition.x, (verticalWallPosition.y + 1), verticalWallPosition.z);

        bottomWall.position = new Vector3(verticalWallPosition.x, -4.5f, verticalWallPosition.z);

        rightWall.position = new Vector3((horizontalWallPosition.x + 0.5f), horizontalWallPosition.y, horizontalWallPosition.z);

        leftWall.position = new Vector3((-horizontalWallPosition.x - 0.5f), horizontalWallPosition.y, horizontalWallPosition.z);

        SetConesAndExplosionObjectsPosition();
    }

    void SetConesAndExplosionObjectsPosition()
    {
        rightCone.position = new Vector3((rightWall.position.x - coneOffset), rightCone.position.y, rightCone.position.z);
        leftCone.position = new Vector3((leftWall.position.x + coneOffset), leftCone.position.y, leftCone.position.z);

        rightExplosionObject.position = new Vector3((rightWall.position.x - objectOffset), rightExplosionObject.position.y, rightExplosionObject.position.z);
        leftExplosionObject.position = new Vector3((leftWall.position.x + objectOffset), leftExplosionObject.position.y, leftExplosionObject.position.z);

        rightSpawner.position = new Vector3((rightWall.position.x - spawnerOffset), rightSpawner.position.y, rightSpawner.position.z);
        leftSpawner.position = new Vector3((leftWall.position.x + spawnerOffset), leftSpawner.position.y, leftSpawner.position.z);
    }

    //private float CalcVertivalFOV(float hFOVInDeg, float aspectRatio)
    //{
    //    float hFOVInRads = hFOVInDeg * Mathf.Deg2Rad;
    //    float vFOVInRads = 2 * Mathf.Atan(Mathf.Tan(hFOVInRads / 2) / aspectRatio);
    //    float vFOV = vFOVInRads * Mathf.Rad2Deg;
    //    return vFOV;
    //}
}