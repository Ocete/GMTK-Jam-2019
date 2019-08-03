using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public sealed class Singleton
    {
        private readonly static Singleton _instance = new Singleton();

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
