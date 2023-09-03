using UnityEngine;

public class AimingPoints : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private Camera _camera;
    [SerializeField] private LoadPoint _loadPoint;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameController _gameController;

    void Awake()
    {
        _inputController.OnTouchOnScreenMoved += UpdateAimingPoints;
        _inputController.OnTouchOnScreenEnd += OffAimingPoints;
        gameObject.SetActive(false);
        transform.position = _loadPoint.transform.position;
    }

    private void OffAimingPoints(Vector2 _)
    {
        gameObject.SetActive(false);
    }

    private void UpdateAimingPoints(Vector2 position)
    {
        if (!_loadPoint.IsBallLoaded)
        {
            gameObject.SetActive(false);
            return;
        }

        var worldFingerPosition = _camera.ScreenToWorldPoint(SetCameraDistance(position));

        RaycastHit2D hit = Physics2D.Raycast(worldFingerPosition, Vector2.zero, float.PositiveInfinity,
            LayerMask.GetMask("CupCollider"));

        if (hit.collider != null)
        {
            gameObject.SetActive(true);
            var alignVector = transform.position - worldFingerPosition;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, alignVector);
            _spriteRenderer.size = new Vector3(_spriteRenderer.size.x, Vector3.Distance(transform.position, worldFingerPosition) * 1.05f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private Vector3 SetCameraDistance(Vector2 position)
    {
        return new Vector3(position.x, position.y, _camera.transform.position.z * -1);
    }
}
