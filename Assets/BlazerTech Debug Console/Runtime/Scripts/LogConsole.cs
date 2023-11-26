using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlazerTech
{
    public class LogConsole : MonoBehaviour
    {
        public event EventHandler OnEnabled;

        private void OnEnable()
        {
            OnEnabled?.Invoke(this, EventArgs.Empty);
        }
    }
}
