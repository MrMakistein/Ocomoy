using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// For controlling the Appearance of the Health Circle.
/// Should be attached to the responsible projector!
/// </summary>
public class HealthBarControl : MonoBehaviour {
    private GameObject _player;
    public Projector _projector; //the projector projecting the circle
    public Projector _projectorInnerLimit;
    public Projector _projectorOuterLimit;

    [Header("Health Bar Logic")]
    public float _maxHealth;
    public float _currentHealth = 5;

    [Header("Circle Parameters")]
    //absolute orthographic values for projector!
    public float _maxOrthoSize = 2;
    public float _minOrthoSize = 0.75f;

    public float max_diameterScale = 1; // max diameter of the health ring (for scaling sprite)
    public float min_diameterScale = 0.5f; //min diameter of the health ring (for scaling sprite)
    public float _initProjectorSize;
    public Sprite _sprite;
    public Color _fullHealthColor = Color.green;
    public Color _noHealthColor = Color.red;


	// Use this for initialization
	void Start () {
        _player = GameObject.Find("Player");
        _currentHealth = _player.GetComponent<Player>().currentHealth;
        _maxHealth = _player.GetComponent<Player>().maxHealth;
        _projector = GetComponent<Projector>();
        _initProjectorSize = _projector.orthographicSize;

        //set boundaries of shadow health circles accordingly
        //_projectorOuterLimit.orthographicSize = _initProjectorSize;
        //_projectorInnerLimit.orthographicSize = _initProjectorSize * min_diameterScale;
        _projectorOuterLimit.orthographicSize = _maxOrthoSize;
        _projectorInnerLimit.orthographicSize = _minOrthoSize;


    }

    // Update is called once per frame
    void Update () {
        _currentHealth = _player.GetComponent<Player>().currentHealth;
        UpdateHealthCircle(_currentHealth);
    }

    /// <summary>
    /// For updating the health circle (color and diameter!)
    /// </summary>
    /// <param name="currentHealth">    the current Health of the player character</param>
    private void UpdateHealthCircle(float currentHealth)
    {
        if (currentHealth >= 0)
        {
            #region calculate size
            // map health percentage to defined circle radius range
            //new_value = (old_value - old_bottom) / (old_top - old_bottom) * (new_top - new_bottom) + new_bottom;
            float toBeOrthoSize = (currentHealth / _maxHealth) * _maxOrthoSize; //--> verhaeltnis
            if (_minOrthoSize != 0)
            {
                //currentOrthoSize = (currentOrthoSize - 0) / (_maxOrthoSize - 0) * (_maxOrthoSize - _minOrthoSize) + _minOrthoSize;
                toBeOrthoSize = toBeOrthoSize / _maxOrthoSize * (_maxOrthoSize - _minOrthoSize) + _minOrthoSize;
            }
            _projector.orthographicSize = toBeOrthoSize;
            #endregion set size 

            #region set color
            //access Shader Parameter!
            //float H = 0;
            //float S = 0;
            //float V = 0;
            //Color.RGBToHSV(_fullHealthColor, out H, out S, out V);
           // Debug.Log(H + " " + S + " " + V);
           // _projector.material.SetColor("_Color", _fullHealthColor);
            #endregion set color

        }
    }

    private void UpdateHealthCircleOLD(float currentHealth)
    {
        /// TODO:
        /// - calculate percentage of health
        float healthInPercent = _currentHealth / _maxHealth;
        /// - determine ratio 
        // map health percentage to defined circle radius range
        //new_value = (old_value - old_bottom) / (old_top - old_bottom) * (new_top - new_bottom) + new_bottom;
        if (healthInPercent >= 0.0)
        {
            float toBeDiameterScale = max_diameterScale * healthInPercent; //Diameter for a min_diameter scale of 0 
            if (min_diameterScale != 0.0)
            { //if lower border was changed...
                toBeDiameterScale = (toBeDiameterScale - 0) / (max_diameterScale - 0) * (max_diameterScale - min_diameterScale) + min_diameterScale; //remapping!
            }
            _projector.orthographicSize = _initProjectorSize * toBeDiameterScale;
        }

        //TODO: lerp!
        //_projector.orthographicSize;

    }
}
