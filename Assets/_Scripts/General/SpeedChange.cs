using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpeedChange : MonoBehaviour {

    public enum TerrainType {regular, water, path};
    private GameObject _player;
    private Movement _playerMovement; //movement script
    private Dash _playerDash; //dash script
    private float _regularSpeed;
    private float _regularDashSpeed;

    public TerrainType _terrainType = TerrainType.regular; //default
    public float _slowdownFactor = 0.5f;
    public float _accFactor = 1.2f;

    public void Start()
    {
        _player = GameObject.Find("Player");
        _playerMovement = _player.GetComponent<Movement>();
        _playerDash = _player.GetComponent<Dash>();
        //I chaged that to a special speed multipicator, as mSpeed is needed for other variables aswell (rain, blzzard)
        _regularSpeed = _playerMovement.speedMultiplicator;
        _regularDashSpeed = _playerDash.dashSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == _player){
            if (_terrainType == TerrainType.water) {
                _playerMovement.speedMultiplicator = _regularSpeed * _slowdownFactor;
                _playerDash.dashSpeed = _regularDashSpeed * _slowdownFactor;
            } else if (_terrainType == TerrainType.path) {
                _playerMovement.speedMultiplicator = _regularSpeed * _accFactor;
                _playerDash.dashSpeed = _regularDashSpeed * _accFactor;
                //TODO: ggf speed up acceleration?
            } else {
                _playerMovement.speedMultiplicator = _regularSpeed;
                _playerDash.dashSpeed = _regularDashSpeed;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == _player) //reset speed when player exits collider
        {
            _playerMovement.speedMultiplicator = _regularSpeed;
            _playerDash.dashSpeed = _regularDashSpeed;
        }
    }

    //TODO: Bug with dashing -> @Maki? dash timer gets stuck in negative :(
    //reason: u used _regularSpeed to calculate the dash speed, not the _regularDashSpeed

}
