using System.Globalization;
using System.IO;
using UnityEngine;
using ColorBlockCrush.Tools;

public class SaveLoadUtility
{
	public static string pass = "colorblock@012356789";
	public static LevelConfig GetLevel(string content)
	{
		content = QT.DecryptAndDecompress(content, pass);		
        return CreateLevelConfigFromJson(content);
	}

    public static LevelConfig CreateLevelConfigFromJson(string json)
    {
        LevelConfig levelConfig = new LevelConfig();
        JsonUtility.FromJsonOverwrite(json, levelConfig);
        return levelConfig;
    }

    public static Quaternion GetRotation(string r)
	{
		string[] values = r.Split(',');
		return new Quaternion(
			float.Parse(values[0], CultureInfo.InvariantCulture),
			float.Parse(values[1], CultureInfo.InvariantCulture),
			float.Parse(values[2], CultureInfo.InvariantCulture),
			float.Parse(values[3], CultureInfo.InvariantCulture)
		);
	}
	public static string RotationToString(Quaternion rotation)
	{

		string x = rotation.x.ToString("F2", CultureInfo.InvariantCulture);
		string y = rotation.y.ToString("F2", CultureInfo.InvariantCulture);
		string z = rotation.z.ToString("F2", CultureInfo.InvariantCulture);
		string w = rotation.w.ToString("F2", CultureInfo.InvariantCulture);
		string sum = x + "," + y + "," + z + "," + w;
		return sum;
	}


	public static Vector3 GetPosition(string p)
	{
		string[] values = p.Split(',');
		return new Vector3(
			float.Parse(values[0], CultureInfo.InvariantCulture),
			float.Parse(values[1], CultureInfo.InvariantCulture),
			float.Parse(values[2], CultureInfo.InvariantCulture)
		);
	}

	public static string PositionToString(Vector3 position)
	{

		string x = position.x.ToString("F2", CultureInfo.InvariantCulture);
		string y = position.y.ToString("F2", CultureInfo.InvariantCulture);
		string z = position.z.ToString("F2", CultureInfo.InvariantCulture);
		string sum = x + "," + y + "," + z;
		return sum;
	}

	//public static CarSaveData GetCarSaveData(Car car)
	//{
	//	CarSaveData data = new CarSaveData();
	//	data.t = new int[1] { car.CarTypeIndex };
	//	data.c = new int[1] { car.colorIndex };
	//	data.g = false;
	//	data.p = PositionToString(car.transform.localPosition);
	//	data.r = RotationToString(car.transform.localRotation);
	//	data.isLock = car.KeyType == KeyType.Lock;	
	//	data.isKey = car.KeyType == KeyType.Key;
	//	data.isHide = car.isHide;
	//	data.keyID = (int)car.KeyID;
	//	data.isFrezze = car.IsFrezze;
	//	data.isBomb = car.isBomb;

 //       if (car.isBomb)
 //       {
	//		data.bombCount = car.bombCount;
	//	}
 //       else
 //       {
	//		data.bombCount = 0;
	//	}

	//	return data;
	//}

	//public static CarSaveData GetCarSaveData(Gara gara)
	//{
	//	CarSaveData data = new CarSaveData();
	//	data.t = gara.carIndexList;
	//	data.c = gara.colorList;
	//	data.g = true;
	//	data.p = PositionToString(gara.transform.localPosition);
	//	data.r = RotationToString(gara.transform.localRotation);
	//	return data;
	//}


	public static void SaveLevelToDisk(string content, string folder, int level)
	{
		string levelName = FetchLevelManager.GetPerfectName(level);
		try
		{
			string folderPath = Path.Combine(Application.persistentDataPath, folder);
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string filePath = Path.Combine(folderPath, levelName);
			System.IO.File.WriteAllText(filePath, content);
			Debug.Log($"[FetchLevel] {level} saved disk success at: {filePath}");
			//ToastUtil.ShowToast($"Level {level} {folder} saved success");
		}

		catch (System.Exception e)
		{
			Debug.LogError("Error saving level to disk: " + e.Message);
			//ToastUtil.ShowToast($"Level {level} {folder} fail save to disk");
		}
	}

	public static LevelConfig LoadLevelFormDisk(string levelNameExtension, string folder)
	{
		//ToastUtil.ShowToastDebug(folder + "-" + levelNameExtension);
        string fileName = Path.Combine(Application.persistentDataPath, folder, levelNameExtension);
        try
		{
			if (File.Exists(fileName))
			{
				string text = File.ReadAllText(fileName); Debug.Log("TExt " + text);
                LevelConfig level = SaveLoadUtility.GetLevel(text);
    
                //ToastUtil.ShowToast($"[FetchLevel] load level form disk success {folder} {levelNameExtension}");
				Debug.Log($"[FetchLevel] load level form disk success {folder} {levelNameExtension}");
				return level;
			}
		}
		catch (System.Exception e)
		{
			//ToastUtil.ShowToast("[FetchLevel] load level from disk fail: " + e.Message);
			Debug.LogError($"[FetchLevel] load level from disk fail:  {folder} {levelNameExtension}");
		}

		return null;
	}

	
}

