#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using Kamilunavo.RisingSteps.Visuals;

// Deliberately separate from world validation so the coordinated gate can run it alone.
public static class RunnerArtValidation
{
 static int _checks;
 static void Check(bool value,string message){_checks++;if(!value)throw new InvalidOperationException("Runner art: "+message);}
 public static void Validate()
 {
  _checks=0;
  var host=new GameObject("RunnerArtFixture");
  var meshes=new HashSet<Mesh>();var materials=new HashSet<Material>();var textures=new HashSet<Texture>();
  try{
   var runner=RunnerArt.Build(host.transform);
   // This is the first failing contract against the toy baseline, before new geometry.
   Check(runner.LeftArm.Find("LeftElbow")!=null&&runner.RightArm.Find("RightElbow")!=null,"arms have independent elbow articulation");
   Check(runner.LeftLeg.Find("LeftKnee")!=null&&runner.RightLeg.Find("RightKnee")!=null,"legs have independent knee articulation");
   Check(runner.transform.parent==host.transform&&runner.Hips.parent==runner.transform&&runner.Head.parent==runner.Hips,"avatar binding is preserved");
   Check(host.GetComponentsInChildren<Collider>(true).Length==0,"visual character never adds physics colliders");
   var backpack=runner.Hips.Find("AdventurerPack");
   Check(backpack!=null,"approved rear adventurer silhouette has a shaped backpack");
   var packMesh=backpack.Find("CanvasPackBody").GetComponent<MeshFilter>().sharedMesh;
   Check(packMesh.bounds.size.y>.40f&&packMesh.bounds.size.z>.14f,"pack is genuine volumetric travel equipment");
   Check(backpack.Find("PackPouches")!=null&&runner.Hips.Find("PackShoulderStraps")!=null,"pack includes pouches and attached shoulder straps");
   foreach(var knee in new[]{runner.LeftLeg.Find("LeftKnee"),runner.RightLeg.Find("RightKnee")}){
    var skin=knee.Find("LowerLegSkin").GetComponent<MeshFilter>().sharedMesh.bounds;
    Check(skin.size.y>.19f,"rolled cargo exposes a modeled lower leg");
    var boot=knee.Find("HikingBootLast").GetComponent<MeshFilter>().sharedMesh.bounds;
    Check(boot.size.y>.20f&&boot.size.z>.34f,"adventure footwear has a substantial boot collar and sole");
   }
   var rearLocks=runner.Head.Find("RearSweptLocks");Check(rearLocks!=null,"rear hair contour has authored swept locks");
   var hairBounds=rearLocks.GetComponent<MeshFilter>().sharedMesh.bounds;
   Check(hairBounds.min.z<-.19f&&hairBounds.max.y>.28f,"rear locks extend the physical hair silhouette");

   int vertices=0,triangles=0,activeRenderers=0;
   foreach(var filter in host.GetComponentsInChildren<MeshFilter>(true)){
    var mesh=filter.sharedMesh;meshes.Add(mesh);vertices+=mesh.vertexCount;var points=mesh.vertices;var normals=mesh.normals;var indices=mesh.triangles;triangles+=indices.Length/3;
    Check(mesh.vertexCount>0&&points.Length==normals.Length,"mesh has vertex/normal coverage");
    foreach(var point in points)Check(float.IsFinite(point.x)&&float.IsFinite(point.y)&&float.IsFinite(point.z),"finite generated coordinates");
    foreach(var normal in normals)Check(float.IsFinite(normal.x)&&float.IsFinite(normal.y)&&float.IsFinite(normal.z)&&normal.sqrMagnitude>.5f,"valid generated lighting normals");
    for(int k=0;k<indices.Length;k+=3){Check(indices[k]>=0&&indices[k]<points.Length&&indices[k+1]>=0&&indices[k+1]<points.Length&&indices[k+2]>=0&&indices[k+2]<points.Length,"triangle indices within buffer");Check(Vector3.Cross(points[indices[k+1]]-points[indices[k]],points[indices[k+2]]-points[indices[k]]).sqrMagnitude>1e-12f,"no degenerate triangles");}
   }
   var face=runner.Head.Find("FaceSkin").GetComponent<MeshFilter>().sharedMesh.bounds;
   Check(face.size.x<.47f&&face.size.y>.38f&&face.size.y<.51f,"head uses a bounded humanoid profile");
   foreach(var renderer in host.GetComponentsInChildren<MeshRenderer>(true)){
    if(renderer.gameObject.activeInHierarchy)activeRenderers++;
    foreach(var material in renderer.sharedMaterials){Check(material!=null&&material.shader!=null,"all character materials resolve");materials.Add(material);if(material.mainTexture!=null)textures.Add(material.mainTexture);}
   }
   Check(vertices<10000&&triangles<10000,"bounded character geometry budget");
   Check(activeRenderers<=32&&materials.Count<=8,"bounded character rendering/material budget");
   foreach(var texture in textures)Check(texture.width*texture.height<=4096,"small character detail texture budget");
   var peerHost=new GameObject("StyleIsolationFixture");try{var peer=RunnerArt.Build(peerHost.transform);Color initial=peer.Hoodie.color;runner.Style(7);Check(peer.Hoodie.color==initial&&peer.Hoodie!=runner.Hoodie,"style material is isolated per avatar");}finally{UnityEngine.Object.DestroyImmediate(peerHost);}
   for(int style=0;style<8;style++){runner.Style(style);for(int crest=4;crest<8;crest++)Check(runner.Hips.Find("PremiumCrest"+crest).gameObject.activeSelf==(style==crest),"eight-style premium crest selection");}
   runner.Style(-1);Check(runner.Hoodie.color==new Color(.035f,.43f,.23f),"invalid negative style clamps to meadow");
   runner.Style(99);Check(runner.Hoodie.color==new Color(.035f,.055f,.14f),"invalid high style clamps to night");
   Debug.Log("RISING_RUNNER_BUDGET vertices="+vertices+" triangles="+triangles+" activeRenderers="+activeRenderers+" materials="+materials.Count+" textures="+textures.Count);
   UnityEngine.Object.DestroyImmediate(host);
   foreach(var mesh in meshes)Check(mesh==null,"generated meshes are released with avatar");
   foreach(var material in materials)Check(material==null,"character materials are released with avatar");
   foreach(var texture in textures)Check(texture==null,"character detail texture is released with avatar");
  }finally{if(host!=null)UnityEngine.Object.DestroyImmediate(host);}
  Debug.Log("RISING_RUNNER_ART_PASS checks="+_checks);
 }
 // Optional root-owned actual render gate. No scenes, native settings or global
 // lighting are saved. Capture does not claim concept acceptance by itself.
 public static void CaptureViews()
 {
  string directory=Environment.GetEnvironmentVariable("RISING_RUNNER_CAPTURE_DIR");
  if(string.IsNullOrEmpty(directory))directory="/Volumes/SSK SSD/Kamilunavo/Builds/RisingExpansion-20261011/Runner/views";
  System.IO.Directory.CreateDirectory(directory);
  var stage=new GameObject("IsolatedAdventurerCapture");stage.transform.position=new Vector3(0,-4096,0);
  var previousAmbientMode=RenderSettings.ambientMode;var previousAmbient=RenderSettings.ambientLight;
  var previousIntensity=RenderSettings.ambientIntensity;
  bool previousFog=RenderSettings.fog;
  var existingLights=UnityEngine.Object.FindObjectsByType<Light>();var lightStates=new bool[existingLights.Length];
  for(int i=0;i<existingLights.Length;i++)lightStates[i]=existingLights[i].enabled;
  RenderTexture target=null;
  var previousTarget=RenderTexture.active;Texture2D pixels=null;
  try{
   target=RenderTexture.GetTemporary(768,1024,24,RenderTextureFormat.ARGB32);
   foreach(var light in existingLights)light.enabled=false;
   var runner=RunnerArt.Build(stage.transform);runner.Style(0);
   foreach(var transform in stage.GetComponentsInChildren<Transform>(true))transform.gameObject.layer=31;
   var cameraObject=new GameObject("RunnerCaptureCamera",typeof(Camera));cameraObject.transform.SetParent(stage.transform,false);
   var camera=cameraObject.GetComponent<Camera>();camera.cullingMask=1<<31;camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.80f,.83f,.82f);camera.orthographic=true;camera.orthographicSize=1.08f;camera.nearClipPlane=.1f;camera.farClipPlane=12;camera.allowHDR=false;camera.targetTexture=target;
   var keyObject=new GameObject("RunnerCaptureKey",typeof(Light));keyObject.transform.SetParent(stage.transform,false);
   var key=keyObject.GetComponent<Light>();key.type=LightType.Directional;key.color=new Color(1,.91f,.79f);key.intensity=1.15f;key.cullingMask=1<<31;keyObject.transform.localRotation=Quaternion.Euler(30,-35,0);
   var fillObject=new GameObject("RunnerCaptureFill",typeof(Light));fillObject.transform.SetParent(stage.transform,false);
   var fill=fillObject.GetComponent<Light>();fill.type=LightType.Directional;fill.color=new Color(.76f,.85f,1);fill.intensity=.50f;fill.cullingMask=1<<31;fillObject.transform.localRotation=Quaternion.Euler(10,145,0);
   RenderSettings.fog=false;RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;RenderSettings.ambientLight=new Color(.54f,.57f,.59f);RenderSettings.ambientIntensity=1;
   pixels=new Texture2D(768,1024,TextureFormat.RGB24,false);
   string[] names={"front","three-quarter","rear","side"};Vector3[] views={new Vector3(0,1.04f,5),new Vector3(3.2f,1.22f,4.4f),new Vector3(0,1.1f,-5),new Vector3(5,1.08f,0)};
   for(int i=0;i<views.Length;i++){
    cameraObject.transform.localPosition=views[i];cameraObject.transform.LookAt(stage.transform.position+new Vector3(0,.97f,0));camera.Render();RenderTexture.active=target;
    pixels.ReadPixels(new Rect(0,0,768,1024),0,0);pixels.Apply();System.IO.File.WriteAllBytes(System.IO.Path.Combine(directory,"runner-"+names[i]+".png"),pixels.EncodeToPNG());
   }
   Debug.Log("RISING_RUNNER_CAPTURE_VIEWS "+directory);
  }finally{
   RenderTexture.active=previousTarget;RenderSettings.ambientMode=previousAmbientMode;RenderSettings.ambientLight=previousAmbient;RenderSettings.ambientIntensity=previousIntensity;
   RenderSettings.fog=previousFog;for(int i=0;i<existingLights.Length;i++)if(existingLights[i]!=null)existingLights[i].enabled=lightStates[i];
   if(pixels!=null)UnityEngine.Object.DestroyImmediate(pixels);UnityEngine.Object.DestroyImmediate(stage);if(target!=null)RenderTexture.ReleaseTemporary(target);
  }
 }
}
#endif
