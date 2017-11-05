using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace iTunesConverter
{
  class Track
  {
    public Track()
    {
      mId = 0;
      mName = "";
      mKind = "";
      mFileType = "";
      mLocation = "";

    }

    public System.UInt64 mId;
    public string mName;
    public string mKind;
    public string mFileType;
    public string mLocation;
  }

  class Playlist
  {
    public Playlist()
    {
      mName =  "";
      mTracks = new List<Track>();
    }

    public string mName;
    public List<Track> mTracks;
  }



  class iTunesHandler
  {
    private Dictionary<System.UInt64, Track> mTracks;
    private Dictionary<string, Playlist> mPlaylists;
    private string mMusicLibrary;

    public iTunesHandler(string aFile)
    {
      mTracks = new Dictionary<System.UInt64, Track>();
      mPlaylists = new Dictionary<string, Playlist>();

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
        var track = new Track();

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


    private void HandleTracks(XmlNode aNode)
    {
      if (aNode.HasChildNodes)
      {
        var nodes = aNode.ChildNodes;
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


    private void HandlePlaylist(XmlNode aNode)
    {
      if (aNode.HasChildNodes)
      {
        var playlist = new Playlist();

        playlist.mName = "";

        var nodes = aNode.ChildNodes;
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
            case "Name":
            {
              playlist.mName = value.InnerText;
              break;
            }
            case "Visible":
            {
              if (value.Name == "false")
              {
                return;
              }
              break;
            }
            case "Playlist Items":
            {
              var playlistItems = value.ChildNodes;
              var playlistSize = value.ChildNodes.Count;
              for (int j = 0; j < playlistSize; ++j)
              {
                var itemId = playlistItems[j].ChildNodes[1];
                System.UInt64 trackId;

                if (true == System.UInt64.TryParse(itemId.InnerText, out trackId))
                {
                  playlist.mTracks.Add(mTracks[trackId]);
                }
                else
                {

                }
              }

              break;
            }
          }
        }

        while (mPlaylists.ContainsKey(playlist.mName))
        {
          playlist.mName += "_DuplicatePlaylistName";
        }

        mPlaylists.Add(playlist.mName, playlist);
      }
    }

    private void HandlePlaylists(XmlNode aNode)
    {
      if (aNode.HasChildNodes)
      {
        var nodes = aNode.ChildNodes;
        var size = nodes.Count;
        //var halfSize = size / 2;


        for (int i = 0; i < size; ++i)
        {
          var value = nodes[i];
          HandlePlaylist(value);
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
          HandlePlaylists(aValue);
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
