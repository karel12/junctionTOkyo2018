using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostprocessBuildPlayer
{

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        LinkLibraries(target, pathToBuiltProject);
    }

    public static void LinkLibraries(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Called OnPostprocessBuild");

        if (target == BuildTarget.iOS)
        {
            Debug.Log("Called OnPostprocessBuild for iOS");

            string projectFile = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            string contents = File.ReadAllText(projectFile);

            // manually add CoreBluetooth Framework by modifying the xcdodeproj/project.pbxproj file

            // build file section -  works
            const string firstPlaceholder = @"56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 56FD43950ED4745200FE3770 /* CFNetwork.framework */; };";
            contents = contents.Replace(firstPlaceholder,  @"56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 56FD43950ED4745200FE3770 /* CFNetwork.framework */; };
				6A6A90C01A2DCF97008B203E /* CoreBluetooth.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 6A6A90BF1A2DCF97008B203E /* CoreBluetooth.framework */; };");

            // file reference adding - works
            const string secondPlaceholder = @"56FD43950ED4745200FE3770 /* CFNetwork.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CFNetwork.framework; path = System/Library/Frameworks/CFNetwork.framework; sourceTree = SDKROOT; };";
            contents = contents.Replace(secondPlaceholder, @"56FD43950ED4745200FE3770 /* CFNetwork.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CFNetwork.framework; path = System/Library/Frameworks/CFNetwork.framework; sourceTree = SDKROOT; };	
					6A6A90BF1A2DCF97008B203E /* CoreBluetooth.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = CoreBluetooth.framework; path = System/Library/Frameworks/CoreBluetooth.framework; sourceTree = SDKROOT; };");

            // build phase adding - works
            const string thirdPlaceholder = @"/* Begin PBXFrameworksBuildPhase section */
		1D60588F0D05DD3D006BFB54 /* Frameworks */ = {
			isa = PBXFrameworksBuildPhase;
			buildActionMask = 2147483647;
			files = (
				1D60589F0D05DD5A006BFB54 /* Foundation.framework in Frameworks */,
				830B5C110E5ED4C100C7819F /* UIKit.framework in Frameworks */,
				83B256E20E62FEA000468741 /* OpenGLES.framework in Frameworks */,
				83B2570B0E62FF8A00468741 /* QuartzCore.framework in Frameworks */,
				83B2574C0E63022200468741 /* OpenAL.framework in Frameworks */,
				83B2574F0E63025400468741 /* libiconv.2.dylib in Frameworks */,
				D8A1C72B0E8063A1000160D3 /* libiPhone-lib.a in Frameworks */,
				8358D1B80ED1CC3700E3A684 /* AudioToolbox.framework in Frameworks */,
				56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */,
				5682F4B20F3B34FF007A219C /* MediaPlayer.framework in Frameworks */,
				5692F3DD0FA9D8E500EBA2F1 /* CoreLocation.framework in Frameworks */,
				56BCBA390FCF049A0030C3B2 /* SystemConfiguration.framework in Frameworks */,
				08B24F76137BFDFA00FBA309 /* iAd.framework in Frameworks */,
				7F36C11113C5C673007FBDD9 /* CoreMedia.framework in Frameworks */,
				7F36C11213C5C673007FBDD9 /* CoreVideo.framework in Frameworks */,
				7F36C11313C5C673007FBDD9 /* AVFoundation.framework in Frameworks */,
				56B7959B1442E0F20026B3DD /* CoreGraphics.framework in Frameworks */,
				56B7960F1442E1770026B3DD /* CoreMotion.framework in Frameworks */,
				56B7961A1442E1B80026B3DD /* GameKit.framework in Frameworks */,
			);
			runOnlyForDeploymentPostprocessing = 0;
		};";

            Debug.Log("Placeholder =" + thirdPlaceholder);
            if (!contents.Contains(thirdPlaceholder))
            {
                Debug.Log("Failed to locate " + thirdPlaceholder);
            }
            contents = contents.Replace(
                    thirdPlaceholder,

                    @"/* Begin PBXFrameworksBuildPhase section */
		1D60588F0D05DD3D006BFB54 /* Frameworks */ = {
			isa = PBXFrameworksBuildPhase;
			buildActionMask = 2147483647;
			files = (
				6A6A90C01A2DCF97008B203E /* CoreBluetooth.framework in Frameworks */,
				1D60589F0D05DD5A006BFB54 /* Foundation.framework in Frameworks */,
				830B5C110E5ED4C100C7819F /* UIKit.framework in Frameworks */,
				83B256E20E62FEA000468741 /* OpenGLES.framework in Frameworks */,
				83B2570B0E62FF8A00468741 /* QuartzCore.framework in Frameworks */,
				83B2574C0E63022200468741 /* OpenAL.framework in Frameworks */,
				83B2574F0E63025400468741 /* libiconv.2.dylib in Frameworks */,
				D8A1C72B0E8063A1000160D3 /* libiPhone-lib.a in Frameworks */,
				8358D1B80ED1CC3700E3A684 /* AudioToolbox.framework in Frameworks */,
				56FD43960ED4745200FE3770 /* CFNetwork.framework in Frameworks */,
				5682F4B20F3B34FF007A219C /* MediaPlayer.framework in Frameworks */,
				5692F3DD0FA9D8E500EBA2F1 /* CoreLocation.framework in Frameworks */,
				56BCBA390FCF049A0030C3B2 /* SystemConfiguration.framework in Frameworks */,
				08B24F76137BFDFA00FBA309 /* iAd.framework in Frameworks */,
				7F36C11113C5C673007FBDD9 /* CoreMedia.framework in Frameworks */,
				7F36C11213C5C673007FBDD9 /* CoreVideo.framework in Frameworks */,
				7F36C11313C5C673007FBDD9 /* AVFoundation.framework in Frameworks */,
				56B7959B1442E0F20026B3DD /* CoreGraphics.framework in Frameworks */,
				56B7960F1442E1770026B3DD /* CoreMotion.framework in Frameworks */,
				56B7961A1442E1B80026B3DD /* GameKit.framework in Frameworks */,
			);
			runOnlyForDeploymentPostprocessing = 0;
		};"
            );

            // framework build phase -works
            const string fourthPlaceholder = @"29B97323FDCFA39411CA2CEA /* Frameworks */ = {
			isa = PBXGroup;
			children = (
				8358D1B70ED1CC3700E3A684 /* AudioToolbox.framework */,
				7F36C11013C5C673007FBDD9 /* AVFoundation.framework */,
				56FD43950ED4745200FE3770 /* CFNetwork.framework */,
				56B7959A1442E0F20026B3DD /* CoreGraphics.framework */,
				5692F3DC0FA9D8E500EBA2F1 /* CoreLocation.framework */,
				7F36C10E13C5C673007FBDD9 /* CoreMedia.framework */,
				56B795C11442E1100026B3DD /* CoreMotion.framework */,
				7F36C10F13C5C673007FBDD9 /* CoreVideo.framework */,
				1D30AB110D05D00D00671497 /* Foundation.framework */,
				56B796191442E1B80026B3DD /* GameKit.framework */,
				08B24F75137BFDFA00FBA309 /* iAd.framework */,
				5682F4B10F3B34FF007A219C /* MediaPlayer.framework */,
				83B2574B0E63022200468741 /* OpenAL.framework */,
				83B256E10E62FEA000468741 /* OpenGLES.framework */,
				83B2570A0E62FF8A00468741 /* QuartzCore.framework */,
				56BCBA380FCF049A0030C3B2 /* SystemConfiguration.framework */,
				830B5C100E5ED4C100C7819F /* UIKit.framework */,
			);
			name = Frameworks;
			sourceTree = ""<group>"";
		};
";
            Debug.Log("Placeholder =" + fourthPlaceholder);
            if (!contents.Contains(fourthPlaceholder))
            {
                Debug.Log("Failed to locate " + fourthPlaceholder);
            }
            contents = contents.Replace(
                    fourthPlaceholder,

                    @"29B97323FDCFA39411CA2CEA /* Frameworks */ = {
			isa = PBXGroup;
			children = (
				6A6A90BF1A2DCF97008B203E /* CoreBluetooth.framework */,
				8358D1B70ED1CC3700E3A684 /* AudioToolbox.framework */,
				7F36C11013C5C673007FBDD9 /* AVFoundation.framework */,
				56FD43950ED4745200FE3770 /* CFNetwork.framework */,
				56B7959A1442E0F20026B3DD /* CoreGraphics.framework */,
				5692F3DC0FA9D8E500EBA2F1 /* CoreLocation.framework */,
				7F36C10E13C5C673007FBDD9 /* CoreMedia.framework */,
				56B795C11442E1100026B3DD /* CoreMotion.framework */,
				7F36C10F13C5C673007FBDD9 /* CoreVideo.framework */,
				1D30AB110D05D00D00671497 /* Foundation.framework */,
				56B796191442E1B80026B3DD /* GameKit.framework */,
				08B24F75137BFDFA00FBA309 /* iAd.framework */,
				5682F4B10F3B34FF007A219C /* MediaPlayer.framework */,
				83B2574B0E63022200468741 /* OpenAL.framework */,
				83B256E10E62FEA000468741 /* OpenGLES.framework */,
				83B2570A0E62FF8A00468741 /* QuartzCore.framework */,
				56BCBA380FCF049A0030C3B2 /* SystemConfiguration.framework */,
				830B5C100E5ED4C100C7819F /* UIKit.framework */,
			);
			name = Frameworks;
			sourceTree = ""<group>"";
		};
");

            File.WriteAllText(projectFile, contents);
        }
    }
}
