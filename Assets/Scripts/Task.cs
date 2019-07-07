using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Task
{

    public static Tools Tools;

    /// <summary>
	/// Determines if given object can be seen between a source and target
	/// </summary>
	/// <param name="gm">the gameobject we want to know if we can see</param>
	/// <param name="pos">the source position</param>
	/// <param name="pos2">the target position</param>
	/// <returns>bool cansee</returns>
	public static bool CanSee(GameObject gm, Vector3 pos, Vector3 pos2)
    {
        RaycastHit hit;
        Vector3 rayDirection = pos2 - pos;
        if (Physics.Raycast(pos, rayDirection, out hit))
        {
            if (hit.transform.gameObject == gm)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks if there is anything obscuring vision between two points. If there is, returns the point it hit
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="pos2"></param>
    /// <returns></returns>
    public static Vector3 CanSee(Vector3 pos, Vector3 pos2, float distance = Mathf.Infinity)
    {
        RaycastHit hit;
        Vector3 rayDirection = pos2 - pos;
        if (Physics.Raycast(pos, rayDirection, out hit, distance))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Why am I writing these, I need sleep
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="radius"></param>
    /// <param name="damage"></param>
    /// <param name="id"></param>
    public static void DealDamageInRadius(Vector3 pos, float radius, int damage, UnityEngine.Networking.NetworkInstanceId id)
    {
        Collider[] hitColliders = Physics.OverlapSphere(pos, radius);

        List<HealthController> targets = new List<HealthController>();

        foreach (Collider col in hitColliders)
        {
            if (col.transform.root.GetComponent<ZombieController>())
            {
                ZombieController zc = col.transform.root.GetComponent<ZombieController>();

                if (!targets.Contains(zc))
                {
                    targets.Add(zc);
                }
            }

            if (col.transform.root.GetComponent<WallController>())
            {
                WallController wc = col.transform.root.GetComponent<WallController>();

                if (!targets.Contains(wc))
                {
                    targets.Add(wc);
                }
            }
        }

        if (targets.Count > 0)
        {
            foreach (HealthController z in targets.ToArray())
            {
                z.TakeDamage(damage, id);
            }
        }
    }

}
