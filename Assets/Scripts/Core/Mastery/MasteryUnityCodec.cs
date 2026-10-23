using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Kamilunavo.RisingSteps.Core.Mastery
{
 public sealed class MasteryUnityCodec:IMasteryCodec
 {
  const string Format="kamilunavo.risingsteps.mastery";
  [Serializable] sealed class Envelope {public string Format,Payload,Digest;public int Schema;}
  // JsonUtility's numeric double parser is not bit-exact. Store IEEE-754 bits
  // as fixed lowercase hex strings, so JSON never parses measured numbers.
  [Serializable] sealed class SampleWire {public string Seconds,X,Y,Z;}
  [Serializable] sealed class AttemptWire
  {
   public string ID="",RecordedUtc="",Seconds="";public MasteryKey Key;
   public string[] Splits=new string[0];public int Falls,Perfects;public SampleWire[] Samples=new SampleWire[0];
  }
  [Serializable] sealed class EntryWire
  {
   public MasteryKey Key;public int TotalAttempts,ImprovementCount;public AttemptWire[] History;
   public bool HasBest;public AttemptWire Best=new AttemptWire();
  }
  [Serializable] sealed class BookWire {public int Schema,Revision;public bool GuideEnabled;public EntryWire[] Entries;}
  static string Bits(double value)=>unchecked((ulong)BitConverter.DoubleToInt64Bits(value)).ToString("x16",CultureInfo.InvariantCulture);
  static double Number(string text)
  {
   ulong bits;
   if(text==null||text.Length!=16||!ulong.TryParse(text,NumberStyles.AllowHexSpecifier,CultureInfo.InvariantCulture,out bits)||bits.ToString("x16",CultureInfo.InvariantCulture)!=text)throw new FormatException("Invalid measured number representation");
   return BitConverter.Int64BitsToDouble(unchecked((long)bits));
  }
  static AttemptWire Wire(MasteryAttempt value)
  {
   var result=new AttemptWire{ID=value.ID,RecordedUtc=value.RecordedUtc,Key=value.Key,Seconds=Bits(value.Seconds),Falls=value.Falls,Perfects=value.Perfects,Splits=new string[value.Splits.Length],Samples=new SampleWire[value.Samples.Length]};
   for(int i=0;i<result.Splits.Length;i++)result.Splits[i]=Bits(value.Splits[i]);
   for(int i=0;i<result.Samples.Length;i++){var sample=value.Samples[i];result.Samples[i]=new SampleWire{Seconds=Bits(sample.Seconds),X=Bits(sample.X),Y=Bits(sample.Y),Z=Bits(sample.Z)};}
   return result;
  }
  static MasteryAttempt Model(AttemptWire value)
  {
   if(value==null||value.Splits==null||value.Samples==null)throw new FormatException("Missing trial fields");
   var result=new MasteryAttempt{ID=value.ID,RecordedUtc=value.RecordedUtc,Key=value.Key,Seconds=Number(value.Seconds),Falls=value.Falls,Perfects=value.Perfects,Splits=new double[value.Splits.Length],Samples=new MasterySample[value.Samples.Length]};
   for(int i=0;i<result.Splits.Length;i++)result.Splits[i]=Number(value.Splits[i]);
   for(int i=0;i<result.Samples.Length;i++){var sample=value.Samples[i];if(sample==null)throw new FormatException("Missing sample");result.Samples[i]=new MasterySample(Number(sample.Seconds),Number(sample.X),Number(sample.Y),Number(sample.Z));}
   return result;
  }
  static BookWire Wire(MasteryBook value)
  {
   var result=new BookWire{Schema=value.Schema,Revision=value.Revision,GuideEnabled=value.GuideEnabled,Entries=new EntryWire[value.Entries.Length]};
   for(int i=0;i<result.Entries.Length;i++){var entry=value.Entries[i];var target=new EntryWire{Key=entry.Key,TotalAttempts=entry.TotalAttempts,ImprovementCount=entry.ImprovementCount,History=new AttemptWire[entry.History.Length],HasBest=entry.Best!=null};for(int j=0;j<target.History.Length;j++)target.History[j]=Wire(entry.History[j]);if(target.HasBest)target.Best=Wire(entry.Best);result.Entries[i]=target;}
   return result;
  }
  static MasteryBook Model(BookWire value)
  {
   if(value==null||value.Entries==null)throw new FormatException("Missing journal fields");
   var result=new MasteryBook{Schema=value.Schema,Revision=value.Revision,GuideEnabled=value.GuideEnabled,Entries=new MasteryEntry[value.Entries.Length]};
   for(int i=0;i<result.Entries.Length;i++){var entry=value.Entries[i];if(entry==null||entry.History==null)throw new FormatException("Missing journal entry");var target=new MasteryEntry{Key=entry.Key,TotalAttempts=entry.TotalAttempts,ImprovementCount=entry.ImprovementCount,History=new MasteryAttempt[entry.History.Length],Best=entry.HasBest?Model(entry.Best):null};for(int j=0;j<target.History.Length;j++)target.History[j]=Model(entry.History[j]);result.Entries[i]=target;}
   return result;
  }
  public string Encode(MasteryBook book)
  {
   MasteryBookRules.Validate(book);string payload=JsonUtility.ToJson(Wire(book));
   return JsonUtility.ToJson(new Envelope{Format=Format,Schema=1,Payload=payload,Digest=Hash(payload)});
  }
  public MasteryBook Decode(string text)
  {
   if(string.IsNullOrEmpty(text)||text.Length>MasteryBookRules.MaximumBytes)throw new FormatException("Invalid journal envelope");
   var envelope=JsonUtility.FromJson<Envelope>(text);
   if(envelope==null||envelope.Format!=Format||envelope.Schema!=1||string.IsNullOrEmpty(envelope.Payload)||envelope.Digest!=Hash(envelope.Payload))throw new FormatException("Unsupported or damaged journal envelope");
   var book=Model(JsonUtility.FromJson<BookWire>(envelope.Payload));MasteryBookRules.Validate(book);
   // Canonical reconstruction rejects unknown/missing/defaulted fields too.
   if(JsonUtility.ToJson(Wire(book))!=envelope.Payload)throw new FormatException("Incomplete or noncanonical journal payload");
   return book;
  }
  static string Hash(string text){using(var sha=SHA256.Create()){var bytes=sha.ComputeHash(Encoding.UTF8.GetBytes(text));var result=new StringBuilder(64);foreach(byte value in bytes)result.Append(value.ToString("x2"));return result.ToString();}}
 }
}
