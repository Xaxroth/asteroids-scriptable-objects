using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Asteroid Base", menuName = "Asteroid")]
public class BaseAsteroid : ScriptableObject
{
    [SerializeField] public AsteroidType condition { get; set; } = AsteroidType.Normal;

    public float _asteroidForce;
    public float _asteroidMinForce = 1;
    public float _asteroidMaxForce = 100;

    public Vector2 _asteroidSize;
    public float _asteroidMinSize = 1;
    public float _asteroidMaxSize = 10;

    public float _asteroidTorque;
    public float _asteroidMinTorque = 1;
    public float _asteroidMaxTorque = 100;

    public Color asteroidColor;
    public Sprite asteroidSprite;


    public enum AsteroidType
    {
        Normal,
        Resistant,
        Beneficial
    }
}
