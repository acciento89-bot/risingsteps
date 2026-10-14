using UnityEngine;
namespace Kamilunavo.RisingSteps.Gameplay
{
 public static class CoursePatterns
 {
  public static float TempleRoofWidth(int index)=>index==12?6.0f:index%4==0?5.2f:4.2f;
  // Complete cornices extend beyond the scored roof. Separate the actual broad
  // masonry models, not merely their collision fans. Preserve X/rise/seed and
  // checkpoint indices; Temple forward spacing deliberately changes.
  static void SeparateTemple(Vector3[] points){float previous=points[0].z;
   for(int i=1;i<points.Length;i++){float original=points[i].z,stride=original-previous;previous=original;float minimum=(TempleRoofWidth(i-1)+TempleRoofWidth(i))*.5f+1.50f;points[i].z=points[i-1].z+Mathf.Max(stride,minimum)+Mathf.Clamp(stride-3.35f,0,.55f)*.35f;}
  }
  public static Vector3[] LevelPoints(int realm,int level){realm=Mathf.Clamp(realm,0,Kamilunavo.RisingSteps.Core.RisingCampaign.RealmCount-1);level=Mathf.Clamp(level,0,9);if(level==0&&realm<3)return Points(realm,260906+realm);
   var random=new System.Random(90101+realm*1301+level*173);var points=new Vector3[13];float amplitude=1.25f+level*.14f+realm*.15f;
   for(int i=1;i<=12;i++){float wave=level%3==0?Mathf.Sin(i*.55f+realm*.3f):level%3==1?Mathf.Sin(i*.92f+realm*.4f):Mathf.Sin(i*.7f)*Mathf.Cos(i*.27f+realm*.4f);
    float nextX=wave*amplitude;nextX=Mathf.Clamp(nextX,points[i-1].x-1.65f,points[i-1].x+1.65f);float rise=.67f+level*.024f+(float)random.NextDouble()*.22f;float stride=3.35f+level*.06f+(float)random.NextDouble()*.36f;
    points[i]=new Vector3(nextX,points[i-1].y+rise,points[i-1].z+stride);}
   if(realm==2)SeparateTemple(points);return points;
  }
  public static Vector3[] Points(int realm,int seed){var random=new System.Random(seed);var points=new Vector3[13];
   for(int i=1;i<=12;i++){float x=realm==0?Mathf.Sin(i*.7f)*1.5f:realm==1?Mathf.Sin(i*1.1f)*2.6f:Mathf.Sin(i*.9f)*3;
    points[i]=new Vector3(x,points[i-1].y+.8f+(float)random.NextDouble()*.22f,points[i-1].z+3.6f+(float)random.NextDouble()*.3f);}
   if(realm==2)SeparateTemple(points);return points;
  }
 }
}
