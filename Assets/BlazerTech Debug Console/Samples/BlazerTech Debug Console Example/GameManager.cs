using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlazerTech
{
    public class GameManager : MonoBehaviour
    {
        int totalLogsCreated;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) CreateLog();

            if (Input.GetKeyDown(KeyCode.Alpha2)) CreateWarning();

            if (Input.GetKeyDown(KeyCode.Alpha3)) CreateError();
        }
        public void CreateLog()
        {
            totalLogsCreated++;
            Debug.Log("Log: " + totalLogsCreated);
        }

        public void CreateWarning()
        {
            totalLogsCreated++;
            Debug.LogWarning("Warning: " + totalLogsCreated);
        }

        public void CreateError()
        {
            totalLogsCreated++;
            Debug.LogError("Error: " + totalLogsCreated);
        }
    }
}
