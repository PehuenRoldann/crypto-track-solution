using System;
using IO = System.IO;
using System.Linq;

namespace CryptoTrackApp.src.utils {

    public static class PathFinder {

        private static string _basePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string GetPath(string[] relativePathArr) {

            if (relativePathArr == null) {
                return "";
            }

            return IO.Path.Combine(relativePathArr.Prepend(_basePath).ToArray());
        }
    }
}