using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Kamilunavo.RisingSteps.Core.Mastery;

// This fixture proves archive I/O behavior with an injected codec. It does not
// substitute for the separate Unity JsonUtility round-trip validation.
public static class MasteryArchiveTests
{
 static int checks;
 static void Check(bool ok,string message){checks++;if(!ok)throw new Exception("FAIL "+message);}
 static void Reject(Action action,string message){checks++;try{action();}catch(Exception){return;}throw new Exception("FAIL expected rejection: "+message);}
 sealed class FixtureCodec:IMasteryCodec
 {
  readonly Dictionary<string,MasteryBook> books=new Dictionary<string,MasteryBook>();
  public string Encode(MasteryBook book){string token="fixture:"+book.Revision+":"+book.GuideEnabled;books[token]=book.Copy();return token;}
  public MasteryBook Decode(string text){MasteryBook book;if(!books.TryGetValue(text,out book))throw new FormatException("Malformed fixture");return book.Copy();}
 }
 sealed class BrokenWriter:IMasteryAtomicWriter
 {
  readonly bool after;
  public BrokenWriter(bool afterCommit){after=afterCommit;}
  public void Write(string path,byte[] bytes){if(after)File.WriteAllBytes(path,bytes);throw new IOException("Injected disk failure");}
 }
 public static int Main()
 {
  string directory=Path.Combine(Path.GetTempPath(),"rising-mastery-test-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(directory);
  try
  {
   string path=Path.Combine(directory,"journal.json"),legacy=Path.Combine(directory,"legacy-profile");File.WriteAllText(legacy,"old profile byte sentinel");
   var codec=new FixtureCodec();var archive=new MasteryFileArchive(path,codec);
   var empty=archive.Load();Check(empty.Writable&&empty.Book.Revision==0&&!File.Exists(path),"missing journal loads empty without writing");
   var first=MasteryBookRules.SetGuideEnabled(empty.Book,true);archive.Save(first,0);
   Check(archive.Load().Book.GuideEnabled&&archive.Load().Book.Revision==1,"actual file survives fresh archive instance");
   var savedBytes=File.ReadAllBytes(path);Reject(()=>archive.Save(first,0),"stale revision cannot overwrite committed journal");
   Check(Convert.ToBase64String(savedBytes)==Convert.ToBase64String(File.ReadAllBytes(path)),"stale write retains exact bytes");
   var next=MasteryBookRules.SetGuideEnabled(first,false);
   Reject(()=>new MasteryFileArchive(path,codec,new BrokenWriter(false)).Save(next,1),"failure before commit is reported");
   Check(archive.Load().Book.Revision==1,"failed commit does not publish requested revision");
   new MasteryFileArchive(path,codec,new BrokenWriter(true)).Save(next,1);
   Check(archive.Load().Book.Revision==2&&!archive.Load().Book.GuideEnabled,"successful write followed by exception recovers exact receipt");
   var third=MasteryBookRules.SetGuideEnabled(next,true);archive.Save(third,2);Check(archive.Load().Book.Revision==3&&archive.Load().Book.GuideEnabled,"actual atomic replacement persists a second normal commit");
   File.WriteAllText(path,"broken-json");var corrupt=File.ReadAllBytes(path);Check(!archive.Load().Writable&&archive.Load().Error!=null,"corruption is visible and blocks writes");
   Reject(()=>archive.Save(first,0),"corrupt archive cannot be silently reset");Check(Convert.ToBase64String(corrupt)==Convert.ToBase64String(File.ReadAllBytes(path)),"corrupt bytes preserved");
   var future=MasteryBook.Empty;future.Schema=99;File.WriteAllText(path,codec.Encode(future));Check(!archive.Load().Writable,"future schema blocks writes");
   File.WriteAllBytes(path,new byte[]{0xff,0xfe,0xff});Check(!archive.Load().Writable,"invalid UTF8 cannot normalize into empty archive");
   File.WriteAllBytes(path,new byte[MasteryBookRules.MaximumBytes+1]);Check(!archive.Load().Writable,"oversized archive is rejected before decode");
   Check(File.ReadAllText(legacy)=="old profile byte sentinel","separate existing profile untouched");
   Check(Directory.GetFiles(directory,"*.tmp").Length==0,"owned temporary file cleanup");
   Console.WriteLine("RISING MASTERY ARCHIVE PASS checks="+checks);return 0;
  }
  finally{Directory.Delete(directory,true);}
 }
}
