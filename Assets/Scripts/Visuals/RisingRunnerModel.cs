using UnityEngine;
namespace Kamilunavo.RisingSteps.Visuals
{
 // Original immutable Unity mesh/material/texture sub-assets; the animator rig
 // is in the companion prefab. Runtime style instances never mutate this source.
 public sealed class RisingRunnerModel:ScriptableObject
 {
  public string SourceDigest;
  public int Vertices,Triangles,OcclusionSamples;
  public float OcclusionMinimum,OcclusionMaximum;
 }
}
