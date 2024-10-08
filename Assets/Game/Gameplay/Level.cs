using System.Collections.Generic;
using UnityEngine;

namespace Funzilla
{
	public class Level : MonoBehaviour
	{
		public List<Pin> pinsPrefab = new List<Pin>();
		internal List<Pin> pins = new List<Pin>(); //TODO: Add cac pin khoi tao sau khi load lai du lieu
		public string levelId;
		internal void LoadLevel()
		{
			LevelData levelData = LevelCreator.GetLevelDataFromJson(levelId);

			foreach(PinData pinData in levelData.pins)
            {
				//Tái tạo pin
				Pin pin = Instantiate(pinsPrefab[pinData.pinType]);
				pin.pinId = pinData.pinId;
				pin.transform.position = new Vector3(pinData.posX, pinData.posY, pinData.posZ);
				pin.transform.rotation = Quaternion.Euler(new Vector3(pinData.rotX, pinData.rotY, pinData.rotZ));

				pin.innerPins = new List<int>(pinData.innerPins);
				pin.frontPins = new List<int>(pinData.frontPins);
				pin.dependencePins = new List<int>(pinData.dependencePins);
            }				
		}
	}
}

