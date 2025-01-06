using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 15f;
    private float turnSpeed = 90f;
    private float fuelConsumption = 0.5f;

    private float horizontalInput;
    private float verticalInput;
    private Rigidbody playerRb;
    private PlayerFuel playerFuel;
    [SerializeField] TextMeshProUGUI speedText;

    [SerializeField] AudioSource engineAudio;

    private void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerFuel = GetComponent<PlayerFuel>();
        engineAudio.loop = true;
        engineAudio.Play(); // Looping basic engine sound, will change it to a more complex AND make it differ between levels
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        playerRb.AddRelativeForce(Vector3.forward * moveSpeed * verticalInput);
        transform.Rotate(Vector3.up, Time.deltaTime * turnSpeed * horizontalInput);

        speedText.text = string.Format("Speed: {0:#.00}", playerRb.velocity.magnitude);

        playerFuel.AddFuel(-fuelConsumption * Time.deltaTime * playerRb.velocity.magnitude);

        UpdateEngineSound(playerRb.velocity.magnitude);
    }

    private void UpdateEngineSound(float speed)
    {
        float maxSpeed = moveSpeed;
        float pitch = Mathf.Lerp(0.5f, 2.0f, speed / maxSpeed);
        float volume = Mathf.Lerp(0.1f, 1.0f, speed / maxSpeed);

        engineAudio.pitch = pitch;
        engineAudio.volume = volume;
    }
}