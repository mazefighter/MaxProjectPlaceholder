using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoScriptTextAdventure : MonoBehaviour
{
   [SerializeField] private TextAsset inkJson;
   private void Start()
   {
      M_DialogueManager.GetInstance().EnterDialogueMode(inkJson);
   }
}
