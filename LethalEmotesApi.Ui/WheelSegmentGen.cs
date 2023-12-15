using System;
using UnityEngine;
using UnityEngine.UI;

namespace LethalEmotesApi.Ui;

[ExecuteAlways]
public class WheelSegmentGen : MonoBehaviour
{
    public Material segmentMat;
    public ColorBlock colorBlock;
    [Range(1, 20)] public int segments = 8;
    [Range(-90, 90)] public float offset;
        
    [Range(0, 700)] public float maxRadius = 300;
    [Range(0, 699)] public float minRadius = 100;
    
    private void OnDrawGizmos()
    {
        if (minRadius >= maxRadius)
            minRadius = maxRadius - 1;
            
        var pos = transform.position;
        float degPer = (float)(Math.PI * 2 / segments);

        for (int i = 0; i < segments; i++)
        {
            float offsetRad = i * degPer + Mathf.Deg2Rad * offset;
            float centerRad = ((i * degPer + (i + 1) * degPer) / 2) + Mathf.Deg2Rad * offset;
                
            float maxX = (float)(pos.x + Math.Cos(offsetRad) * maxRadius);
            float maxY = (float)(pos.y + Math.Sin(offsetRad) * maxRadius);

            float minX = (float)(pos.x + Math.Cos(offsetRad) * minRadius);
            float minY = (float)(pos.y + Math.Sin(offsetRad) * minRadius);
                

            Vector3 start = new Vector3(minX, minY, pos.z);
            Vector3 end = new Vector3(maxX, maxY, pos.z);
                
            Gizmos.DrawLine(start, end);
        }
    }
}