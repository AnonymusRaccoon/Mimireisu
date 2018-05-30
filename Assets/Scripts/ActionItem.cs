using UnityEngine;

public class ActionItem
{
    public DistanceJoint2D joint;
    public GameObject line;

    public ActionItem(DistanceJoint2D joint, GameObject line)
    {
        this.joint = joint;
        this.line = line;
    }
}
