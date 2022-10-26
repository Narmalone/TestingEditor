using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Processors;

public class Laser : MonoBehaviour
{
    [SerializeField] float perfRange = 20f;

    LineRenderer lineRenderer;
    private Vector3 vecReflected;
    private Vector3 vecNewPosition;

    private Ray nextRay;

    //List collider
    //si dans le mask ou bien trouve le collider à un script on l'ajoute
    public List<Collider> colliders;

    private bool isRaycasting = false;

    private int indexCollider = 0;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
    }

    private void Start()
    {
    }

    //Si le hit collider existe dans la liste de col on dessine le point
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        lineRenderer.SetPosition(1, new Vector3(transform.forward.x * perfRange, transform.position.y, transform.forward.z * perfRange));
        if (Physics.Raycast(ray, out hit, perfRange))
        {
            lineRenderer.SetPosition(1, new Vector3(hit.point.x, transform.position.y, hit.point.z));
            if (colliders.Contains(hit.collider))
            {
                //Reflect
                //trouver un moyen d'ajouter des points sans qu'ils s'ajoutent h24 vu que c'est un update
                Vector3 reflectedVect = ReflectedVector(hit.point, hit.normal);

                if (hit.point != reflectedVect)
                {
                    Rebond(hit.point, new Vector3(reflectedVect.x, 0, reflectedVect.z));
                }
            }
        }
    }

    public void AddPoint()
    {
    }

    private Vector3 ReflectedVector(Vector3 fromDirection, Vector3 Normal)
    {
        vecReflected = Vector3.Reflect(fromDirection, Normal);
        return vecReflected;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Origin"></param>
    /// <param name="Dir"></param>
    private void Rebond(Vector3 Origin, Vector3 Dir)
    {
        Ray ray = new Ray(Origin, Dir);

        Debug.DrawRay(Origin, new Vector3(Dir.x * perfRange, 0f, Dir.z * perfRange));
        RaycastHit hit;

        int touched = 0;

        if (Physics.Raycast(ray, out hit, perfRange))
        {


            //vérifier si on retouche le même point
            if (hit.point != Origin)
            {
                
                //à chaque fois qu'un truc touche on aligne soit sur le gameobject soit un truc assez précis nous même 
                if(hit.collider == colliders[1])
                {
                    lineRenderer.SetPosition(2, hit.point);
                    Debug.Log("ça collide sa daronne");
                }
                if(hit.collider == colliders[2])
                {
                    lineRenderer.SetPosition(3, hit.point);
                }
                Vector3 temp = ReflectedVector(hit.point, hit.normal);
                touched = 1;
                //Lancer fonction
                Rebond(hit.point, new Vector3(temp.x, 0, temp.z));

            }
          
        }
        else
        {
            if (touched != 0) return;
            Debug.Log("gros pdd");
        }
    }

 
}
