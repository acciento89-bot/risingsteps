using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Two composed near banks make each natural realm a landscape rather than
 // a bare chain of landings. Every vertex stays outside the playable corridor.
 public static class NaturalRealmLandscape
 {
  public static void Build(Transform parent,int realm)
  {
   var root=new GameObject("OriginalNaturalLandscape"+realm);root.transform.SetParent(parent,false);var stone=LandscapePalette.Surface(parent,realm);var floor=LandscapePalette.Surface(parent,realm,1);
   for(int bank=0;bank<2;bank++){
    int sign=bank==0?-1:1;var center=new Vector3(sign*(10.4f+bank*1.3f),-3+bank*4.2f,18+bank*18);
    var cliff=LandscapeGeometry.Rock(root.transform,"NaturalNearTerrace",center,new Vector3(9,14,9),stone,realm*311+bank*61);var mesh=cliff.GetComponent<MeshFilter>().sharedMesh;var vertices=mesh.vertices;
    for(int i=0;i<vertices.Length;i++)if(vertices[i].y>4.7f)vertices[i].y=6.3f;mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();LandscapeGeometry.JoinNormals(mesh);
    var summit=center+Vector3.up*6.3f;var cap=MeshArt.Oval(root.transform,"NaturalBankCap",summit,new Vector3(8.2f,.15f,7.6f),floor,20);
    for(int ledge=0;ledge<3;ledge++){var shelf=LandscapeGeometry.BeveledBlock(root.transform,"NearCliffSediment",center+new Vector3((ledge-1)*1.5f,-3.8f+ledge*3.6f,-3.7f),new Vector3(4.6f+ledge*.4f,.33f,.75f),stone,.065f);LandscapeGeometry.StoneUv(shelf,.17f);}
    for(int detail=0;detail<(realm==1?1:3);detail++){
     var model=NaturalRealmArt.Instance(root.transform,realm);model.name="NaturalBankFeature";model.transform.localPosition=summit+new Vector3((detail-1)*2.2f,.09f,(detail%2)*1.8f-.5f);model.transform.localScale=Vector3.one*(realm==0?1.5f:realm==5?1.35f:1.1f);
    }
    if(realm==0||realm==3){var leaves=LandscapePalette.Surface(parent,realm,3);for(int group=0;group<3;group++)LandscapeBotany.LeafCluster(root.transform,"NearBankFoldedPlants",summit+new Vector3((group-1)*2.0f,.18f,1.6f),new Vector3(1.65f,.60f,1.3f),leaves,realm*17+group,16);}
    if(realm==7){MeshArt.Oval(root.transform,"NearBasaltLavaPool",summit+Vector3.up*.10f,new Vector3(5.7f,.08f,4.2f),LandscapePalette.Surface(parent,realm,7),24);}
    if(realm==8){MeshArt.Oval(root.transform,"NearCoastalLagoon",summit+new Vector3(-sign*1.0f,.10f,-1.2f),new Vector3(4.2f,.06f,3.1f),LandscapePalette.Surface(parent,realm,2),20);}
   }
   MeshArt.Batch(root);
  }
 }
}
