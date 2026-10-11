#if UNITY_EDITOR
using System;
using UnityEngine;
public static class RisingExpansionBaseline
{
 public static void Validate()
 {
  int failures=0;
  try{RunnerArtValidation.Validate();}catch(Exception e){failures++;Debug.Log("BASELINE_RUNNER_FAILURE "+e.Message);}
  try{RisingLandscapeValidation.Validate();}catch(Exception e){failures++;Debug.Log("BASELINE_LANDSCAPE_FAILURE "+e.Message);}
  try{RisingNavigationValidation.Validate();}catch(Exception e){failures++;Debug.Log("BASELINE_NAVIGATION_FAILURE "+e.Message);}
  try{RisingCampaignValidation.Validate();}catch(Exception e){failures++;Debug.Log("BASELINE_CAMPAIGN_FAILURE "+e.Message);}
  if(failures>0)throw new InvalidOperationException("Expansion baseline failing contracts="+failures);
 }
}
#endif
