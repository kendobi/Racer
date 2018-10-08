using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGenerator : MonoBehaviour
{
    bool addHats;
    bool specialMat;
    public GameObject Result;
    public Material[] mats;
    public GameObject[] bodies;
    public GameObject[] Reyes;
    public GameObject[] Leyes;
    public GameObject[] Ftopfins;
    public GameObject[] Btopfins;
    public GameObject[] tailfins;
    public GameObject[] Rsidefins;
    public GameObject[] Lsidefins;
    public GameObject[] Rbottomfins;
    public GameObject[] Lbottomfins;
    public GameObject[] bottomfins2;
  
    // Use this for initialization
    void Start()
    {
        GenerateFish();
    }


    // Update is called once per frame

    public void GenerateFish()
    {
      
        if (Random.Range(0, 100) >= 97f)
        {
           specialMat = true;
        }
        else
        {
            specialMat = false;
        }
        transform.localScale = new Vector3(1, 1, 1);
        // body
        foreach (GameObject body in bodies)
        {
            body.SetActive(false);
        }
        int RandomBody = Random.Range(0, bodies.Length);
        
        bodies[RandomBody].SetActive(true);
        // position
        Vector3 Reyeposition = bodies[RandomBody].transform.Find("REyePosition").position;
        Vector3 Leyeposition = bodies[RandomBody].transform.Find("LEyePosition").position;
        Vector3 TailPosition = bodies[RandomBody].transform.Find("TailPosition").position;
        Vector3 BackBottomFinPosition = bodies[RandomBody].transform.Find("BackBottomFinPosition").position;
        Vector3 FrontTopFinPosition = bodies[RandomBody].transform.Find("FrontTopFinPosition").position;
        Vector3 BottomTopFinPosition = bodies[RandomBody].transform.Find("BottomTopFinPosition").position;
        Vector3 LSideFinPosition = bodies[RandomBody].transform.Find("LSideFinPosition").position;
        Vector3 RSideFinPosition = bodies[RandomBody].transform.Find("RSideFinPosition").position;
        Vector3 RBottomFrontFinPosition = bodies[RandomBody].transform.Find("RBottomFrontFinPosition").position;
        Vector3 LBottomFrontFinPosition = bodies[RandomBody].transform.Find("LBottomFrontFinPosition").position;
        // silly stuff
      
        // eye
        int RandomEyes = Random.Range(0, Reyes.Length);
        foreach (GameObject Reye in Reyes)
        {
            Reye.SetActive(false);
        }
        Reyes[RandomEyes].SetActive(true);
        Reyes[RandomEyes].transform.position = Reyeposition;
        foreach (GameObject Leye in Leyes)
        {
            Leye.SetActive(false);
        }
        Leyes[RandomEyes].SetActive(true);
        Leyes[RandomEyes].transform.position = Leyeposition;
        // sidefin
        int RandomSideFin = Random.Range(0, Rsidefins.Length);
        foreach (GameObject Rside in Rsidefins)
        {
            Rside.SetActive(false);
        }
        Rsidefins[RandomSideFin].SetActive(true);
        Rsidefins[RandomSideFin].transform.position = RSideFinPosition;
        foreach (GameObject Lfin in Lsidefins)
        {
            Lfin.SetActive(false);
        }
        Lsidefins[RandomSideFin].SetActive(true);
        Lsidefins[RandomSideFin].transform.position = LSideFinPosition;
        // Bottomfin
        int RandomBottomSideFin = Random.Range(0, Rbottomfins.Length);
        foreach (GameObject Rside in Rbottomfins)
        {
            Rside.SetActive(false);
        }
        Rbottomfins[RandomBottomSideFin].SetActive(true);
        Rbottomfins[RandomBottomSideFin].transform.position = RBottomFrontFinPosition;
        foreach (GameObject Lfin in Lbottomfins)
        {
            Lfin.SetActive(false);
        }
        Lbottomfins[RandomBottomSideFin].SetActive(true);
        Lbottomfins[RandomBottomSideFin].transform.position = LBottomFrontFinPosition;
        // bottomback
        int RandomBottomBackFin = Random.Range(0, bottomfins2.Length);
        foreach (GameObject Lfin in bottomfins2)
        {
            Lfin.SetActive(false);
        }
        bottomfins2[RandomBottomSideFin].SetActive(true);
        bottomfins2[RandomBottomSideFin].transform.position = BackBottomFinPosition;
        // tailfin
        int RandomTailFin = Random.Range(0, tailfins.Length);
        foreach (GameObject Lfin in tailfins)
        {
            Lfin.SetActive(false);
        }
        tailfins[RandomTailFin].SetActive(true);
        tailfins[RandomTailFin].transform.position = TailPosition;
        // topfront
        int RandomTopfront = Random.Range(0, Ftopfins.Length);
        foreach (GameObject Lfin in Ftopfins)
        {
            Lfin.SetActive(false);
        }
        Ftopfins[RandomTopfront].SetActive(true);
        Ftopfins[RandomTopfront].transform.position = FrontTopFinPosition;

        // topBack
        int RandomTopback = Random.Range(0, Btopfins.Length);
        foreach (GameObject Lfin in Btopfins)
        {
            Lfin.SetActive(false);
        }
        Btopfins[RandomTopback].SetActive(true);
        Btopfins[RandomTopback].transform.position = BottomTopFinPosition;

           int RandomMat = Random.Range(0, mats.Length);
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in rends)
        {
           
                rend.material = mats[RandomMat];
            if (specialMat)
            {
                rend.material = mats[4];
            }

        }
       
       
        transform.localScale = new Vector3(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
        //    blades[RandomBlade].GetComponent<Renderer>().material = mats[RandomMat];
        //    guards[RandomGuard].GetComponent<Renderer>().material = mats[RandomMat];
        //    grips[RandomGrip].GetComponent<Renderer>().material = mats[RandomMat];
        //    pommels[RandomPommel].GetComponent<Renderer>().material = mats[RandomMat];
        //    pommels[RandomPommel].GetComponent<Renderer>().material = mats[RandomMat];
        //    extraGem[RandomGem].GetComponent<Renderer>().material = mats[RandomMat];
       
        Merge(bodies[RandomBody].GetComponent<Renderer>().material);
       
    }

    void Merge(Material mat)
    {
      
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
       
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        Result.GetComponent<MeshFilter>().mesh = new Mesh();
        Result.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        Result.GetComponent<Renderer>().material = mat;
       
        transform.position = position;
        transform.rotation = rotation;
        //transform.gameObject.SetActive(true);
       
    }

  
}
