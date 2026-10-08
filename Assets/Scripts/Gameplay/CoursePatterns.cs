using UnityEngine;
namespace Kamilunavo.RisingSteps.Gameplay
{
 public static class CoursePatterns
 {
  public static Vector3[] Points(int realm,int seed){var random=new System.Random(seed);var points=new Vector3[13];
   for(int i=1;i<=12;i++){float x=realm==0?Mathf.Sin(i*.7f)*1.5f:realm==1?Mathf.Sin(i*1.1f)*2.6f:Mathf.Sin(i*.9f)*3;
    points[i]=new Vector3(x,points[i-1].y+.8f+(float)random.NextDouble()*.22f,points[i-1].z+3.6f+(float)random.NextDouble()*.3f);}
   return points;
  }
 }
}
