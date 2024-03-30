using MelonLoader;
using System.IO;
using UnityEngine;

namespace Nova_Max.EasterEggMod
{
    public class EasterEggMod : MelonMod
    {
        private bool playing = false;
        private bool notesReplaced = false;
        private bool railsReplaced = false;
        public GameObject eggObject;
        public Texture2D tex;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Easter Egg Mod loaded!");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (eggObject == null)
            {
                eggObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Object.DontDestroyOnLoad(eggObject);

                eggObject.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
                eggObject.transform.position = new Vector3(0f, 0.5f, 1f);

                eggObject.GetComponent<MeshFilter>().mesh = ObjLoader.LoadMeshFromObj("Mods/EasterEggMod/model.obj");

                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, File.ReadAllBytes("Mods/EasterEggMod/texture.png"));
                Material debugMaterial = new Material(Shader.Find("Unlit/Texture"));
                debugMaterial.SetTexture("_MainTex", tex);

                eggObject.GetComponent<MeshRenderer>().SetMaterial(debugMaterial);
                eggObject.SetActive(false);
                LoggerInstance.Msg("Egg created");
            }


            playing = sceneName.Contains("Stage");
            if (!playing)
            {
                notesReplaced = false;
                railsReplaced = false;
            }
        }

        public override void OnUpdate()
        {
            if (playing)
            {
                if (!notesReplaced)
                {
                    GameObject noteManager = GameObject.Find("Note Manager(Clone)");
                    if (noteManager != null)
                    {
                        ReplaceAssets(noteManager);
                        notesReplaced = true;
                    }
                }

                if (!railsReplaced)
                {
                    GameObject railManager = GameObject.Find("Rail Manager(Clone)");
                    if (railManager != null)
                    {
                        ReplaceAssets(railManager);
                        railsReplaced = true;
                    }
                }
            }
        }

        private void ReplaceAssets(GameObject gameObject)
        {
            int count = gameObject.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                Transform note = gameObject.transform.GetChild(i);
                if (note != null)
                {
                    if (note.name.Contains("Note Variant") || note.name.Contains("Rail Variant"))
                    {
                        Transform model = note.Find("Note Model/Display Size Scaler/Standard");
                        model.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);

                        model.GetComponent<MeshFilter>().mesh = eggObject.GetComponent<MeshFilter>().mesh;

                        model.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", eggObject.GetComponent<MeshRenderer>().material.mainTexture);
                    }
                }
            }
        }
    }
}
