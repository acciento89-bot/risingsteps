using System;
using System.Globalization;

namespace Kamilunavo.RisingSteps.Core.Mastery
{
 [Serializable] public struct MasteryKey : IEquatable<MasteryKey>
 {
  public int Realm,Level; public string Geometry;
  public MasteryKey(int realm,int level,string geometry){Realm=realm;Level=level;Geometry=geometry;Validate();}
  public void Validate(){if(Realm<0||Realm>=10||Level<0||Level>=10||Geometry==null||Geometry.Length!=64)throw new ArgumentException("Invalid course key");foreach(char c in Geometry)if(!((c>='a'&&c<='f')||(c>='0'&&c<='9')))throw new ArgumentException("Invalid geometry signature");}
  public bool Equals(MasteryKey other)=>Realm==other.Realm&&Level==other.Level&&Geometry==other.Geometry;
  public override bool Equals(object other)=>other is MasteryKey&&Equals((MasteryKey)other);
  public override int GetHashCode()=>((Realm*397)^Level)*397^(Geometry??"").GetHashCode();
 }
 [Serializable] public struct MasterySample
 {
  public double Seconds,X,Y,Z;
  public MasterySample(double seconds,double x,double y,double z){Seconds=seconds;X=x;Y=y;Z=z;}
 }
 [Serializable] public sealed class MasteryAttempt
 {
  public string ID,RecordedUtc;public MasteryKey Key;public double Seconds;public double[] Splits=new double[0];public int Falls,Perfects;public MasterySample[] Samples=new MasterySample[0];
  public MasteryAttempt Copy(bool samples=true)=>new MasteryAttempt{ID=ID,RecordedUtc=RecordedUtc,Key=Key,Seconds=Seconds,Splits=(double[])Splits.Clone(),Falls=Falls,Perfects=Perfects,Samples=samples?(MasterySample[])Samples.Clone():new MasterySample[0]};
  public void Validate()
  {
   Key.Validate();Guid parsed;DateTime at;
   if(ID==null||!Guid.TryParseExact(ID,"N",out parsed)||parsed.ToString("N")!=ID||!DateTime.TryParseExact(RecordedUtc,"O",CultureInfo.InvariantCulture,DateTimeStyles.RoundtripKind,out at)||at.Kind!=DateTimeKind.Utc||!MasteryBounds.Finite(Seconds)||Seconds<=0||Seconds>3600||Falls<0||Falls>1000000||Perfects<0||Perfects>12||Splits==null||Splits.Length!=12||Samples==null)throw new ArgumentException("Invalid trial record");
   double previous=0;foreach(double split in Splits){if(!MasteryBounds.Finite(split)||split<=previous||split>Seconds+1e-9)throw new ArgumentException("Invalid landing splits");previous=split;}
   if(Samples.Length>0){MasteryBounds.ValidateSamples(Samples);if(Math.Abs(Samples[Samples.Length-1].Seconds-Seconds)>1e-6||Seconds>300)throw new ArgumentException("Incomplete guide coverage");}
  }
 }
 public static class MasteryBounds
 {
  public const int MaxSamples=1500;
  public static bool Finite(double value)=>!double.IsNaN(value)&&!double.IsInfinity(value);
  public static bool Pose(double x,double y,double z)=>Finite(x)&&Finite(y)&&Finite(z)&&Math.Abs(x)<=10000&&Math.Abs(y)<=10000&&Math.Abs(z)<=10000;
  public static void ValidateSamples(MasterySample[] samples)
  {
   if(samples==null||samples.Length<2||samples.Length>MaxSamples||samples[0].Seconds!=0)throw new ArgumentException("Invalid guide samples");
   double previous=-1;foreach(var sample in samples){if(!Finite(sample.Seconds)||sample.Seconds<=previous||sample.Seconds>300||!Pose(sample.X,sample.Y,sample.Z))throw new ArgumentException("Invalid guide sample");previous=sample.Seconds;}
  }
 }
 public sealed class MasteryGuide
 {
  readonly MasteryKey _key;readonly MasterySample[] _samples;
  public MasteryGuide(MasteryKey key,MasterySample[] samples){key.Validate();MasteryBounds.ValidateSamples(samples);_key=key;_samples=(MasterySample[])samples.Clone();}
  public MasterySample At(double seconds,MasteryKey key)
  {
   if(!_key.Equals(key)||!MasteryBounds.Finite(seconds))throw new ArgumentException("Incompatible guide request");
   if(seconds<=0)return _samples[0];if(seconds>=_samples[_samples.Length-1].Seconds)return _samples[_samples.Length-1];
   int low=0,high=_samples.Length-1;while(high-low>1){int middle=(low+high)/2;if(_samples[middle].Seconds<=seconds)low=middle;else high=middle;}
   var a=_samples[low];var b=_samples[high];double f=(seconds-a.Seconds)/(b.Seconds-a.Seconds);
   return new MasterySample(seconds,a.X+(b.X-a.X)*f,a.Y+(b.Y-a.Y)*f,a.Z+(b.Z-a.Z)*f);
  }
 }
}
