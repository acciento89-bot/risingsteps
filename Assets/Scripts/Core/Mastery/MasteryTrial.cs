using System;
using System.Collections.Generic;

namespace Kamilunavo.RisingSteps.Core.Mastery
{
 public sealed class MasteryTrial
 {
  readonly MasteryKey _key;readonly string _id;readonly List<MasterySample> _samples=new List<MasterySample>(MasteryBounds.MaxSamples);readonly double[] _splits=new double[12];
  int _step,_falls,_perfects;bool _anchored,_ended,_coverage=true;double _elapsed;
  public double Elapsed=>_elapsed;public int Step=>_step;public bool Ended=>_ended;
  public MasteryTrial(MasteryKey key,string id,int startStep,bool daily)
  {key.Validate();Guid parsed;if(startStep!=0||daily||id==null||!Guid.TryParseExact(id,"N",out parsed)||parsed.ToString("N")!=id)throw new ArgumentException("A fresh campaign trial is required");_key=key;_id=id;}
  public void Advance(double delta,bool active)
  {
   if(!MasteryBounds.Finite(delta)||delta<0)throw new ArgumentException("Invalid active delta");if(_ended)return;
   if(!active){_anchored=false;return;}if(!_anchored){_anchored=true;return;}
   if(delta>3600-_elapsed){_ended=true;_coverage=false;return;}_elapsed+=delta;
  }
  public bool Capture(double x,double y,double z,bool force=false)
  {
   if(!MasteryBounds.Pose(x,y,z))throw new ArgumentException("Invalid recorded position");if(_ended||!_anchored)return false;
   if(!_coverage)return false;if(_elapsed>300){_coverage=false;return false;}
   var sample=new MasterySample(_elapsed,x,y,z);
   if(_samples.Count>0){var previous=_samples[_samples.Count-1];if(_elapsed==previous.Seconds){if(force)_samples[_samples.Count-1]=sample;return force;}if(!force&&_elapsed-previous.Seconds<.2-1e-9)return false;}
   if(_samples.Count>=MasteryBounds.MaxSamples){_coverage=false;return false;}_samples.Add(sample);return true;
  }
  public bool Land(int step,bool perfect)
  {if(_ended||!_anchored||step!=_step+1||step>12||_elapsed<=(_step>0?_splits[_step-1]:0))return false;_splits[_step]=_elapsed;_step++;if(perfect)_perfects++;return true;}
  public void Fall(){if(!_ended&&_falls<1000000)_falls++;}
  public void Cancel(){_ended=true;_coverage=false;}
  public MasteryAttempt Complete(string recordedUtc,double x,double y,double z)
  {
   if(_ended||_step!=12||_elapsed<=0)return null;
   Capture(x,y,z,true);
   var result=new MasteryAttempt{ID=_id,RecordedUtc=recordedUtc,Key=_key,Seconds=_elapsed,Splits=(double[])_splits.Clone(),Falls=_falls,Perfects=_perfects,Samples=_coverage&&_samples.Count>=2&&_samples[0].Seconds==0?_samples.ToArray():new MasterySample[0]};
   result.Validate();_ended=true;return result;
  }
 }
}
