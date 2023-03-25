using UnityEngine;

public class CountdownCameras : MonoBehaviour
{
    public Camera[] _cameras;
    private int cameraIndex = 0;

    public GameObject player;

    private void Update()
    {
        if (!GameController.instance) return;

        if (!player)
            player = GameController.instance.CurrentPlayer;
    }

    public Camera SwitchToNextCamera()
    {
        if (player)
            transform.SetPositionAndRotation(player.transform.position, player.transform.rotation);

        if (cameraIndex < _cameras.Length)
        {
            DisableAllCountdownCameras();
            _cameras[cameraIndex].enabled = true;
            _cameras[cameraIndex].gameObject.SetActive(true);
            cameraIndex++;
            return _cameras[cameraIndex - 1];
        }
        return null;
    }

    public void DisableAllCountdownCameras()
    {
        foreach (Camera cam in _cameras)
        {
            cam.enabled = false;
            cam.gameObject.SetActive(false);
        }
    }
}