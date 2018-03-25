using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace GrabData
{
  public class DataNode
  {
    public int time;
    public Vector3Int accelC;
    public Vector3Int gyroC;
    public Vector3Int accelO;
    public Vector3Int gyroO;

    protected void ShowData()
    {
      Debug.Log("Time: " + time + " (ms)");
      Debug.Log("Closed accel: " + accelC);
      Debug.Log("Closed gyro: " + gyroC);
      Debug.Log("Open accel: " + accelO);
      Debug.Log("Open gyro: " + gyroO);
    }

    public void Summary()
    {
      ShowData();
    }
  }

  public class GrabData : MonoBehaviour
  {
    public TextAsset deviceLog;

    public List<string> deviceData;

    public int sensorNumber;

    /*
      unit--timestamp[ms];
      
      bma280_x[mg];bma280_y[mg];bma280_z[mg];
      bmg160_x[mDeg];bmg160_y[mDeg];bmg160_z[mDeg];
      bmi160_a_x[mg];bmi160_a_y[mg];bmi160_a_z[mg];
      bmi160_g_x[mDeg];bmi160_g_y[mDeg];bmi160_g_z[mDeg];

      0;-25;294;1062;19409;148620;-106994;-11;535;1127;15747;147155;-107788;

    public int time;
    public Vector3Int accelC, gyroC, accelO, gyroO;
    */

    // List of MyObject data values for every element generated from our log file
    public List<DataNode> myData;

    private void Start()
    {
      myData = new List<DataNode>();
    }

    /// <summary>
    /// Loads data into a List from the raw log file, separated by newline
    /// </summary>
    public void LoadData()
    {
      deviceData = deviceLog.text.Split("\r\n".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries).ToList();

      Debug.Log("There are " + deviceData.Count + " entries.");

      for (int i = 1; i < deviceData.Count; i++)
      {
        Debug.Log("Entry " + i + ": " + deviceData[i]);
      }
    }

    /// <summary>
    /// Creates Data Set Object from FULL log (1 to total line count)
    /// </summary>
    public void CreateDataSet()
    {
      if (deviceData != null && deviceData.Count >= 1)
      {
        CreateDataSet(new Vector2Int(1, deviceData.Count));
      }
      else
      {
        Debug.LogWarning("Data not loaded");
      }
    }

    /// <summary>
    /// Creates Data Set in specific range
    /// </summary>
    /// <param name="range">Min/Max Range.</param>
    public void CreateDataSet(Vector2Int range)
    {
      // Range should never start below 1 (element 0 has data point names)
      if (range.x < 1) { range.x = 1; }

      for (int i = range.x; i < range.y; i++)
      {
        // Add Data into our public List
        DataNode n = PopulateObject(i);
        myData.Add(n);

        Debug.Log("Entry " + i + ": " + deviceData[i]);
      }
    }

    /// <summary>
    /// Outputs entire data set in range to console
    /// </summary>
    public void ShowDataSet()
    {
      // Range should never start below 1 (element 0 has data point names)
      if (myData != null && myData.Count >= 1)
      {
        ShowDataSet(new Vector2Int(0, myData.Count));
      }
    }

    /// <summary>
    /// Outputs data set in range to console
    /// </summary>
    /// <param name="range">Range.</param>
    public void ShowDataSet(Vector2Int range)
    {
      // Range should never start below 1 (element 0 has data point names)
      if (range.x < 0) { range.x = 0; }

      for (int i = range.x; i < range.y; i++)
      {
        myData[i].Summary();
      }
    }

    /// <summary>
    /// Populates object with one index's values
    /// </summary>
    /// <param name="index">Index.</param>
    public DataNode PopulateObject(int index)
    {
      DataNode node = new DataNode();

      // Split line into array values
      var array = deviceData[index].Split(';');

      Debug.Log("Array length is: " + array.Length);

      // Time in ms
      node.time = System.Convert.ToInt32(array[0]);

      // Acceleration Closed
      node.accelC = new Vector3Int(System.Convert.ToInt32(array[1]),
                                       System.Convert.ToInt32(array[2]),
                                       System.Convert.ToInt32(array[3]));

      // Gyro Closed
      node.gyroC = new Vector3Int(System.Convert.ToInt32(array[4]),
                                       System.Convert.ToInt32(array[5]),
                                       System.Convert.ToInt32(array[6]));

      // Accel Open
      node.accelO = new Vector3Int(System.Convert.ToInt32(array[7]),
                                       System.Convert.ToInt32(array[8]),
                                       System.Convert.ToInt32(array[9]));

      // Gyro Open
      node.gyroO = new Vector3Int(System.Convert.ToInt32(array[10]),
                                       System.Convert.ToInt32(array[11]),
                                       System.Convert.ToInt32(array[12]));

      return node;
    }

  }
}