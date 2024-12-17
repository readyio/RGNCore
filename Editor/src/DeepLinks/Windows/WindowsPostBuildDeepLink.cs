﻿using System.IO;
using System.Xml;
using RGN.DeepLink;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RGN.MyEditor
{
    public class WindowsPostBuildDeepLink
    {
        [PostProcessBuild]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.StandaloneWindows &&
                buildTarget != BuildTarget.StandaloneWindows64)
            {
                return;
            }

            using (var writer = XmlWriter.Create(Path.Combine(Path.GetDirectoryName(pathToBuiltProject)!, "deeplink.xml")))
            {
                writer.WriteStartElement("app");
                writer.WriteElementString("redirect_scheme", RGNDeepLinkHttpUtility.GetDeepLinkRedirectSchemeForBuild());
                writer.WriteElementString("pipe_name", Application.productName);
                writer.WriteElementString("executable_name", Application.productName + ".exe");
                writer.WriteEndElement();
                writer.Flush();
            }

            AssetDatabase.CopyAsset("Packages/io.getready.rgn.core/Plugins/Windows/RGNDeepLinkReflector.exe",
                Path.Combine(Path.GetDirectoryName(pathToBuiltProject)!, $"{Application.productName}DL.exe"));
        }
    }
}
