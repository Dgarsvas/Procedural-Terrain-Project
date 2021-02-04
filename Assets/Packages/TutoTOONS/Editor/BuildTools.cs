using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;
using TutoTOONS;
using System;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

#if UNITY_IOS
	using UnityEditor.iOS.Xcode;
#endif

public class BuildTools
{
	public const string ARG_BUNDLE_ID = "-bundle_id";
	public const string ARG_VERSION_NUMBER = "-version_number";
	public const string ARG_PRODUCT_NAME = "-product_name";
	public const string ARG_PLATFORM = "-platform";
	public const string ARG_FILENAME = "-buildFilename";
	public const string ARG_DEBUG = "debug";
	public const string ARG_CHARTBOOST = "chartboost";
	public const string ARG_SIGNING_TEAM_ID = "-signteam";
	public const string ARG_CERTIFICATE = "-certificate";
	public const string ARG_KEYSTORE = "-keystore";
	public const string ARG_TUTO_ADS = "adolf";
	public const string ARG_SUPERAWESOME = "superawesome";
	public const string ARG_KIDOZ = "kidoz";
	public const string ARG_SAFEDK = "safedk";
	public const string ARG_AD_MOB_MEDIATION = "admob";
	public const string ARG_IRONSOURCE_MEDIATION = "ironsource";
	public const string ARG_IRONSOURCE_MEDIATION_ADCOLONY = "iron_source_mediation_adcolony";
	public const string ARG_IRONSOURCE_MEDIATION_ADMOB = "iron_source_mediation_admob";
	public const string ARG_IRONSOURCE_MEDIATION_APPLOVIN = "iron_source_mediation_applovin";
	public const string ARG_IRONSOURCE_MEDIATION_UNITY_ADS = "iron_source_mediation_unity_ads";
	public const string ARG_IRONSOURCE_MEDIATION_VUNGLE = "iron_source_mediation_vungle";
	public const string ARG_IRONSOURCE_MEDIATION_INMOBI = "iron_source_mediation_inmobi";
	public const string ARG_GOOGLE_ANALYTICS = "google_analytics";
	public const string ARG_UNITY_ANALYTICS = "unity";
	public const string ARG_FIREBASE_ANALYTICS = "firebase_analytics";
	public const string ARG_FIREBASE_AUTH = "firebase_auth";
	public const string ARG_FIREBASE_REALTIME_DATABASE = "firebase_realtimedatabase";
	public const string ARG_SINGULAR = "singular";
	public const string ARG_APP_BUNDLE = "android_app_bundle";
	public const string ARG_SOOMLA = "soomla";
	public const string ARG_FREETIME = "freetime";
	public const string ARG_SUBSCRIPTION = "subscription";
	public const string ARG_XCODE_PATH = "xcode_path";
	public const string ARG_IS_AIR = "is_air";
	public const string ARG_EXECUTION_ORDER = "-exec_order";

	static string path_to_billing_json = "Assets/Resources/BillingMode.json";
	static string keystore_path = "../_generator/";

	public int callbackOrder { get { return 0; } }

	public static void ChangeAppStore(bool amazon)
	{
		string _currentPath = Directory.GetCurrentDirectory();
		char[] _splitChars = new[] { '\\', '/' };
		string[] _pathArray = _currentPath.Split(_splitChars, 100);
		string[] _billingArray = path_to_billing_json.Split(_splitChars, 100);
		List<string> _combined = new List<string>(_pathArray);
		for (int i = 0; i < _billingArray.Length; ++i)
		{
			if (!_combined.Contains(_billingArray[i]))
			{
				_combined.Add(_billingArray[i]);
			}
		}
		string _path = string.Join(Path.DirectorySeparatorChar.ToString(), _combined.ToArray());
		if (!File.Exists(_path)) return;
		string _text = File.ReadAllText(_path);
		if (amazon)
		{
			_text = _text.Replace("GooglePlay", "AmazonAppStore");
		}
		else
		{
			_text = _text.Replace("AmazonAppStore", "GooglePlay");
		}
		File.WriteAllText(_path, _text);
	}

	static MonoScript GetMonoScript()
	{
		string _asset_path = "Assets/Packages/TutoTOONS/TutoTOONS.prefab";
		GameObject _prefab_tuto = PrefabUtility.LoadPrefabContents(_asset_path);
		return MonoScript.FromMonoBehaviour(_prefab_tuto.GetComponent<TutoTOONS.TutoTOONS>());
	}

	public static void GetExecutionOrder()
	{
		File.WriteAllText(Application.dataPath + "/../exec_order.txt", MonoImporter.GetExecutionOrder(GetMonoScript()).ToString());
	}

	public static void Build()
	{
		BuildArguments _build_arguments = GetBuildArguments();

		if (_build_arguments.execution_order != "0")
		{
			MonoImporter.SetExecutionOrder(GetMonoScript(), int.Parse(_build_arguments.execution_order));
		}

		if (Enum.IsDefined(typeof(AndroidSdkVersions), 30))
		{
			PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)30;
		}

		// Get scene list
		bool _subscription_scenes_exist = false;
		int _len = EditorBuildSettings.scenes.Length;
		List<EditorBuildSettingsScene> _scenes = new List<EditorBuildSettingsScene>();
		for (int i = 0; i < _len; i++)
		{
			if (!EditorBuildSettings.scenes[i].enabled) continue;
			if (!_build_arguments.subscription)
			{
				if (EditorBuildSettings.scenes[i].path.Contains("TutoToonsSubscription")) continue;
			}
			else
			{
				if (EditorBuildSettings.scenes[i].path.Contains("TutoToonsSubscription"))
				{
					_subscription_scenes_exist = true;
				}
			}
			_scenes.Add(EditorBuildSettings.scenes[i]);
		}
		if (_build_arguments.subscription && !_subscription_scenes_exist)
		{
			_scenes.Add(new EditorBuildSettingsScene("Assets/Packages/TutoToonsSubscription/Subscription Manager/Scenes/Device_Limit.unity", true));
			_scenes.Add(new EditorBuildSettingsScene("Assets/Packages/TutoToonsSubscription/Subscription Manager/Scenes/Parental_Gate_Screen.unity", true));
			_scenes.Add(new EditorBuildSettingsScene("Assets/Packages/TutoToonsSubscription/Subscription Manager/Scenes/Signin_Screen.unity", true));
			_scenes.Add(new EditorBuildSettingsScene("Assets/Packages/TutoToonsSubscription/Subscription Manager/Scenes/Subscription_Screen.unity", true));
		}
		EditorBuildSettingsScene[] _new_scenes = _scenes.ToArray();
		EditorBuildSettings.scenes = _new_scenes;

		// Build
		BuildTarget _build_target = BuildTarget.Android;


		if (_build_arguments.platform == "android" || _build_arguments.platform == "amazon")
		{
			_build_target = BuildTarget.Android;

			PlayerSettings.productName = _build_arguments.product_name;
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _build_arguments.bundle_id);
			PlayerSettings.bundleVersion = _build_arguments.version_number;
			PlayerSettings.Android.bundleVersionCode = GetVersionCode(PlayerSettings.bundleVersion);

			KeystoreData _keystore = new KeystoreData(_build_arguments.keystore);

			PlayerSettings.Android.useCustomKeystore = true;
			PlayerSettings.Android.keystoreName = System.IO.Path.GetFullPath(keystore_path + _keystore.keystoreName).Replace("\\", "/");
			PlayerSettings.Android.keystorePass = _keystore.keystorePassword;
			PlayerSettings.Android.keyaliasName = _keystore.keyAliasName;
			PlayerSettings.Android.keyaliasPass = _keystore.keyAliasPassword;
			SetAndroidAppBundle(_build_arguments.android_app_bundle);
			ChangeAppStore(_build_arguments.platform == "amazon");
		}
		else if (_build_arguments.platform == "ios")
		{
			_build_target = BuildTarget.iOS;

			PlayerSettings.productName = _build_arguments.product_name;
			PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, _build_arguments.bundle_id);
			PlayerSettings.bundleVersion = _build_arguments.version_number;
			PlayerSettings.iOS.buildNumber = GetVersionCode(PlayerSettings.bundleVersion).ToString();

			PlayerSettings.iOS.appleDeveloperTeamID = _build_arguments.sign_team;
			PlayerSettings.iOS.appleEnableAutomaticSigning = true;
			PlayerSettings.iOS.targetOSVersionString = "11.2";
		}

		if (_build_arguments.is_air)
		{
			if (PlayerSettings.applicationIdentifier.Substring(0, 4) != "air.")
			{
				PlayerSettings.applicationIdentifier = "air." + PlayerSettings.applicationIdentifier;
			}
		}
		else
		{
			if (PlayerSettings.applicationIdentifier.Substring(0, 4) == "air.")
			{
				PlayerSettings.applicationIdentifier = PlayerSettings.applicationIdentifier.Substring(4);
			}
		}

		BuildOptions _options = BuildOptions.None;
		UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(_new_scenes, _build_arguments.filename, _build_target, _options);
	}

	private static int GetVersionCode(string _version_number)
	{
		string _version_code = "0";
		string[] version_split = _version_number.Split('.');

		if (int.Parse(version_split[0]) > 1000000)
		{
			_version_code = version_split[0];
		}
		else
		{
			if (version_split.Length == 3)
			{
				_version_code = version_split[0] + version_split[1].PadLeft(3, '0') + version_split[2].PadLeft(3, '0');
			}
		}

		return int.Parse(_version_code);
	}

	static string GetAppGroup(string _certificate)
	{
		if (_certificate == "cert_apix")
			return "group.com.sugarfree.shared";
		if (_certificate == "cert_support")
			return "group.com.tutotoons.shared";
		if (_certificate == "cert_zaidimustudija")
			return "group.com.cutetinygames.shared";
		
		return "";
	}

	//PBX project needs to be updated after installing pods
	public static void UpdatePBXProject()
	{
		#if UNITY_IOS
			BuildArguments _build_arguments = GetBuildArguments();

			PBXProject _project = new PBXProject();
			string _project_path = PBXProject.GetPBXProjectPath(_build_arguments.xcode_path);
			_project.ReadFromFile(_project_path);

			string _target_guid = _project.GetUnityMainTargetGuid();
			string _framework_guid = _project.GetUnityFrameworkTargetGuid();
			string _project_guid = _project.ProjectGuid();

			_project.AddBuildProperty(_target_guid, "OTHER_LDFLAGS", "$(fakeparameter)");

			_project.AddBuildProperty(_framework_guid, "OTHER_LDFLAGS", "$(inherited)");
			_project.AddBuildProperty(_framework_guid, "OTHER_LDFLAGS", "-ObjC");

			_project.SetBuildProperty(_target_guid, "ENABLE_BITCODE", "false");
			_project.SetBuildProperty(_project_guid, "ENABLE_BITCODE", "false");

			_project.AddCapability(_target_guid, PBXCapabilityType.AppGroups);

			File.WriteAllText(_project_path, _project.WriteToString());

			string _app_group = GetAppGroup(_build_arguments.certificate);

			ProjectCapabilityManager _project_capabilities = new ProjectCapabilityManager(_project_path, "Unity-iPhone/mmk.entitlements", "Unity-iPhone", _target_guid);
			string[] _app_groups = { _app_group };
			_project_capabilities.AddAppGroups(_app_groups);
			_project_capabilities.WriteToFile();
			string _new_content = File.ReadAllText(Application.dataPath + "/Packages/TutoTOONS/Editor/mmk.entitlements");
			_new_content = _new_content.Replace("CHANGETHIS", _app_group);
			File.WriteAllText(_build_arguments.xcode_path + "/Unity-iPhone/mmk.entitlements", _new_content);
		#endif
	}

	public static void AddComponents()
	{
		string _asset_path = "Assets/Packages/TutoTOONS/TutoTOONS.prefab";

		GameObject _prefab_tuto = PrefabUtility.LoadPrefabContents(_asset_path);

		BuildArguments _build_arguments = GetBuildArguments();

		if (_build_arguments.kidoz)
		{
			AdNetworkKeys _kidoz_keys = TutoToonsEditor.GetAdNetworkKeys(TutoToonsEditor.AD_NETWORK_KIDOZ);

			System.Type _class_type = System.Type.GetType("KidozSDK.Kidoz,Assembly-CSharp");
			Component _obj = _prefab_tuto.AddComponent(_class_type);
			FieldInfo _field_publisher_id = _class_type.GetField("PublisherID");
			FieldInfo _field_security_token = _class_type.GetField("SecurityToken");
			_field_publisher_id.SetValue(_obj, _kidoz_keys.key1);
			_field_security_token.SetValue(_obj, _kidoz_keys.key2);
		}
		if (_build_arguments.ironsource_mediation)
		{
			System.Type _class_type = System.Type.GetType("IronSourceEvents,Assembly-CSharp");
			GameObject _go = new GameObject("IronSourceEvents");
			_go.transform.SetParent(_prefab_tuto.transform);
			_go.AddComponent(_class_type);
		}
		if (_build_arguments.singular)
		{
			System.Type _class_type = System.Type.GetType("TutoTOONS.SingularWrapper,Assembly-CSharp");
			_prefab_tuto.AddComponent(_class_type);

			System.Type _class_type2 = System.Type.GetType("SingularSDK,Assembly-CSharp");
			_prefab_tuto.AddComponent(_class_type2);
		}
		if (_build_arguments.adolf)
		{
			System.Type _class_type = System.Type.GetType("TutoAds,Assembly-CSharp");
			_prefab_tuto.AddComponent(_class_type);
		}

		PrefabUtility.SaveAsPrefabAsset(_prefab_tuto, _asset_path);
		PrefabUtility.UnloadPrefabContents(_prefab_tuto);
	}

	public static BuildArguments GetBuildArguments()
	{
		BuildArguments _build_arguments = new BuildArguments();
		string[] _args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < _args.Length; i++)
		{
			if (_args[i] == ARG_BUNDLE_ID)
			{
				_build_arguments.bundle_id = _args[i + 1];
			}
			if (_args[i] == ARG_VERSION_NUMBER)
			{
				_build_arguments.version_number = _args[i + 1];
			}
			if (_args[i] == ARG_PRODUCT_NAME)
			{
				_build_arguments.product_name = _args[i + 1];
			}
			if (_args[i] == ARG_SIGNING_TEAM_ID)
			{
				_build_arguments.sign_team = _args[i + 1];
			}
			if (_args[i] == ARG_CERTIFICATE)
			{
				_build_arguments.certificate = _args[i + 1];
			}
			if (_args[i] == ARG_KEYSTORE)
			{
				_build_arguments.keystore = _args[i + 1];
			}
			if (_args[i] == ARG_PLATFORM)
			{
				_build_arguments.platform = _args[i + 1];
			}
			if (_args[i] == ARG_FILENAME)
			{
				_build_arguments.filename = _args[i + 1];
			}
			if (_args[i] == ARG_DEBUG)
			{
				_build_arguments.debug = true;
			}
			if (_args[i] == ARG_CHARTBOOST)
			{
				_build_arguments.chartboost = true;
			}
			if (_args[i] == ARG_TUTO_ADS)
			{
				_build_arguments.adolf = true;
			}
			if (_args[i] == ARG_SUPERAWESOME)
			{
				_build_arguments.super_awesome = true;
			}
			if (_args[i] == ARG_KIDOZ)
			{
				_build_arguments.kidoz = true;
			}
			if (_args[i] == ARG_AD_MOB_MEDIATION)
			{
				_build_arguments.admob_mediation = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION)
			{
				_build_arguments.ironsource_mediation = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_ADCOLONY)
			{
				_build_arguments.ironsource_mediation_adcolony = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_ADMOB)
			{
				_build_arguments.ironsource_mediation_admob = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_APPLOVIN)
			{
				_build_arguments.ironsource_mediation_applovin = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_UNITY_ADS)
			{
				_build_arguments.ironsource_mediation_unity_ads = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_VUNGLE)
			{
				_build_arguments.ironsource_mediation_vungle = true;
			}
			if (_args[i] == ARG_IRONSOURCE_MEDIATION_INMOBI)
			{
				_build_arguments.ironsource_mediation_inmobi = true;
			}
			if (_args[i] == ARG_GOOGLE_ANALYTICS)
			{
				_build_arguments.google_analytics = true;
			}
			if (_args[i] == ARG_UNITY_ANALYTICS)
			{
				_build_arguments.unity_analytics = true;
			}
			if (_args[i] == ARG_FIREBASE_ANALYTICS)
			{
				_build_arguments.firebase_analytics = true;
			}
			if (_args[i] == ARG_SINGULAR)
			{
				_build_arguments.singular = true;
			}
			if (_args[i] == ARG_APP_BUNDLE)
			{
				_build_arguments.android_app_bundle = true;
			}
			if (_args[i] == ARG_XCODE_PATH)
			{
				_build_arguments.xcode_path = _args[i + 1];
			}
			if (_args[i] == ARG_IS_AIR)
			{
				_build_arguments.is_air = true;
			}
			if (_args[i] == ARG_SOOMLA)
			{
				_build_arguments.soomla = true;
			}
			if (_args[i] == ARG_SAFEDK)
			{
				_build_arguments.safedk = true;
			}
			if (_args[i] == ARG_FREETIME)
			{
				_build_arguments.freetime = true;
			}
			if (_args[i] == ARG_SUBSCRIPTION)
			{
				_build_arguments.subscription = true;
			}
			if (_args[i] == ARG_EXECUTION_ORDER)
			{
				_build_arguments.execution_order = _args[i + 1];
			}
		}
		return _build_arguments;
	}

	private static void SetAndroidAppBundle(bool _enabled)
	{
		if (_enabled)
		{
			EditorUserBuildSettings.buildAppBundle = true;
			EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		}
		else
		{
			EditorUserBuildSettings.buildAppBundle = false;
			EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;
		}
	}
}

public class KeystoreData
{
	public string keystoreName;
	public string keystorePassword;
	public string keyAliasName;
	public string keyAliasPassword;

	public KeystoreData(string _keystore_data)
	{
		string[] _data = _keystore_data.Split(';');

		keystoreName = _data[0];
		keystorePassword = _data[1];
		keyAliasName = _data[2];
		keyAliasPassword = _data[3];
	}
}

public class BuildArguments
{
	public string bundle_id = "";
	public string version_number = "";
	public string product_name = "";
	public string platform = "";
	public string filename = "";
	public string sign_team = "";
	public string xcode_path = "";
	public string certificate = "";
	public string keystore = "";
	public string execution_order = "0";
	public bool is_air;
	public bool debug;
	public bool chartboost;
	public bool adolf;
	public bool safedk;
	public bool super_awesome;
	public bool kidoz;
	public bool admob_mediation;
	public bool ironsource_mediation;
	public bool ironsource_mediation_adcolony;
	public bool ironsource_mediation_admob;
	public bool ironsource_mediation_applovin;
	public bool ironsource_mediation_unity_ads;
	public bool ironsource_mediation_vungle;
	public bool ironsource_mediation_inmobi;
	public bool google_analytics;
	public bool unity_analytics;
	public bool firebase_analytics;
	public bool singular;
	public bool android_app_bundle;
	public bool soomla;
	public bool freetime;
	public bool subscription;
}