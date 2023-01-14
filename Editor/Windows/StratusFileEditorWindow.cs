using Stratus.Editor;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Stratus.Editor
{
    public class StratusFileEditorWindow : StratusEditorWindow<StratusFileEditorWindow>
    {
        public static void Open()
        {
            OpenWindow("Stratus Assets", true);
        }


        protected override void OnWindowEnable()
        {

        }

        protected override void OnWindowGUI()
        {

        }
    }
}