using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfViewMerge : MonoBehaviour
{
    #region Variables
    public float viewRadius = 5f;
    [Range(0f, 360f)]
    public float viewAngle;

    [SerializeField] private Transform viewMeshPosition;

    [SerializeField] private int subDivisionsCount = 15;
    [SerializeField] public float hauteur = 2f;

    //Masques contenant ce qui est cibl� ou non
    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    //Liste qui va contenir tous les transforms des cibles dans le radius
    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();

    //Optimisation
    public int edgeResolutionIteration = 10;
    //threshold = limite
    //va servir pour diff�rencier plusieurs obstacles en m�me temps
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
        Debug.Log(viewMeshPosition.name);
        viewMeshPosition.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        viewMeshPosition.localRotation = Quaternion.Euler(0f, -180f, -90f);
        StartCoroutine(FindTargetsCoroutineWithDelay(.2f));
    }
    private void LateUpdate()
    {
        //Dans le late update car sinon �a attend que notre personnage ait finis de bouger
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
    /// Structure qui va tenir diff�rentes informations pour nous que l'on ait pas besoin de l'�crire � chaque fois
    /// </summary>
    public struct ViewCastInfo
    {
        //Si le raycast a touch� qqch
        public bool hit;
        //Le point que le ray � touch�
        public Vector3 point;
        //Lenght du ray
        public float distance;
        //angle auquel le ray a �t� lanc�
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
        //mesh resolution est basiquement le nb de ray qui va �tre cast par d�gr�
        int stepCount = Mathf.RoundToInt(viewAngle * MeshResolution);

        float stepAngleSize = viewAngle / stepCount;

        //Construire mesh
        List<Vector3> viewPoints = new List<Vector3>();

        //loop pour trouver tous les viewpoints, on va vouloir savoir si le viewcast pr�c�dant hit un obstacle ou pas
        ViewCastInfo oldViewCast = new ViewCastInfo();
        for (int i = 0; i < stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            //Si i > 0, chech si l'ancien viewcast hit un obstacle et le nouveau non ou l'ancien non et le nouveau oui
            //car on veut trouver l'edge
            if (i > 0)
            {
                //si c'est sup�rieur c'est � true
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;
                if (oldViewCast.hit != newViewCast.hit || oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);

                    //Ajouter les points de ces 2 edges � la list de viewpoint fournie � laquelle ils ont �t� set
                    //En gros ils n'ont toujours pas leurs valeurs de Vector3 par d�fauts
                    if (edge.PointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointA);
                    }
                    if (edge.PointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        //Triangles pareils chacuns des triangles ont 3 vertices
        triangles = new int[(subDivisionsCount * 2) * 3];

        //Le point d'origine ne bouge jamais donc vector0
        vertices[0] = Vector3.zero;

        //G�n�rer vertexs en fonctions des angles et du nombre de segments � cr�er
        for (int i = 0, n = subDivisionsCount - 1; i < subDivisionsCount; i++)
        {
            //Ratio correspond � l'avanc�e de i / par le nombre de subdivision -1
            float ratio = (float)i / n;

            //Utiliser ratio qui est une �quation qui �value a et b �cris a / b ou b n'�quivaut pas � 0
            float r = ratio * (Mathf.PI * 2f);

            //En calculant le ratio on va pouvoir s�parer chaques edges de mani�res �quivalentes (l� le placement des points)
            //� l'aide du cosinus et du sinus
            float x = Mathf.Cos(r) * viewRadius;
            float z = Mathf.Sin(r) * viewRadius;

            //ensuite on set la nouvelle position du point en fonction x et z
            vertices[i + 1] = new Vector3(x, 0f, z);

            //Pareil pour l'uv ou on l'update
            //uv[i + 1] = new Vector2(ratio, 0f);
        }

        //On cr�er en gros une nouvelle edge qui repart du point d'origine jusqu'a hauteur
        vertices[subDivisionsCount + 1] = new Vector3(0f, hauteur, 0f);

        //Pareil pour l'Uv on set nouvelle face
        //uv[subdivisions + 1] = new Vector2(0.5f, 1f);

        //Construction de la face du bas

        for (int i = 0, n = subDivisionsCount - 1; i < n; i++)
        {
            //Chaques triangles � 3 vertexs donc on multiplie par 3 et on vient cr�er le triangle de mani�re � faire 0,1,2 � chaque fois
            int offset = i * 3;
            triangles[offset] = 0;
            triangles[offset + 1] = i + 1;
            triangles[offset + 2] = i + 2;
        }

        //Construction des c�t�s
        //M�me chose que le dessus
        int bottomOffset = subDivisionsCount * 3;
        for (int i = 0, n = subDivisionsCount - 1; i < n; i++)
        {
            //Le changement est surtout ici
            int offset = i * 3 + bottomOffset;
            triangles[offset] = i + 1;
            triangles[offset + 1] = subDivisionsCount + 1;
            triangles[offset + 2] = i + 2;
        }

        //R�atribution des donn�es du mesh
        mesh.vertices = vertices;
        //mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    #endregion

    #region Targets
    /// <summary>
    /// Coroutine pour d�marer la fonction Trouver les targers
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
    /// Fonction qui va chercher les cibles visibles dans le radius et v�rifier si il y'a un obstacle
    /// </summary>
    private void FindVisibleTargets()
    {
        //Clear la liste � chaque fois qu'on appelle la fonction
        visibleTargets.Clear();

        //Stocker les informations dans un tableau de Collider
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, TargetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            //R�cup�rer le transform de la cible touch�e
            Transform target = targetsInViewRadius[i].transform;

            //Check si la target est dans notre vision
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            //V�rifier par l'angle si on vois le joueur
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                //V�rifier la distance 
                float distToTarget = Vector3.Distance(transform.position, target.position);

                //Si il n'y a pas d'obstacles et que la cible est visible
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, ObstacleMask))
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
    /// <returns>Retourner le Vector3 en fonction des param�tres qu'on a mis</returns>
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
    //Le but est de cr�er d'autres raycast quand y'en a un qui touche un obstacle et l'autre non et � chaque fois set un nouveau maximum quand �a touche pas
    //puis quand �a touche �a devient le minium
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

        //cast array entre le min et max angle donc plus de fois on run la loop plus ce sera pr�cis
        for (int i = 0; i < edgeResolutionIteration; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

            //V�rifier si nouveau hit obstacle ou ancien
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
