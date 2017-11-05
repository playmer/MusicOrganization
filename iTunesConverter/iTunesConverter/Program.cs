using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace iTunesConverter
{
  public class Ref<T> where T : struct
  {
    public T Value { get; set; }
  }

  struct Track
  {
    public System.UInt64 mId;
    public string mName;
    public string mKind;
    public string mFileType;
    public string mLocation;
  }

  class Playlist
  {
    string mPlaylistName;

    List<Ref<Track>> mTracks;
  }



  class iTunesHandler
  {
    private Dictionary<System.UInt64, Track> mTracks;
    private string mMusicLibrary;

    public iTunesHandler(string aFile)
    {
      mTracks = new Dictionary<System.UInt64, Track>();

      XmlDocument doc = new XmlDocument();
      doc.Load(aFile);

      XmlElement plist = doc.DocumentElement;
      XmlNodeList nodes = plist.GetElementsByTagName("dict"); // You can also use XPath here

      HandleDictionary(nodes.Item(0));
    }


    private void HandleTrack(XmlNode aKey, XmlNode aValue)
    {
      if (aValue.HasChildNodes)
      {
        Track track;

        track.mId = 0;
        track.mName = "";
        track.mKind = "";
        track.mFileType = "";
        track.mLocation = "";

        var nodes = aValue.ChildNodes;
        var size = nodes.Count;
        var halfSize = size / 2;


        for (int i = 0; i < halfSize; ++i)
        {
          var keyIndex = i * 2;
          var valueIndex = keyIndex + 1;

          var key = nodes[i * 2];
          var value = nodes[i * 2 + 1];

          switch (key.InnerText)
          {
            case "Track ID":
            {
              System.UInt64.TryParse(value.InnerText, out track.mId);
              break;
            }
            case "Kind":
            {
              track.mKind = value.InnerText;
              break;
            }
            case "Location":
            {
              track.mLocation = Uri.UnescapeDataString(value.InnerText);
              track.mFileType = Path.GetExtension(track.mLocation);
              break;
            }
            case "Name":
            {
              track.mName = value.InnerText;
              break;
            }
          }
        }

        mTracks.Add(track.mId, track);
      }

    }


    private void HandleTracks(XmlNode node)
    {
      if (node.HasChildNodes)
      {
        var nodes = node.ChildNodes;
        var size = nodes.Count;
        var halfSize = size / 2;


        for (int i = 0; i < halfSize; ++i)
        {
          var keyIndex = i * 2;
          var valueIndex = keyIndex + 1;

          var key = nodes[i * 2];
          var value = nodes[i * 2 + 1];

          HandleTrack(key, value);
        }
      }
    }

    private void HandlePlaylist(XmlNode node)
    {
      if (node.HasChildNodes)
      {
        var nodes = node.ChildNodes;
        var size = nodes.Count;
        //var halfSize = size / 2;


        for (int i = 0; i < size; ++i)
        {
          var value = nodes[i];
        }
      }
    }

    void HandleTopLevel(XmlNode aKey, XmlNode aValue)
    {
      switch (aKey.InnerText)
      {
        case "Tracks":
        {
          HandleTracks(aValue);
          break;
        }
        case "Playlists":
        {
          HandlePlaylist(aValue);
          break;
        }
        case "Music Folder":
        {
          mMusicLibrary = aValue.InnerText;
          break;
        }
      }
    }



    private void HandleDictionary(XmlNode node)
    {
      if (node.HasChildNodes)
      {
        var nodes = node.ChildNodes;
        var size = nodes.Count;
        var halfSize = size / 2;


        for (int i = 0; i < halfSize; ++i)
        {
          var keyIndex = i * 2;
          var valueIndex = keyIndex + 1;

          var key = nodes[i * 2];
          var value = nodes[i * 2 + 1];

          HandleTopLevel(key, value);
        }
      }
    }
  }



  class Program
  {

    static void Main(string[] args)
    {
      var itunes = new iTunesHandler("D:/Music/iTunes/iTunes Library.xml");
    }
  }
}
