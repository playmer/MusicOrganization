#include <filesystem>
#include <iostream>
#include <fstream>
#include <set>

int main(int aArgumentCount, char **aArguments)
{
  namespace fs = std::experimental::filesystem;

  if (5 < aArgumentCount)
  {
    std::cout << "Must have two directories, a source directory, "
                 "and a destination directory.\n";
    return 1;
  }

  std::set<fs::path> changeToMp3{".mp3~2", ".MP3", ".Mp3"};
  std::set<fs::path> move{ ".mp3", ".flac" };
  fs::path m4a{ ".m4a" };
  fs::path mp3{ ".mp3" };

  std::vector<fs::path> files;

  fs::path logFile{ aArguments[1] };

  fs::path source{ aArguments[2] };
  fs::path destination{ aArguments[3] };
  fs::path tempDirectory{ aArguments[4] };

  std::ofstream log;
  log.open(logFile, std::ios::out | std::ios::trunc | std::ios::binary);

  log << 0xEF;
  log << 0xBB;
  log << 0xBF;

  {
    fs::path path;
    fs::path toPath;
    fs::path extension;
    std::string u8String;
    for (auto& it : fs::recursive_directory_iterator(source))
    {
      path = it;
      toPath = path;

      if (fs::is_directory(path))
      {
        continue;
      }

      extension = path.extension();

      if (extension == m4a)
      {

      }

      if (changeToMp3.find(extension) != changeToMp3.end())
      {
        toPath.replace_extension(mp3);
      }

      u8String = path.generic_u8string();

      log << u8String << '\n';
    }
  }

  log.close();
}