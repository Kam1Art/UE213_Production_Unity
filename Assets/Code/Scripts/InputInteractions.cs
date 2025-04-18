using PathCreation.Examples;
using UnityEngine;

public class InputInteractions : MonoBehaviour
{
    public GameObject vehicle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        transform.position = new Vector3(100, 100, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Left()
    {
        PathFollower vehicleData = vehicle.GetComponent<PathFollower>();
        if (vehicleData.isAlive == true)
        {
            PathFollower pathFollower = vehicle.GetComponent<PathFollower>();
            float tempOffset = pathFollower.offset - pathFollower.widthOffset;
            pathFollower.offset = Mathf.Max(tempOffset, -pathFollower.widthOffset);
        }
        
        


    }

    public void Right()
    {
        PathFollower vehicleData = vehicle.GetComponent<PathFollower>();
        if (vehicleData.isAlive == true)
        {
            PathFollower pathFollower = vehicle.GetComponent<PathFollower>();
            float tempOffset = pathFollower.offset + pathFollower.widthOffset;
            pathFollower.offset = Mathf.Min(tempOffset, pathFollower.widthOffset);
        }
        
        
    }
}
