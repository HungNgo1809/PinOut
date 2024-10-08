using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Funzilla
{
	internal class LevelCreator : Scene
	{
        public PinType curPinType;
        public Level level;
        public int pinSize = 1;

        Pin curPin;
        List<Pin> createdPins = new List<Pin>();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
                RotateY(-90.0f);
            if (Input.GetKeyDown(KeyCode.D))
                RotateY(90.0f);
            if (Input.GetKeyDown(KeyCode.W))
                RotateX(90.0f);
            if (Input.GetKeyDown(KeyCode.S))
                RotateX(-90.0f);
            if (Input.GetKeyDown(KeyCode.Q))
                RotateZ(90.0f);
            if (Input.GetKeyDown(KeyCode.Delete))
                DeletePin();
            if (Input.GetMouseButtonDown(0))
                SelectPin();
            if (Input.GetKeyDown(KeyCode.UpArrow))
                curPin.transform.position = new Vector3(curPin.transform.position.x, curPin.transform.position.y, curPin.transform.position.z + 0.5f);
            if (Input.GetKeyDown(KeyCode.DownArrow))
                curPin.transform.position = new Vector3(curPin.transform.position.x, curPin.transform.position.y, curPin.transform.position.z - 0.5f);
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                curPin.transform.position = new Vector3(curPin.transform.position.x - 0.5f, curPin.transform.position.y, curPin.transform.position.z);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                curPin.transform.position = new Vector3(curPin.transform.position.x + 0.5f, curPin.transform.position.y, curPin.transform.position.z);
            if (Input.GetKeyDown(KeyCode.K))
                curPin.transform.position = new Vector3(curPin.transform.position.x, curPin.transform.position.y + 0.5f, curPin.transform.position.z);
            if (Input.GetKeyDown(KeyCode.M))
                curPin.transform.position = new Vector3(curPin.transform.position.x, curPin.transform.position.y - 0.5f, curPin.transform.position.z);
        }
        public void SetPinType(int type)
        {
            curPinType = (PinType)type;
        }
        public void CreateNewPins()
        {
            Pin newPin = Instantiate(level.pinsPrefab[(int)curPinType], level.transform);
            newPin.pinId = createdPins.Count;
            newPin.size = pinSize;
            newPin.HandlePinSize();

            curPin = newPin;
            createdPins.Add(newPin);
        }
        void RotateX(float radius)
        {
            Quaternion rotation = Quaternion.Euler(radius, 0, 0); // Tạo rotation cho trục X
            curPin.transform.rotation = curPin.transform.rotation * rotation; // Quay theo X
        }

        void RotateY(float radius)
        {
            Quaternion rotation = Quaternion.Euler(0, radius, 0); // Tạo rotation cho trục Y
            curPin.transform.rotation = curPin.transform.rotation * rotation; // Quay theo Y
        }

        void RotateZ(float radius)
        {
            Quaternion rotation = Quaternion.Euler(0, 0, radius); // Tạo rotation cho trục Z
            curPin.transform.rotation = curPin.transform.rotation * rotation; // Quay theo Z
        }
        void DeletePin()
        {
            Destroy(curPin.gameObject);
        }    
        public void SaveLevel()
        {
            LevelData levelData = new LevelData();
            levelData.levelId = level.levelId;

            foreach (Pin pin in createdPins)
            {
                pin.ClearAllLinkedPins();
            }
            foreach (Pin pin in createdPins)
            {
                pin.Init(level);

                PinData pinData = new PinData();
                pinData.pinId = pin.pinId;
                pinData.pinType = (int)pin.type;

                pinData.posX = pin.transform.position.x;
                pinData.posY = pin.transform.position.y;
                pinData.posZ = pin.transform.position.z;

                pinData.rotX = pin.transform.eulerAngles.x;
                pinData.rotY = pin.transform.eulerAngles.y;
                pinData.rotZ = pin.transform.eulerAngles.z;

                pinData.innerPins = new List<int>(pin.innerPins);
                pinData.frontPins = new List<int>(pin.frontPins);
                pinData.dependencePins = new List<int>(pin.dependencePins);

                levelData.pins.Add(pinData);
            }

            ExportLevelData(levelData);
        }
        void SelectPin()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Pin selectedPin = hit.collider.gameObject.GetComponent<Pin>();
                if (selectedPin != null)
                    curPin = selectedPin;
            }
        }
        internal void ExportLevelData(LevelData levelData)
        {
            string data = JsonUtility.ToJson(levelData, true);
            string filePath = Path.Combine(Application.dataPath, "Resources/") + GlobalDefine.LevelDataPath + "levelData_" + level.levelId + ".json";
            File.WriteAllText(filePath, data);
            Debug.Log("write data to: " + filePath);
        }
        internal static LevelData GetLevelDataFromJson(string id)
        {
            string filePath = GlobalDefine.LevelDataPath + "levelData_" + id;
            TextAsset jsonData = Resources.Load<TextAsset>(filePath);
            if (jsonData != null)
            {
                LevelData levelData = JsonUtility.FromJson<LevelData>(jsonData.text);
                Debug.Log("Level data loaded from Resources: " + filePath);
                return levelData;
            }
            else
            {
                Debug.LogError("File not found in Resources: " + filePath);
                return null;
            }
        }
    }
}

[System.Serializable]
public class PinData
{
    public int pinId;
    public int pinType;

    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;

    public List<int> innerPins = new List<int>();
    public List<int> frontPins = new List<int>();
    public List<int> dependencePins = new List<int>();
}
[System.Serializable]
public class LevelData
{
    public string levelId;
    public List<PinData> pins = new List<PinData>();
}