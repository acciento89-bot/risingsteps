using System;
using System.IO;
using System.Text;

namespace Kamilunavo.RisingSteps.Core.Mastery
{
 public interface IMasteryCodec { string Encode(MasteryBook book); MasteryBook Decode(string text); }
 public interface IMasteryAtomicWriter { void Write(string path,byte[] bytes); }
 public sealed class MasteryArchiveState
 {
  public readonly MasteryBook Book;public readonly string Error;public bool Writable=>Error==null;
  public MasteryArchiveState(MasteryBook book,string error){Book=book;Error=error;}
 }
 public sealed class MasteryFileArchive
 {
  // Unity owns one archive; this also serializes overlapping adapters in-process.
  static readonly object Gate=new object();
  static readonly UTF8Encoding Utf8=new UTF8Encoding(false,true);
  readonly string _path;readonly IMasteryCodec _codec;readonly IMasteryAtomicWriter _writer;
  public MasteryFileArchive(string path,IMasteryCodec codec,IMasteryAtomicWriter writer=null)
  {if(string.IsNullOrEmpty(path)||codec==null)throw new ArgumentException("Journal path and codec required");_path=Path.GetFullPath(path);_codec=codec;_writer=writer??new MasteryAtomicWriter();}
  public MasteryArchiveState Load(){lock(Gate)return Read();}
  MasteryArchiveState Read()
  {
   try
   {
    if(!File.Exists(_path))return new MasteryArchiveState(MasteryBook.Empty,null);
    byte[] bytes=ReadBytes();var book=_codec.Decode(Utf8.GetString(bytes));MasteryBookRules.Validate(book);
    return new MasteryArchiveState(book,null);
   }
   catch(Exception error){return new MasteryArchiveState(null,"Journal unavailable: "+error.Message);}
  }
  byte[] ReadBytes()
  {
   using(var stream=new FileStream(_path,FileMode.Open,FileAccess.Read,FileShare.Read))
   {
    if(stream.Length<1||stream.Length>MasteryBookRules.MaximumBytes)throw new IOException("Journal size limit");
    var bytes=new byte[(int)stream.Length];int offset=0;
    while(offset<bytes.Length){int read=stream.Read(bytes,offset,bytes.Length-offset);if(read<=0)throw new IOException("Incomplete journal read");offset+=read;}
    if(stream.ReadByte()!=-1)throw new IOException("Journal changed while reading");return bytes;
   }
  }
  public void Save(MasteryBook book,int expectedRevision)
  {
   lock(Gate)
   {
    MasteryBookRules.Validate(book);var current=Read();
    if(!current.Writable)throw new IOException(current.Error);
    if(expectedRevision<0||expectedRevision==int.MaxValue||current.Book.Revision!=expectedRevision||book.Revision!=expectedRevision+1)throw new InvalidOperationException("Journal changed; reload before saving");
    string encoded=_codec.Encode(book);byte[] bytes=Utf8.GetBytes(encoded);
    if(bytes.Length<1||bytes.Length>MasteryBookRules.MaximumBytes)throw new IOException("Journal size limit");
    var roundTrip=_codec.Decode(encoded);MasteryBookRules.Validate(roundTrip);
    if(_codec.Encode(roundTrip)!=encoded)throw new IOException("Journal codec did not round-trip exactly");
    try{_writer.Write(_path,bytes);}
    catch{if(HasReceipt(bytes))return;throw;}
    if(!HasReceipt(bytes))throw new IOException("Journal commit could not be verified");
   }
  }
  bool HasReceipt(byte[] expected)
  {
   try{var actual=ReadBytes();if(actual.Length!=expected.Length)return false;for(int i=0;i<actual.Length;i++)if(actual[i]!=expected[i])return false;return Read().Writable;}
   catch{return false;}
  }
 }
 public sealed class MasteryAtomicWriter:IMasteryAtomicWriter
 {
  public void Write(string path,byte[] bytes)
  {
   string directory=Path.GetDirectoryName(path);Directory.CreateDirectory(directory);
   string temporary=Path.Combine(directory,"."+Path.GetFileName(path)+"."+Guid.NewGuid().ToString("N")+".tmp");
   try
   {
    using(var stream=new FileStream(temporary,FileMode.CreateNew,FileAccess.Write,FileShare.None)){stream.Write(bytes,0,bytes.Length);stream.Flush(true);}
    // Same-directory rename/replace. Never delete the previous journal first.
    if(File.Exists(path))File.Replace(temporary,path,null);else File.Move(temporary,path);
   }
   finally{if(File.Exists(temporary))File.Delete(temporary);}
  }
 }
}
