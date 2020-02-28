using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Astroid : MonoBehaviour
{

    [SerializeField]
    private float _rotateSpeed;
    [SerializeField]
    private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    [SerializeField]
    private AudioClip _explosionSound;
    private AudioSource _audioSource;
    // Start is called before the first frame update
    void Start()
    {
        _rotateSpeed = 50.0f;
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
        {
            Debug.LogError("Spawn Manager is null");
        }
        _audioSource = gameObject.GetComponent<AudioSource>();
        if(_audioSource == null)
        {
            Debug.LogError("Audio source is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Laser")
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject,0.25f);
            _audioSource.clip = _explosionSound;
            _audioSource.Play();
        }
    }
}
