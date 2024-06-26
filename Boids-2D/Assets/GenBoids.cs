using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GenBoids : MonoBehaviour
{
    GameObject goal = null;
    public List<GameObject> goals;

    public List<long> timeToGoal = new List<long>();
    long goalStartTime;

    public float percentFlocked;

    public long timeToAllFlock;
    long flockStartTime;

    public int goalCollisions = 0;

    public float flockMaxDist;

    GameObject canvas;
    public float canvasWidth;
    public float canvasHeight;

    public float sepLength = 10;
    public float alignLength = 20;
    public float cohesLength = 20;
    public float droneSpeed = 0.1f;

    public float accel;

    Stopwatch timer = new Stopwatch();

    List<GameObject> boids;

    public float obstScale = 1;
    public float sepScale = 0.25f;
    public float goalScale = 0;
    public float aliScale = 0.125f;
    public float cohScale = 0.125f;
    public float prevMovScale = 1;

    public int boidsToGenerate = 1;

    GameObject flockCentre;

    System.Random rnd = new System.Random();


    Color32 red = new Color32(255, 0, 0, 255);
    Color32 white = new Color32(255, 255, 255, 255);
    Color32 gray = new Color32(255, 255, 255, 30);
    Color32 lightGray = new Color32(255, 255, 255, 20);
    Color32 transparent = new Color32(255, 255, 255, 0);
    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        canvasWidth = canvas.transform.localScale.x;
        canvasHeight = canvas.transform.localScale.y;
        boids = new List<GameObject>();

        // Add back in to create moving goal
        /*goal = GameObject.Instantiate(GameObject.Find("DeadGoal"));
        SpriteRenderer gr = goal.GetComponent<SpriteRenderer>();
        gr.color = new Color32(0, 255, 0, 255);
        goal.name = "Goal";
        goal.tag = "goal";
        goal.transform.position = new Vector2(0, 0);
        goal.AddComponent<GoalScript>();
        goal.GetComponent<GoalScript>().canvas = canvas;
        goal.GetComponent<GoalScript>().randoPos();*/

        flockCentre = GameObject.Find("Flock Circle");


        // add back in for auto run
        /*boidsToGenerate = 50;
        generateBoids();*/
    }
    // Update is called once per frame
    void Update()
    {
        canvas.transform.localScale = new Vector3(canvasWidth, canvasHeight, 1f);

        getMetrics(); // Get current metrics for testing

        //goals = canvas.GetComponent<CanvasScript>().goals; // Get current goals

        foreach (GameObject boid in boids)
        {
            var boidScript = boid.GetComponent<BoidScript>();
            // Param assignment
                if (goal != null)
                {
                    boidScript.goalPos = boid.GetComponent<BoidScript>().goal.transform.position;
                }
                //boidScript.goal = goal;

                boidScript.sepLength = sepLength;
                boidScript.alignLength = alignLength;
                boidScript.cohesLength = cohesLength;
                boidScript.droneSpeed = droneSpeed;

                boidScript.accel = accel;

                boidScript.obstScale = obstScale;
                boidScript.sepScale = sepScale;
                boidScript.goalScale = goalScale;
                boidScript.aliScale = aliScale;
                boidScript.cohScale = cohScale;
                boidScript.prevMovScale = prevMovScale;
            // Param assignment
        }
    }
    public void generateBoids(int genNum, bool randPos, Vector2 position)
    {
        timer.Start();
        goalStartTime = timer.ElapsedMilliseconds;
        //UnityEngine.Debug.Log("Generating " + genNum + " Boids...");
        if (GameObject.Find("Main"))
        {
            //UnityEngine.Debug.Log("Main Boid already exists!");
            genNum++;
        } else
        {
            //UnityEngine.Debug.Log("Generating main Boid...");

            if (randPos) {
                position.x = rnd.Next((int)(((canvasWidth / 2) * -1) + 10), (int)((canvasWidth / 2) - 10));
                position.y = rnd.Next((int)(((canvasHeight / 2) * -1) + 10), (int)((canvasHeight / 2) - 10));
            }
            createBoid("Main", red, gray, lightGray, position);
        }
        for (int i = 0; i < genNum-1; i++)
        {
            if (randPos)
            {
                position.x = rnd.Next((int)(((canvasWidth / 2) * -1) + 10), (int)((canvasWidth / 2) - 10));
                position.y = rnd.Next((int)(((canvasHeight / 2) * -1) + 10), (int)((canvasHeight / 2) - 10));
            }
            createBoid(("Gen" + (boids.Count-1)), white, transparent, transparent, position);
        }
        if (goal != null)
        {
            goal.GetComponent<GoalScript>().findBoids();
        }
    }
    public void deleteAllBoids()
    {
        if (goal != null)
        {
            goal.GetComponent<GoalScript>().boids.Clear();
        }
        GameObject[] allRadii = GameObject.FindGameObjectsWithTag("radius");
        GameObject[] allHeadings = GameObject.FindGameObjectsWithTag("heading");
        int num = boids.Count;
        foreach (GameObject b in boids)
        {
            Destroy(b);
        }
        foreach (GameObject r in allRadii)
        {
            Destroy(r);
        }
        foreach (GameObject h in allHeadings)
        {
            Destroy(h);
        }
        UnityEngine.Debug.Log("Deleted " + num + " Boids");
        boids.Clear();
    }
    public void createBoid(string name, Color32 boidColour, Color32 sepColour, Color32 groupColour, Vector2 startPos)
    {
        GameObject newDrone = GameObject.Instantiate(GameObject.Find("Dead")); // Create new boid from template
        SpriteRenderer sr = newDrone.GetComponent<SpriteRenderer>();
        newDrone.name = name;
        sr.color = boidColour;
        newDrone.tag = "drone";
        newDrone.AddComponent<BoidScript>(); // Assign script to boid
        var boidScript = newDrone.GetComponent<BoidScript>();
        newDrone.transform.position = startPos;

        if (name == "Main") // If main boid, then create radii
        {
            GameObject newDroneRadius = GameObject.Instantiate(GameObject.Find("DeadRadius"));
            SpriteRenderer rsr = newDroneRadius.GetComponent<SpriteRenderer>();
            rsr.color = sepColour;
            newDroneRadius.tag = "radius";
            newDroneRadius.name = newDrone.name + "Radius";
            boidScript.scanCircle = newDroneRadius;

            GameObject newDroneGroupRadius = GameObject.Instantiate(GameObject.Find("DeadGroupRadius"));
            SpriteRenderer grsr = newDroneGroupRadius.GetComponent<SpriteRenderer>();
            grsr.color = groupColour;
            newDroneGroupRadius.tag = "radius";
            newDroneGroupRadius.name = newDrone.name + "GroupRadius";
            boidScript.groupCircle = newDroneGroupRadius;
        }

        
        
        
        boidScript.canvas = canvas;
        // Param assignment
            boidScript.sepLength = sepLength;
            boidScript.alignLength = alignLength;
            boidScript.cohesLength = cohesLength;
            boidScript.droneSpeed = droneSpeed;

            boidScript.accel = accel;

            boidScript.obstScale = obstScale;
            boidScript.sepScale = sepScale;
            boidScript.goalScale = goalScale;
            boidScript.aliScale = aliScale;
            boidScript.cohScale = cohScale;
            boidScript.prevMovScale = prevMovScale;
        // Param assignment
        if (goal != null)
        {
            newDrone.GetComponent<BoidScript>().goalPos = goal.GetComponent<GoalScript>().transform.position; // Tell boid where the goal is
        }
        newDrone.GetComponent<BoidScript>().goal = goal;

        foreach (GameObject b in boids) // Tell all boids to search for neighbours
        {
            b.GetComponent<BoidScript>().findObjects("drone");
        }
        boids.Add(newDrone);
    }
    public void deleteAllObstacles()
    {
        GameObject canvas = GameObject.Find("Canvas");
        var script = canvas.GetComponent<CanvasScript>();
        foreach (GameObject line in script.lines)
        {
            Destroy(line);
        }
        script.lines.Clear();
    }
    public void getMetrics()
    {
        long time = timer.ElapsedMilliseconds;

        // Flock centre visualization
        flockCentre.transform.position = avgPos();
        flockCentre.transform.localScale = new Vector2(flockMaxDist, flockMaxDist);

        // Time to goal metric
        if (goalScale == 0)
        {
            goalStartTime = time;
        } else
        {
            if (goal != null)
            {
                goal = GameObject.Find("Goal");
                var goalScript = goal.GetComponent<GoalScript>();
                if (goalScript.numCollisions > goalCollisions)
                {
                    goalCollisions = goalScript.numCollisions;

                    //UnityEngine.Debug.Log("Time to Goal: " + time + "ms");
                    timeToGoal.Add(time - goalStartTime);
                    goalStartTime = time;
                }
            }
        }

        // Percent flocked metric
        percentFlocked = calcPercentFlocked();

        // Time to flock metric
        if (timeToAllFlock == 0)
        {
            if (percentFlocked == 1)
            {
                timeToAllFlock = time - flockStartTime;
            }
        }
    }
    public float calcPercentFlocked()
    {
        float numFlocked = 0;
        foreach(GameObject boid in boids)
        {
            float dist = (boid.transform.position - flockCentre.transform.position).magnitude;
            if (dist < (flockMaxDist/2))
            {
                numFlocked++;
            }
        }
        numFlocked /= boids.Count;
        return numFlocked;
    }
    public bool isFlocked()
    {
        float max = 0;
        for (int i = 0; i < boids.Count-1; i++)
        {
            for (int j = i+1; j < boids.Count; j++)
            {
                float dist = (boids[j].transform.position - boids[i].transform.position).magnitude;
                if (dist > max)
                {
                    max = dist;
                    if (max > flockMaxDist)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
    public Vector3 avgPos()
    {
        Vector3 centreOfMass = new Vector3(0, 0, 0);
        foreach (GameObject boid in boids)
        {
            centreOfMass += boid.transform.position;
        }
        if (boids.Count > 0)
        {
            centreOfMass /= boids.Count;
        }
        return centreOfMass;
    }
    public void resetMetrics()
    {
        long time = timer.ElapsedMilliseconds;

        timeToGoal.Clear();
        goalStartTime = time;

        timeToAllFlock = 0;
        flockStartTime = time;

        percentFlocked = 0;
    }
    float distSqr(Vector2 a, Vector2 b)
    {
        float dx = a.x - b.x;
        float dy = a.y - b.y;
        float d = (dx * dx) + (dy * dy);
        return d;
    }
    public void assignBoids()
    {
        goals = canvas.GetComponent<CanvasScript>().goals;
        // Get average position of swarm
        Vector3 avgPosition = avgPos();

        // Sort goals by largest distance to avgPosition
        List<(float, int)> dist = new List<(float, int)>();
        // Find distances from goal points to avgPosition
        for (int i = 0; i < goals.Count; i++)
        {
            dist.Add((distSqr(goals[i].transform.position, avgPosition), i));
        }
        // Sort goals by largest distance first
        dist.Sort((a, b) => b.Item1.CompareTo(a.Item1));
        List<(float, int)> order = new List<(float, int)>();
        order = dist;

        // Select closest boid for each goal node
        List<int> selected = new List<int>();
        for (int i = 0; i < order.Count; i++)
        {
            List<(float, int)> dist2 = new List<(float, int)>();
            // Find distance from goal node to each boid
            for (int j = 0; j < boids.Count; j++)
            {
                dist2.Add((distSqr(goals[order[i].Item2].transform.position, boids[j].transform.position), j));
            }

            // Find closest boid that hasn't been selected yet
            dist2.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            int closetBoid = dist2[0].Item2;
            while (selected.Contains(closetBoid))
            {
                dist2.Remove(dist2[0]);
                closetBoid = dist2[0].Item2;
            }

            // Set boid's goal + add boid to selected list
            boids[closetBoid].GetComponent<BoidScript>().goalPos = goals[order[i].Item2].transform.position;
            boids[closetBoid].GetComponent<BoidScript>().goal = goals[order[i].Item2];
            print("assigned goal[" + order[i].Item2 + "] to boid[" + closetBoid + "]");
            selected.Add(closetBoid);
        }
    }
}
