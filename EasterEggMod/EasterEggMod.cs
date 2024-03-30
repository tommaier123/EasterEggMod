using MelonLoader;
using System.IO;
using UnityEngine;

namespace Nova_Max.EasterEggMod
{
    public class EasterEggMod : MelonMod
    {
        private bool playing = false;
        private bool replacedA = false;
        private bool replacedB = false;
        public GameObject eggObject;
        public Mesh mesh;
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
                eggObject.transform.rotation = Quaternion.Euler(0, 90, 0);

                eggObject.GetComponent<MeshFilter>().mesh = ObjLoader.LoadMeshFromObj("Mods/EasterEggMod/model.obj");
                mesh = eggObject.GetComponent<MeshFilter>().mesh;

                tex = new Texture2D(2, 2);
                ImageConversion.LoadImage(tex, File.ReadAllBytes("Mods/EasterEggMod/texture.png"));
                Material eggMaterial = new Material(Shader.Find("Unlit/Texture"));
                eggMaterial.SetTexture("_MainTex", tex);

                eggObject.GetComponent<MeshRenderer>().material = eggMaterial;
                eggObject.SetActive(false);
                LoggerInstance.Msg("Egg created");
            }


            playing = sceneName.Contains("Stage");
            if (!playing)
            {
                replacedA = false;
                replacedB = false;
            }
        }

        public override void OnUpdate()
        {
            if (playing)
            {
                if (!replacedA)
                {
#if IL2CPP
                    GameObject collectionA = GameObject.Find("Note Manager(Clone)");
#else
                    GameObject collectionA = GameObject.Find("NotesWrap");
#endif
                    if (collectionA != null)
                    {
                        ReplaceAssets(collectionA);
                        replacedA = true;
                    }
                }

                if (!replacedB)
                {
#if IL2CPP
                    GameObject collectionB = GameObject.Find("Rail Manager(Clone)");
#else
                    GameObject collectionB = GameObject.Find("Disabled Track Notes Holder");
#endif
                    if (collectionB != null)
                    {
                        ReplaceAssets(collectionB);
                        replacedB = true;
                    }
                }
            }
        }


#if IL2CPP
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

                        model.GetComponent<MeshFilter>().mesh = mesh;

                        model.GetComponent<MeshRenderer>().material.SetTexture("_BaseMap", tex);
                    }
                }
            }
        }
#else
        private void ReplaceAssets(GameObject gameObject)
        {
            int count = gameObject.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                //normal
                Transform note = gameObject.transform.GetChild(i);
                Transform model = note.Find("Wrap/Sphere/NotesSphere");
                model.transform.localScale = new Vector3(0.008f, 0.008f, 0.008f);
                model.transform.rotation = Quaternion.Euler(0, 90, 0);

                MeshFilter[] mfs = model.GetComponentsInChildren<MeshFilter>(true);
                MeshRenderer[] mrs = model.GetComponentsInChildren<MeshRenderer>(true);

                foreach (var mf in mfs)
                {
                    mf.mesh = mesh;
                }
                foreach (var mr in mrs)
                {
                    mr.material.SetTexture("_MainTex", tex);
                    mr.material.SetTexture("_EmissionTex", tex);
                }

                ////vanish
                //model = note.Find("Wrap/Sphere/VanishRenderers/Vanish Sphere");
                //model.transform.localScale = new Vector3(1f, 1f, 1f);
                //model.transform.rotation = Quaternion.Euler(0, 90, 0);

                //model.GetComponent<MeshFilter>().mesh = mesh;

                //MeshRenderer mmr = model.GetComponent<MeshRenderer>();
                //mmr.material.SetTexture("_MainTex", tex);
                //mmr.material.SetTexture("_EmissionTex", tex);


                //rainbow
                model = note.Find("Wrap/Sphere/RaimbowRenders/Raimbow Sphere");
                model.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                model.GetComponent<MeshFilter>().mesh = mesh;
            }
        }
#endif
    }
}
