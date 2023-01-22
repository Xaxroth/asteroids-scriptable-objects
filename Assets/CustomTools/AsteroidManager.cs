using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AsteroidManager : EditorWindow
{
    public BaseAsteroid AsteroidSO;
    private BaseAsteroid currentEditorObject;
    private bool informationGathered = false;

    [Header("Physics")]

    public float _asteroidMinForce = 1;
    public float _asteroidMaxForce = 20;

    public float _asteroidMinSize = 0.1f;
    public float _asteroidMaxSize = 2;

    public float _asteroidMinTorque = 1;
    public float _asteroidMaxTorque = 100;

    [Header("Cosmetics")]
    public Color asteroidColor = new Color(255, 255, 255, 255);
    public Sprite asteroidSprite;

    [MenuItem("Tools/Asteroid Manager")]
    public static void DisplayWindow()
    {
        GetWindow(typeof(AsteroidManager));
    }

    public void OnGUI()
    {
        if (AsteroidSO != null)
        {
            if (!informationGathered)
            {
                GatherObjectInformation();
                informationGathered = true;
            }

            if (currentEditorObject != AsteroidSO)
            {
                informationGathered = false;
            }
        }

        if (AsteroidSO == null)
        {
            ResetAllValues();
            informationGathered = false;
        }

        DisplayVariables();
    }

    // The variables visible in the Custom Inspector.

    public void DisplayVariables()
    {
        EditorGUILayout.LabelField("[Asteroid To Edit]", EditorStyles.boldLabel);

        AsteroidSO = EditorGUILayout.ObjectField("Asteroid", AsteroidSO, typeof(BaseAsteroid), false) as BaseAsteroid;

        EditorGUILayout.LabelField("[Physics Settings]", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Force", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Min Force");
        _asteroidMinForce = EditorGUILayout.Slider(_asteroidMinForce, 1f, 20f);

        EditorGUILayout.LabelField("Max Force");
        _asteroidMaxForce = EditorGUILayout.Slider(_asteroidMaxForce, 1f, 20f);

        if (_asteroidMinForce > _asteroidMaxForce)
        {
            _asteroidMinForce = _asteroidMaxForce;
        }

        EditorGUILayout.LabelField("Torque", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Min Torque");
        _asteroidMinTorque = EditorGUILayout.Slider(_asteroidMinTorque, 1f, 100f);

        EditorGUILayout.LabelField("Max Torque");
        _asteroidMaxTorque = EditorGUILayout.Slider(_asteroidMaxTorque, 1f, 100f);

        if (_asteroidMinTorque > _asteroidMaxTorque)
        {
            _asteroidMinTorque = _asteroidMaxTorque;
        }

        EditorGUILayout.LabelField("Size", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Min Size");
        _asteroidMinSize = EditorGUILayout.Slider(_asteroidMinSize, 0.1f, 2f);

        EditorGUILayout.LabelField("Max Size");
        _asteroidMaxSize = EditorGUILayout.Slider(_asteroidMaxSize, 0.1f, 2f);

        if (_asteroidMinSize > _asteroidMaxSize)
        {
            _asteroidMinSize = _asteroidMaxSize;
        }

        EditorGUILayout.LabelField("[Cosmetics]", EditorStyles.boldLabel);

        asteroidColor = EditorGUILayout.ColorField("Asteroid Color", asteroidColor);
        asteroidSprite  = EditorGUILayout.ObjectField("Asteroid Sprite", asteroidSprite, typeof(Sprite), false) as Sprite;

        // Either updates the current ScriptableObject selected, or creates a new asset in the folder of the user's choosing.

        if (AsteroidSO != null)
        {
            if (GUILayout.Button("Save and set as Spawn"))
            {
                UpdateAsteroid();
            }
        }
        else
        {
            if (GUILayout.Button("Create New Asteroid Asset"))
            {
                CreateNewAsteroid();
            }
        }
    }

    // Sets the variables in the scriptableobject to be equal to that of the current values in the inspector.

    public void UpdateAsteroid()
    {
        AsteroidSO._asteroidMinForce = _asteroidMinForce;
        AsteroidSO._asteroidMaxForce = _asteroidMaxForce;

        AsteroidSO._asteroidMinTorque = _asteroidMinTorque;
        AsteroidSO._asteroidMaxTorque = _asteroidMaxTorque;

        AsteroidSO._asteroidMinSize = _asteroidMinSize;
        AsteroidSO._asteroidMaxSize = _asteroidMaxSize;

        AsteroidSO.asteroidColor = asteroidColor;
        AsteroidSO.asteroidSprite = asteroidSprite;

        SaveAsteroid();
    }

    // Gathers the object's data values and aligns the editors variables to match.

    public void GatherObjectInformation()
    {
        currentEditorObject = AsteroidSO;

        _asteroidMinForce = AsteroidSO._asteroidMinForce;
        _asteroidMaxForce = AsteroidSO._asteroidMaxForce;

        _asteroidMinTorque = AsteroidSO._asteroidMinTorque;
        _asteroidMaxTorque = AsteroidSO._asteroidMaxTorque;

        _asteroidMinSize = AsteroidSO._asteroidMinSize;
        _asteroidMaxSize = AsteroidSO._asteroidMaxSize;

        asteroidColor = AsteroidSO.asteroidColor;
        asteroidSprite = AsteroidSO.asteroidSprite;
    }

    // Resets the custom editors sliders and values to their default values.

    public void ResetAllValues()
    {
        _asteroidMinForce = 1;
        _asteroidMaxForce = 100;

        _asteroidMinTorque = 1;
        _asteroidMaxTorque = 100;

        _asteroidMinSize = 0.1f;
        _asteroidMaxSize = 2f;

        asteroidColor = new Color(255, 255, 255, 255);
        asteroidSprite = null;
    }

    // Creates a new ScriptableObject asset that can be used by the asteroidspawner.

    public void CreateNewAsteroid()
    {
        string path = EditorUtility.SaveFilePanel("Save Asteroid SO", "Assets/", "Asteroids", "asset");
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("PATH INVALID!");
            return;
        }

        path = FileUtil.GetProjectRelativePath(path);

        BaseAsteroid newAsteroid = ScriptableObject.CreateInstance<BaseAsteroid>();

        newAsteroid._asteroidMinForce = _asteroidMinForce;
        newAsteroid._asteroidMaxForce = _asteroidMaxForce;

        newAsteroid._asteroidMinTorque = _asteroidMinTorque;
        newAsteroid._asteroidMaxTorque = _asteroidMaxTorque;

        newAsteroid._asteroidMinSize = _asteroidMinSize;
        newAsteroid._asteroidMaxSize = _asteroidMaxSize;

        newAsteroid.asteroidColor = asteroidColor;
        newAsteroid.asteroidSprite = asteroidSprite;

        AssetDatabase.CreateAsset(newAsteroid, path);
        AssetDatabase.SaveAssets();
    }

    // Checks if the default data path for saving exists.

    public bool Saving()
    {
        return Directory.Exists(Application.persistentDataPath + "/saved_asteroids");
    }

    // Creates a JSON file in the default directory.

    public void SaveAsteroid()
    {
        if (!Saving())
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saved_asteroids");
        }
        if (!Directory.Exists(Application.persistentDataPath + "/saved_asteroids/asteroid_data"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saved_asteroids/asteroid_data");
        }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/saved_asteroids/asteroid_data/asteroid.txt");
            var json = JsonUtility.ToJson(AsteroidSO);
            bf.Serialize(file, json);
            file.Close();
    }

    public void LoadAsteroid()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saved_asteroids"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saved_asteroids");
        }
        BinaryFormatter bf = new BinaryFormatter();
        if(File.Exists(Application.persistentDataPath + "/saved_asteroids/asteroid_data/asteroid.txt"))
        {
            FileStream file = File.Open(Application.persistentDataPath + "/saved_asteroids/asteroid_data/asteroid.txt", FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), AsteroidSO);
            file.Close();
        }
    }
}
