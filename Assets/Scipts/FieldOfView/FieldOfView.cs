using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Référence Sébastien Lague
#endregion
public class FieldOfView : MonoBehaviour
{
    #region Variables
    public float viewRadius = 5f;
    [Range(0f, 360f)]
    public float viewAngle;

    //Masques contenant ce qui est ciblé ou non
    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    //Liste qui va contenir tous les transforms des cibles dans le radius
    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

    //Optimisation
    public int edgeResolutionIteration = 10;
    //threshold = limite
    //va servir pour différencier plusieurs obstacles en même temps
    public float edgeDistanceThreshold = 10f;

    //MESH
    public float MeshResolution;
    public MeshFilter meshFilter;
    private Mesh mesh;

    #endregion

    #region Unity's Functions

    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "View Mesh";
        meshFilter.mesh = mesh;
    }

    private void Start()
    {
        StartCoroutine(FindTargetsCoroutineWithDelay(.2f));
    }
    private void LateUpdate()
    {
        //Dans le late update car sinon ça attend que notre personnage ait finis de bouger
        DrawFieldOfView();
    }
    #endregion

    #region ViewCast
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        //Si on touche un obstacle
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, ObstacleMask))
        {
            //pas draw
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            //Draw
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }
    /// <summary>
    /// Structure qui va tenir différentes informations pour nous que l'on ait pas besoin de l'écrire à chaque fois
    /// </summary>
    public struct ViewCastInfo
    {
        //Si le raycast a touché qqch
        public bool hit;
        //Le point que le ray à touché
        public Vector3 point;
        //Lenght du ray
        public float distance;
        //angle auquel le ray a été lancé
        public float angle;

        public ViewCastInfo(bool p_hit, Vector3 p_point, float p_distance, float p_angle)
        {
            hit = p_hit;
            point = p_point;
            distance = p_distance;
            angle = p_angle;
        }

    }
    #endregion

    #region FieldOfView
    /// <summary>
    /// Afin de draw ce qu'il y'a entre le rayon A et B
    /// </summary>
    private void DrawFieldOfView()
    {
        //mesh resolution est basiquement le nb de ray qui va être cast par dégré
        int stepCount = Mathf.RoundToInt(viewAngle * MeshResolution);

        float stepAngleSize = viewAngle / stepCount;

        //Construire mesh
        List<Vector3> viewPoints = new List<Vector3>();

        //loop pour trouver tous les viewpoints, on va vouloir savoir si le viewcast précédant hit un obstacle ou pas
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for(int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            //Si i > 0, chech si l'ancien viewcast hit un obstacle et le nouveau non ou l'ancien non et le nouveau oui
            //car on veut trouver l'edge
            if(i > 0)
            {
                //si c'est supérieur c'est à true
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if(oldViewCast.hit != newViewCast.hit || oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    //Ajouter les points de ces 2 edges à la list de viewpoint fournie à laquelle ils ont été set
                    //En gros ils n'ont toujours pas leurs valeurs de Vector3 par défauts
                    if(edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    } 
                    if(edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count +1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        //le mesh sera un child du chara object donc les positions en localSpace

        //Le point zéro est l'origine(depuis l'objet puis les triangles vont faire)
        //0,1,2 -> 0,2,3 -> 0,3,4 du moins c'est l'idée

        vertices[0] = Vector3.zero;

        for(int i = 0; i < vertexCount -1; i++)
        {
            //ne pas overwrite le premier vertex
            //invert pour que ce soit des points locaux et pas globaux
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if(i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }           
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    #endregion

    #region Targets
    /// <summary>
    /// Coroutine pour démarer la fonction Trouver les targers
    /// </summary>
    /// <param name="delay"></param>
    /// <returns>temps en s</returns>
    private IEnumerator FindTargetsCoroutineWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    /// <summary>
    /// Fonction qui va chercher les cibles visibles dans le radius et vérifier si il y'a un obstacle
    /// </summary>
    private void FindVisibleTargets()
    {
        //Clear la liste à chaque fois qu'on appelle la fonction
        visibleTargets.Clear();

        //Stocker les informations dans un tableau de Collider
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, TargetMask);

        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            //Récupérer le transform de la cible touchée
            Transform target = targetsInViewRadius[i].transform;

            //Check si la target est dans notre vision
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //Vérifier par l'angle si on vois le joueur
            if(Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //Vérifier la distance 
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Si il n'y a pas d'obstacles et que la cible est visible
                if(!Physics.Raycast(transform.position, dirToTarget, distToTarget, ObstacleMask))
                {
                    //Ajouter la cible dans la liste
                    visibleTargets.Add(target);
                }
            }
        }
    }
    #endregion

    #region Angle of FOW
    /// <summary>
    /// Angle et savoir si notre angle est global ou non
    /// </summary>
    /// <param name="angleInDegress"></param>
    /// <param name="angleIsGlobal"></param>
    /// <returns>Retourner le Vector3 en fonction des paramètres qu'on a mis</returns>
    public Vector3 DirFromAngle(float angleInDegress, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegress += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegress * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegress * Mathf.Deg2Rad));
    }

    #endregion

    #region Optimisations With Edge
    //Le but est de créer d'autres raycast quand y'en a un qui touche un obstacle et l'autre non et à chaque fois set un nouveau maximum quand ça touche pas
    //puis quand ça touche ça devient le minium
    //Donc trouver l'edge entre les 2 rays quand y'a une edge dans un obstacle et l'autre non
    public struct EdgeInfo
    {
        //Un de ces points sera le point le plus proche de l'edge sur l'obstacle
        public Vector3 PointA;
        public Vector3 PointB;

        public EdgeInfo(Vector3 p_pointA, Vector3 p_pointB)
        {
            PointA = p_pointA;
            PointB = p_pointB;
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;

        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        //cast array entre le min et max angle donc plus de fois on run la loop plus ce sera précis
        for(int i = 0; i < edgeResolutionIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

            //Vérifier si nouveau hit obstacle ou ancien
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    #endregion
}
