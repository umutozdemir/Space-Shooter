using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 3.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _fireRate = 0.5f;

    private float _canFire = 0f;

    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    private bool _isSpeedPowerupActive = false;
    private bool _isShieldPowerupActive = false;
    [SerializeField]
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject shieldVisualizer;
    [SerializeField]
    private int _score = 0;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject _leftEngine, _rightEngine;

    [SerializeField]
    private AudioClip _laserSound;
    private AudioSource _audioSource;


    // Start is called before the first frame update
    void Start()
    {
        // take position and assign it to start position.
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = gameObject.GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL");
        }
        if(_uiManager == null)
        {
            Debug.LogError("UIManager is NULL");
        }
        if(_audioSource == null)
        {
            Debug.LogError("Audio Source is null");
        }
        else
        {
            _audioSource.clip = _laserSound;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float forwardInput = Input.GetAxis("Mouse ScrollWheel");
        //  transform.Translate(Vector3.right * horizontalInput * _speed * Time.deltaTime);
        //  transform.Translate(Vector3.up * verticalInput * _speed * Time.deltaTime);
        //  transform.Translate(Vector3.forward * forwardInput * _speed * Time.deltaTime);
        // transform.Translate(new Vector3(horizontalInput, verticalInput, forwardInput) * _speed * Time.deltaTime);
        Vector3 direction = new Vector3(horizontalInput, verticalInput, forwardInput);
        transform.Translate(direction * _speed * Time.deltaTime);

        if (transform.position.y >= 6)
        {
            transform.position = new Vector3(transform.position.x, -6, transform.position.z);
        }
        else if (transform.position.y <= -3.9f)
        {
            transform.position = new Vector3(transform.position.x, -3.9f, transform.position.z);
        }
        // alternatively we can do this like transform.position = new Vector3d(transform.position.x,Mathf.Clamp(transform.position.y,6.-3.9f)
        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, transform.position.z);
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldPowerupActive)
        {
            _isShieldPowerupActive = false;
            shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;
        _uiManager.UpdateLivesImage(_lives);

        if(_lives == 2)
        {
           _leftEngine.SetActive(true);
        }
        else if(_lives == 1)
        {
           _rightEngine.SetActive(true);
        }
        else
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
            _uiManager.GameOver();
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotDownRoutine());
    }


    IEnumerator TripleShotDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public void SpeedPowerupActive()
    {
        _isSpeedPowerupActive = true;
        _speed = _speed * _speedMultiplier;
        StartCoroutine(SpeedPowerDownRoutine());
    }

    IEnumerator SpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedPowerupActive = false;
        _speed = _speed / _speedMultiplier;
    }

    public void ShieldPowerUpActive()
    {
        _isShieldPowerupActive = true;
        shieldVisualizer.SetActive(true);
    }

    public void AddScore(int point)
    {
        _score += point;
        _uiManager.UpdateScore(_score);
    }
   
}