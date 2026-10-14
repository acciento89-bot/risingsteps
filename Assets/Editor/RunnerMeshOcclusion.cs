#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

// A mesh-only acceleration tree for real authored vertex-occlusion rays.
// It cannot touch the project's physics settings, scene colliders or controllers.
internal sealed class RunnerMeshOcclusion
{
 struct Triangle {public Vector3 A,B,C,Center;public Bounds Bounds;}
 sealed class Node {public Bounds Bounds;public Node Left,Right;public int Start,Count;}
 sealed class Order:IComparer<Triangle>{readonly int _axis;public Order(int axis){_axis=axis;}public int Compare(Triangle a,Triangle b)=>a.Center[_axis].CompareTo(b.Center[_axis]);}
 readonly Triangle[] _triangles;readonly Node _root;
 public RunnerMeshOcclusion(Transform root){
  var triangles=new List<Triangle>();foreach(var filter in root.GetComponentsInChildren<MeshFilter>(true))if(filter.gameObject.activeInHierarchy){var mesh=filter.sharedMesh;var v=mesh.vertices;var indices=mesh.triangles;var matrix=filter.transform.localToWorldMatrix;
   for(int i=0;i<indices.Length;i+=3){var a=matrix.MultiplyPoint3x4(v[indices[i]]);var b=matrix.MultiplyPoint3x4(v[indices[i+1]]);var c=matrix.MultiplyPoint3x4(v[indices[i+2]]);var bounds=new Bounds(a,Vector3.zero);bounds.Encapsulate(b);bounds.Encapsulate(c);triangles.Add(new Triangle{A=a,B=b,C=c,Center=(a+b+c)/3,Bounds=bounds});}
  }_triangles=triangles.ToArray();if(_triangles.Length==0)throw new InvalidOperationException("Original runner occlusion has no real mesh triangles");_root=Build(0,_triangles.Length);
 }
 Node Build(int start,int count){
  var bounds=_triangles[start].Bounds;for(int i=start+1;i<start+count;i++)bounds.Encapsulate(_triangles[i].Bounds);var node=new Node{Bounds=bounds,Start=start,Count=count};if(count<=8)return node;
  var size=bounds.size;int axis=size.x>=size.y&&size.x>=size.z?0:size.y>=size.z?1:2;Array.Sort(_triangles,start,count,new Order(axis));int half=count/2;node.Left=Build(start,half);node.Right=Build(start+half,count-half);return node;
 }
 public bool Occluded(Vector3 origin,Vector3 direction,float distance)=>Hit(_root,new Ray(origin,direction),distance);
 bool Hit(Node node,Ray ray,float distance){
  if(!node.Bounds.IntersectRay(ray,out float enter)||enter>distance)return false;if(node.Left!=null)return Hit(node.Left,ray,distance)||Hit(node.Right,ray,distance);
  for(int i=node.Start;i<node.Start+node.Count;i++){
   var triangle=_triangles[i];var first=triangle.B-triangle.A;var second=triangle.C-triangle.A;var cross=Vector3.Cross(ray.direction,second);float determinant=Vector3.Dot(first,cross);if(Mathf.Abs(determinant)<1e-9f)continue;
   float reciprocal=1/determinant;var from=ray.origin-triangle.A;float u=Vector3.Dot(from,cross)*reciprocal;if(u<0||u>1)continue;var q=Vector3.Cross(from,first);float v=Vector3.Dot(ray.direction,q)*reciprocal;if(v<0||u+v>1)continue;float length=Vector3.Dot(second,q)*reciprocal;if(length>.0001f&&length<=distance)return true;
  }return false;
 }
}
#endif
