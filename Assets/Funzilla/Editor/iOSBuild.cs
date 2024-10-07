
#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;

namespace Funzilla
{
	public class iOSBuild
	{
		private static readonly string[] Localizations = { "en", "zh-Hans", "zh-Hant", "fr", "de", "ja", "ko", "es" };

		[PostProcessBuild]
		public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
		{
			if (buildTarget != BuildTarget.iOS)
			{
				return;
			}

			// Get plist
			var plistPath = buildPath + "/Info.plist";
			var plist = new PlistDocument();
			plist.ReadFromString(File.ReadAllText(plistPath));

			// Get root
			var rootDict = plist.root;
			rootDict.SetString("NSUserTrackingUsageDescription", "NSUserTrackingUsageDescription");
			rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://postbacks-is.com");
			rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);
			rootDict.CreateDict("NSAppTransportSecurity").SetBoolean("NSAllowsArbitraryLoads", true);

			var adNetworkItems = rootDict.CreateArray("SKAdNetworkItems");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "su67r6k2v3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "f7s53z58qe.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "2u9pt9hc89.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "hs6bdukanm.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "8s468mfl3y.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "c6k4g5qg8m.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "v72qych5uu.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "44jx6755aq.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "prcb7njmu6.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "m8dbw4sv7c.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "3rd42ekr43.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4fzdc2evr5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "t38b2kh725.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "f38h382jlk.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "424m5254lk.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ppxm28t8ap.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "av6w8kgt66.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "cp8zw746q7.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4468km3ulz.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "e5fvkxwrpn.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4pfyvq9l8r.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "yclnxrl5pm.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "tl55sbb4fm.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "mlmmfzh3r3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "klf5c3l5u5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9t245vhmpl.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9rd848q2bz.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "7ug5zh24hu.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "7rz58n8ntl.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ejvt5qm6ak.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "5lm9lj6jb7.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "mtkv5xtk9e.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "6g9af3uyq4.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "uw77j35x4d.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "u679fj5vs4.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "rx5hdcabgc.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "g28c52eehv.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "cg4yq2srnc.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9nlqeag3gk.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "275upjj5gd.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "wg4vff78zm.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "qqp299437r.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "2fnua5tdw4.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "3qcr597p9d.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "3qy4746246.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "3sh42y64q3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "5a6flpkh64.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "cstr6suwn9.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "kbd757ywx3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "n6fk4nfna4.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "p78axxw29g.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "s39g8k73mm.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "wzmmz9fp6w.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ydx93a7ass.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "zq492l623r.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "24t9a8vw3c.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "32z4fx6l9h.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "523jb4fst2.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "54nzkqm89y.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "578prtvx9j.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "5l3tpt7t6e.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "6xzpu9s2p8.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "79pbpufp6p.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9b89h5y424.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "cj5566h2ga.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "feyaarzu9v.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ggvn48r87g.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "glqzh8vgby.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "gta9lk7p23.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "k674qkevps.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ludvb6z3bs.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "n9x2a789qt.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "pwa73g5rt2.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "xy9t38ct57.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "zmvfpc5aq8.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "22mmun2rn5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "294l99pt4k.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "44n7hlldy6.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4dzt52r2t5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "4w7y6s5ca2.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "5tjdwbrq8w.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "6964rsfnh4.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "6p4ks3rnbw.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "737z793b9f.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "74b6s63p6l.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "84993kbrcf.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "97r2b46745.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "a7xqa6mtl2.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "b9bk5wbcq9.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "bxvub5ada5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "dzg6xy7pwj.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "f73kdq92p3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "g2y4y55b64.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "hdw39hrw9y.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "kbmxgpxpgc.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "lr83yxwka7.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "mls7yz5dvl.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "mp6xlyr22a.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "pwdxu55a5a.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "r45fhb6rf7.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "rvh3l7un93.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "s69wq72ugq.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "w9q455wk68.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "x44k69ngh6.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "x8uqf25wch.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "y45688jllp.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "v9wttpbfk9.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "n38lu8286q.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "252b5q8x7y.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9g2aggbj52.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "wzmmZ9fp6w.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "238da6jt44.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "488r3q3dtq.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "52fl2v3hgk.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "9yg77x724h.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "ecpz2srf59.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "gvmwg8q7h5.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "n66cz3y3bx.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "nzq8sh4pbs.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "pu4na253f3.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "v79kvwwj4g.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "yrqqpx2mcb.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "z4gj7hsk7h.skadnetwork");
			adNetworkItems.AddDict().SetString("SKAdNetworkIdentifier", "7953jerfzd.skadnetwork");

			// Write to file
			File.WriteAllText(plistPath, plist.WriteToString());

			// Configure project
			var pbxPath = PBXProject.GetPBXProjectPath(buildPath);
			var project = new PBXProject();
			var file = File.ReadAllText(pbxPath);
			project.ReadFromString(file);
			var targetGuid = project.GetUnityMainTargetGuid();
			project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");

			var frameworkGuid = project.GetUnityFrameworkTargetGuid();
			project.SetBuildProperty(frameworkGuid, "ENABLE_BITCODE", "NO");

			foreach (var localization in Localizations)
			{
				var guid = project.AddFolderReference(
					$"{Application.dataPath}/../Build/iOS/Localizations/{localization}.lproj",
					$"{localization}.lproj",
					PBXSourceTree.Source);
				project.AddFileToBuild(targetGuid, guid);
				project.AddFileToBuild(frameworkGuid, guid);
			}

			File.WriteAllText(pbxPath, project.WriteToString());
			
			// Add PN capability
			var idArray = Application.identifier.Split('.');
			var entitlementsPath = $"Unity-iPhone/{idArray[idArray.Length - 1]}.entitlements";
			var capManager = new ProjectCapabilityManager(pbxPath, entitlementsPath, null, targetGuid);
			capManager.AddPushNotifications(true);
			capManager.WriteToFile();
		}
	}
}
#endif