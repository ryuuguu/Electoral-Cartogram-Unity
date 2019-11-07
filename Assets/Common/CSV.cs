using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CSV {
    public static string[][] SplitPath(string path, char[] separator = null) {
        if (separator == null) separator =new[] {','};
        return File.ReadAllLines(path).Select(a => a.Split(separator)).ToArray();
    }

    public static string[][] SplitString(string aString, string[] separator = null) {
        if (separator == null) separator =new string[] {','.ToString()};
        return aString.Split(
            new[] { "\r\n", "\r", "\n" },StringSplitOptions.None
        ).Select(a => a.Split(separator,StringSplitOptions.None)).ToArray();
    }
    
    
}
