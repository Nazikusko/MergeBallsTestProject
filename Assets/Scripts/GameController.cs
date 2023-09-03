using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class GameController : MonoBehaviour
{
    private const float BALL_VELOCITY_MULTIPLAYER = 1.5f;
    private const float SIMULATE_PHYSICS_DELTA = 0.1f;

    [SerializeField] private InputController _inputController;
    [SerializeField] private Ball[] _ballsPrefabs;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _debugPointPrefab;
    [SerializeField] private LoadPoint _loadPoint;
    [SerializeField] private Transform _ballsParent;
    [SerializeField] private PopUpWindow _popUpWindow;

    private Ball _loadedBall;
    private List<Ball> _ballsInCup = new List<Ball>();

    void Awake()
    {
        _inputController.OnTapOnScreen += TapOnScreenHolder;
        _inputController.OnTouchOnScreenEnd += TouchOnScreenEnd;
        _loadPoint.OnLoadZoneClear += LoadRandomBall;
        _loadPoint.OnGameOver += GameOver;
        Ball.OnBallsMatch += BallsMatch;
        _popUpWindow.Close();
    }

    void Start()
    {
        RestartGame();
    }

    private void TouchOnScreenEnd(Vector2 position)
    {
        ThrowBall(position);
    }

    private void TapOnScreenHolder(Vector2 tapPosition)
    {
        ThrowBall(tapPosition);
    }

    private void ThrowBall(Vector2 tapPosition)
    {
        if (_loadedBall == null)
            return;

        var worldPoint = _camera.ScreenToWorldPoint(tapPosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, float.PositiveInfinity,
            LayerMask.GetMask("CupCollider"));

        if (hit.collider != null)
        {
            var velocityVector = hit.point - new Vector2(_loadPoint.transform.position.x, _loadPoint.transform.position.y);
            _loadedBall.BallRigidBody.isKinematic = false;
            _loadedBall.BallRigidBody.velocity = velocityVector * BALL_VELOCITY_MULTIPLAYER;
            _ballsInCup.Add(_loadedBall);
            _loadedBall = null;
            _loadPoint.BallThrow();
        }
    }

    private void LoadRandomBall()
    {
        int randomRange = _ballsInCup.Max(b => b.BallNumber);
        _loadedBall = Instantiate(_ballsPrefabs[Random.Range(0, randomRange > 7 ? 7 : randomRange)],
            _loadPoint.transform.position, Quaternion.identity, _ballsParent);

        _loadedBall.BallRigidBody.isKinematic = true;
        _loadPoint.BallLoaded();
    }

    void SpawnRandomBallsInCup()
    {
        Physics2D.simulationMode = SimulationMode2D.Script;

        int ballsCount = Random.Range(1, 5);

        for (int i = 0; i < ballsCount; i++)
        {
            int randomBallNumber;
            do
            {
                randomBallNumber = Random.Range(0, 4);
            } while (_ballsInCup.Exists(b => b.BallNumber - 1 == randomBallNumber));

            var ball = Instantiate(_ballsPrefabs[randomBallNumber], _loadPoint.transform.position,
                Quaternion.identity, _ballsParent);
            ball.BallRigidBody.velocity = (new Vector2(Random.Range(-0.35f, 0.35f), 0)
                                           - ToVector2(_loadPoint.transform.position)) * BALL_VELOCITY_MULTIPLAYER;
            _ballsInCup.Add(ball);

            SimulatePhysics(4f);
        }
        Physics2D.simulationMode = SimulationMode2D.FixedUpdate;

        _ballsInCup.ForEach(b => b.BallRigidBody.Sleep());
    }

    private static void SimulatePhysics(float seconds)
    {
        for (float j = 0; j < seconds; j += SIMULATE_PHYSICS_DELTA)
        {
            Physics2D.Simulate(SIMULATE_PHYSICS_DELTA);
        }
    }

    private void BallsMatch(Ball ball1, Ball ball2, Vector2 point)
    {
        int ballNumber = ball1.BallNumber;

        if (ballNumber == 9) GameWin();

        if (ball1.BallNumber != 10)
        {
            _ballsInCup.Remove(ball1);
            ball1.DestroyBall();
        }

        if (ball2.BallNumber != 10)
        {
            _ballsInCup.Remove(ball2);
            ball2.DestroyBall();
        }

        if (ball1.BallNumber == 10 || ball2.BallNumber == 10) return;

        var ball = Instantiate(_ballsPrefabs[ballNumber], point,
            Quaternion.identity, _ballsParent);
        ball.SpawnedNewBall();
        _ballsInCup.Add(ball);
    }

    private void GameWin()
    {
        _inputController.IsGameInputActive = false;
        _popUpWindow.SetOkButtonText("Restart").
            SetCancelButtonText("Quit").
            ShowWindow("You Win!\nDo you want to restart the game?", RestartGame, () => Application.Quit());
    }

    private void GameOver()
    {
        _inputController.IsGameInputActive = false;
        _popUpWindow.SetOkButtonText("Restart").
            SetCancelButtonText("Quit").
            ShowWindow("Game Over\nDo you want to restart the game?", RestartGame, () => Application.Quit());
    }

    private void RestartGame()
    {
        StopAllCoroutines();
        _ballsInCup.ForEach(b => Destroy(b.gameObject));
        _ballsInCup.Clear();

        if (_loadedBall != null) Destroy(_loadedBall.gameObject);

        SpawnRandomBallsInCup();
        LoadRandomBall();
        _loadPoint.StartScanLoadPoint();
        DOVirtual.DelayedCall(0.5f, () => _inputController.IsGameInputActive = true);
    }

    private Vector2 ToVector2(Vector3 vector3) => new Vector2(vector3.x, vector3.y);

    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Q))
            GameOver();

        if (Input.GetKeyUp(KeyCode.W))
        {
            var ball = Instantiate(_ballsPrefabs[9], Vector3.zero, Quaternion.identity, _ballsParent);
            _ballsInCup.Add(ball);
            ball.SpawnedNewBall();
        }
#endif
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
    }
}
