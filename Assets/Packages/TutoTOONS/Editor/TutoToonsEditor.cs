using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Xml;
using System.Text.RegularExpressions;
using UnityEditor.Android;
using UnityEngine.Networking;
using System;

public class TutoToonsEditor : AssetPostprocessor, IPreprocessBuildWithReport, IPostGenerateGradleAndroidProject
{
    public const string AD_NETWORK_KIDOZ = "kidoz";
    public const string AD_NETWORK_AD_MOB = "admob";
        
    public static string path_to_android_manifest = Application.dataPath + "/Plugins/Android/AndroidManifest.xml";
    public static XmlDocument android_manifest_document;
    private static bool is_build_cleaned = false;
    public int callbackOrder { get { return 0; } }

    [MenuItem("Packages/Export Package/Export TutoToons Base Package")]
    static void ExportTutoToonsBaseUnityPackage()
    {
        string _exported_package_filename = "../../TutoToonsBase.unitypackage";
        List<string> _assets_to_export = new List<string>();
        AddFilesToList(Application.dataPath, _assets_to_export, true);
        AssetDatabase.ExportPackage(_assets_to_export.ToArray(), _exported_package_filename, ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Interactive);
    }

    private static void AddFilesToList(string _path, List<string> _list, bool is_root)
    {
        foreach (string _dir_path in Directory.GetDirectories(_path))
        {
            if (is_root)
            {
                if (!(_dir_path.EndsWith("Editor") || _dir_path.EndsWith("Packages") || _dir_path.EndsWith("Plugins") || _dir_path.EndsWith("TutoTOONS")))
                {
                    continue;
                }
            }
            AddFilesToList(_dir_path, _list, false);
        }

        foreach (string _file_path in Directory.GetFiles(_path))
        {
            if (_file_path.EndsWith(".meta"))
            {
                continue;
            }
            int _pos = Application.dataPath.Length;
            _list.Add("Assets" + _file_path.Substring(_pos));
        }
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        RemovePrefab("IronSourceEvents");
        RemovePrefab("TutoAds");
        RemovePrefab("Kidoz");

#if GEN_SUBSCRIPTION
        TutoTOONS.SubscriptionEditor.GenerateIconList();
#endif

#if UNITY_IOS
        RunPostProcessTasksiOS();
#elif UNITY_ANDROID
        CleanBuildFiles();

        BuildArguments _build_arguments = BuildTools.GetBuildArguments();
        
        if (_build_arguments.safedk)
        {
            AddSafeDK();
        }

        RunPostProcessTasksAndroid();
#endif
    }

    static void AddSafeDK()
    {
        string _url = "https://apps.tutotoons.com/";

        UnityWebRequest _www = UnityWebRequest.Get(_url + TutoTOONS.SystemUtils.GetBundleID() + ".android.json");
        _www.SendWebRequest();
        while (!_www.isDone)
        {
            //Wait
        }
        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log("Error: Failed to get app config for safedk: " + _www.error);
            return;
        }

        string _result = _www.downloadHandler.text;

        TutoTOONS.AppConfigData _data = JsonUtility.FromJson<TutoTOONS.AppConfigData>(_result);

        string _content = File.ReadAllText(Application.dataPath + "/Plugins/Android/launcherTemplate.gradle");

        if (!_content.Contains("//SAFEDKADDED"))
        {
            string _template = File.ReadAllText(Application.dataPath + "/Packages/TutoTOONS/Editor/safedkgradle.txt");

            _template = _template.Replace("*APP_ID*", _data.settings.safedk_id);
            _template = _template.Replace("*APP_KEY*", _data.settings.safedk_key);

            string _keyword = "//SAFEDKSTART";

            string _new_content = _content;
            _new_content = _new_content.Insert(_new_content.IndexOf(_keyword) + _keyword.Length + 1, _template);

            File.WriteAllText(Application.dataPath + "/Plugins/Android/launcherTemplate.gradle", _new_content);
        }
        string _content_base = File.ReadAllText(Application.dataPath + "/Plugins/Android/baseProjectTemplate.gradle");

        if (!_content_base.Contains("//SAFEDKADDED"))
        {
            string _keyword1 = "//SAFEDK1";
            string _keyword2 = "//SAFEDK2";
            string _keyword3 = "//SAFEDK3";

            string _new_content_base = _content_base;
            _new_content_base = _new_content_base.Insert(_new_content_base.IndexOf(_keyword1) + _keyword1.Length + 1, "classpath \"com.safedk:SafeDKGradlePlugin:1.+\"");
            _new_content_base = _new_content_base.Insert(_new_content_base.IndexOf(_keyword2) + _keyword2.Length + 1, "maven { url 'http://download.safedk.com/maven' }");
            _new_content_base = _new_content_base.Insert(_new_content_base.IndexOf(_keyword3) + _keyword3.Length + 1, "maven { url 'http://download.safedk.com/maven' }");
            _new_content_base += "\n//SAFEDKADDED";
            File.WriteAllText(Application.dataPath + "/Plugins/Android/baseProjectTemplate.gradle", _new_content_base);
        }
    }

    private void CleanBuildFiles()
    {
        if (!is_build_cleaned)
        {
            string _path_clean_android_manifest = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/AndroidManifest.xml");
            string _path_clean_gradle_template = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/mainTemplate.gradle");
            string _path_clean_gradle_base_template = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/baseProjectTemplate.gradle");
            string _path_clean_gradle_launcher_template = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/launcherTemplate.gradle");
            string _path_clean_gradle_properties = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/gradleTemplate.properties");
            string _path_current_android_manifest = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            string _path_current_gradle_template = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
            string _path_current_gradle_base_template = Path.Combine(Application.dataPath, "Plugins/Android/baseProjectTemplate.gradle");
            string _path_current_gradle_launcher_template = Path.Combine(Application.dataPath, "Plugins/Android/launcherTemplate.gradle");
            string _path_current_gradle_properties = Path.Combine(Application.dataPath, "Plugins/Android/gradleTemplate.properties");

            if (Directory.Exists(Path.Combine(Application.dataPath, "Plugins/Android/res")))
            {
                Directory.Delete(Path.Combine(Application.dataPath, "Plugins/Android/res"), true);
            }

            DeleteFile(_path_current_android_manifest);
            DeleteFile(_path_current_gradle_template);
            DeleteFile(_path_current_gradle_base_template);
            DeleteFile(_path_current_gradle_launcher_template);
            DeleteFile(_path_current_gradle_properties);
            CopyFile(_path_clean_android_manifest, _path_current_android_manifest);
            CopyFile(_path_clean_gradle_template, _path_current_gradle_template);
            CopyFile(_path_clean_gradle_base_template, _path_current_gradle_base_template);
            CopyFile(_path_clean_gradle_launcher_template, _path_current_gradle_launcher_template);
            CopyFile(_path_clean_gradle_properties, _path_current_gradle_properties);

            ProcessBuildFiles();

            // Temporary Amazon IAP manifest fix
            string _path_amazon_iap_sdk_new = Path.Combine(Application.dataPath, "Packages/TutoTOONS/Editor/BuildResources/AmazonAppStore.aar");
            string _path_amazon_iap_sdk = Path.Combine(Application.dataPath, "Plugins/UnityPurchasing/Bin/Android/AmazonAppStore.aar");
            if (File.Exists(_path_amazon_iap_sdk) && File.Exists(_path_amazon_iap_sdk_new))
            {
                DeleteFile(_path_amazon_iap_sdk);
                CopyFile(_path_amazon_iap_sdk_new, _path_amazon_iap_sdk);
            }
            // Temporary Amazon IAP manifest fix end




            ReadAndroidManifestDocument();

            is_build_cleaned = true;
        }
    }

    private static void ProcessBuildFiles()
    {
        string _path_gradle_template = Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle");
        string _path_gradle_launcher_template = Path.Combine(Application.dataPath, "Plugins/Android/launcherTemplate.gradle");
        string _path_gradle_properties = Path.Combine(Application.dataPath, "Plugins/Android/gradleTemplate.properties");
        string _path_gradle_base_template = Path.Combine(Application.dataPath, "Plugins/Android/baseProjectTemplate.gradle");

        #if UNITY_2020_1_OR_NEWER
        replaceSymbolInFile(_path_gradle_template, "**TUTO_COMPRESSION_SUPPORT**", "noCompress = ['.ress', '.resource', '.obb'] + unityStreamingAssets.tokenize(', ')");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_COMPRESSION_SUPPORT**", "noCompress = ['.ress', '.resource', '.obb'] + unityStreamingAssets.tokenize(', ')");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_PROGUARD_DEBUG_SUPPORT**", "");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_PROGUARD_RELEASE_SUPPORT**", "");
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_UNITY_STREAMING_ASSETS**", "unityStreamingAssets =.unity3d**STREAMING_ASSETS**");
        replaceSymbolInFile(_path_gradle_base_template, "**TUTO_GRADLE_VERSION**", "3.6.0");

        #if GEN_PLATFORM_AMAZON
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_R8_COMPILER**", "android.enableR8=false");
        #else
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_R8_COMPILER**", "android.enableR8=**MINIFY_WITH_R_EIGHT**");
        #endif

        #else
        replaceSymbolInFile(_path_gradle_template, "**TUTO_COMPRESSION_SUPPORT**", "");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_COMPRESSION_SUPPORT**", "noCompress = ['.unity3d', '.ress', '.resource', '.obb'**STREAMING_ASSETS**]");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_PROGUARD_DEBUG_SUPPORT**", "useProguard **PROGUARD_DEBUG**");
        replaceSymbolInFile(_path_gradle_launcher_template, "**TUTO_PROGUARD_RELEASE_SUPPORT**", "useProguard **PROGUARD_RELEASE**");
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_UNITY_STREAMING_ASSETS**", "");
        replaceSymbolInFile(_path_gradle_base_template, "**TUTO_GRADLE_VERSION**", "3.4.3");

        #if GEN_PLATFORM_AMAZON
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_R8_COMPILER**", "android.enableR8=false");
        #else
        replaceSymbolInFile(_path_gradle_properties, "**TUTO_R8_COMPILER**", "");
        #endif

        #endif
    }

    private static void replaceSymbolInFile(string _file_path, string _symbol, string _replace_text)
    {
        StreamReader _stream_reader = new StreamReader(_file_path);
        string _file_contents = _stream_reader.ReadToEnd();
        _stream_reader.Close();
        string _updated_file_contents = _file_contents.Replace(_symbol, _replace_text);
        File.WriteAllText(_file_path, _updated_file_contents);
    }

    public static XmlElement GenerateXMLElement(string _name, Attribute[] _attributes)
    {
        XmlElement _activity_element = android_manifest_document.CreateElement(_name);

        for (int i = 0; i < _attributes.Length; i++)
        {
            _activity_element.SetAttribute("android__" + _attributes[i].name, _attributes[i].value);
        }

        return _activity_element;
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        is_build_cleaned = false;
    }

    public virtual void RunPostProcessTasksAndroid() { }

    public virtual void RunPostProcessTasksiOS() { }

    public static void CopyDirectoryRecursive(string _source_dir, string _target_dir)
    {
        DirectoryInfo _source_dir_info = new DirectoryInfo(_source_dir);
        DirectoryInfo _traget_dir_info = new DirectoryInfo(_target_dir);
        CopyAllFiles(_source_dir_info, _traget_dir_info);
    }

    public static void CopyAllFiles(DirectoryInfo _source_info, DirectoryInfo _target_info)
    {
        Directory.CreateDirectory(_target_info.FullName);

        foreach (FileInfo _file_info in _source_info.GetFiles())
        {
            if (_file_info.Name.EndsWith("meta"))
            {
                continue;
            }

            _file_info.CopyTo(Path.Combine(_target_info.FullName, _file_info.Name), true);
        }

        foreach (DirectoryInfo _sub_dir in _source_info.GetDirectories())
        {
            DirectoryInfo _target_sub_dir = _target_info.CreateSubdirectory(_sub_dir.Name);
            CopyAllFiles(_sub_dir, _target_sub_dir);
        }
    }

    public static void CopyFile(string path_source, string path_traget)
    {
        FileInfo _source_file = new FileInfo(path_source);
        _source_file.CopyTo(path_traget);
    }

    public static void DeleteFile(string path_file)
    {
        if (File.Exists(path_file))
        {
            File.Delete(path_file);
        }
    }

    public static void CleanManifestFile(string manifestPath)
    {
        TextReader _stream_reader = new StreamReader(manifestPath);
        string _file_content = _stream_reader.ReadToEnd();
        _stream_reader.Close();

        Regex _regex = new Regex("android__");
        _file_content = _regex.Replace(_file_content, "android:");

        TextWriter _stream_writer = new StreamWriter(manifestPath);
        _stream_writer.Write(_file_content);
        _stream_writer.Close();
    }

    public static void ReadAndroidManifestDocument()
    {
        if (android_manifest_document == null)
        {
            android_manifest_document = new XmlDocument();
            android_manifest_document.Load(path_to_android_manifest);
        }
    }

    public static void AddApplicationNodeToManifest(string _node_type, string _name, XmlElement _node_to_add)
    {
        XmlElement _manifest_root = android_manifest_document.DocumentElement;
        XmlNode _application_node = null;

        foreach (XmlNode _node in _manifest_root.ChildNodes)
        {
            if (_node.Name == "application")
            {
                _application_node = _node;
                break;
            }
        }

        if (_application_node == null)
        {
            Debug.LogError("There is no application node in AndroidManifest.xml");
            return;
        }

        List<XmlNode> _activity_nodes = GetNodes(_application_node, _node_type);

        bool _node_exists = false;

        for (int i = 0; i < _activity_nodes.Count; i++)
        {
            foreach (XmlAttribute _attribute in _activity_nodes[i].Attributes)
            {
                if (_attribute.Value.Contains(_name))
                {
                    _node_exists = true;
                }
            }
        }

        if (!_node_exists)
        {
            _application_node.AppendChild(_node_to_add);
        }

        android_manifest_document.Save(path_to_android_manifest);
        CleanManifestFile(path_to_android_manifest);
    }

    public static List<XmlNode> GetNodes(XmlNode _application_node, string _node_name)
    {
        List<XmlNode> _nodes = new List<XmlNode>();
        foreach (XmlNode _node in _application_node.ChildNodes)
        {
            if (_node.Name == _node_name)
            {
                _nodes.Add(_node);
            }
        }
        return _nodes;
    }

    public static void AddGradleDependeny(string _dependency)
    {
        TextReader _gradle_reader = new StreamReader(Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle"));
        string _gradle_content = _gradle_reader.ReadToEnd();
        _gradle_reader.Close();
        Regex _regex = new Regex("(?s)(?<=\\/\\/ Dependencies start).*(?=\\/\\/ Dependencies end)");
        Match _dependecy_match = _regex.Match(_gradle_content);
        string _dependency_list = _dependecy_match.Value;
        _dependency_list += "\n" + _dependency;
        _gradle_content = _regex.Replace(_gradle_content, _dependency_list);
        TextWriter _gradle_writer = new StreamWriter(Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle"));
        _gradle_writer.Write(_gradle_content);
        _gradle_writer.Close();
    }

    public static void AddGradleDependeny(string _dependency, string _conflicting_dependency)
    {
        TextReader _gradle_reader = new StreamReader(Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle"));
        string _gradle_content = _gradle_reader.ReadToEnd();
        _gradle_reader.Close();
        Regex _regex = new Regex("(?s)(?<=\\/\\/ Dependencies start).*(?=\\/\\/ Dependencies end)");
        Match _dependecy_match = _regex.Match(_gradle_content);
        string _dependency_list = _dependecy_match.Value;
        if (_dependency_list.IndexOf(_conflicting_dependency) == -1)
        {
            _dependency_list += "\n" + _dependency;
        }
        _gradle_content = _regex.Replace(_gradle_content, _dependency_list);
        TextWriter _gradle_writer = new StreamWriter(Path.Combine(Application.dataPath, "Plugins/Android/mainTemplate.gradle"));
        _gradle_writer.Write(_gradle_content);
        _gradle_writer.Close();
    }

    public static void AddCocoaPodsDependeny(string _root_path, string _dependency)
    {
#if UNITY_IOS
        TextReader _podfile_reader = new StreamReader(_root_path + "/Podfile");
        string _podfile_content = _podfile_reader.ReadToEnd();
        _podfile_reader.Close();

        var _keyword = "#Dependencies start";
        var _index = _podfile_content.IndexOf(_keyword);
        _index += _keyword.Length + 1;
        while (_index >= 0)
        {
            var _val = _dependency + "\n";
            _podfile_content = _podfile_content.Insert(_index, _val);
            _index += _val.Length;
            _index = _podfile_content.IndexOf(_keyword, _index);
            if (_index < 0) break;
            _index += _keyword.Length + 1;
        }

        TextWriter _pofile_writer = new StreamWriter(_root_path + "/Podfile");
        _pofile_writer.Write(_podfile_content);
        _pofile_writer.Close();
#endif
    }

    public static void RemovePrefab(string _name_prefab_to_remove)
    {
        string _path_tutotoons_prefab = "Assets/Packages/TutoTOONS/TutoTOONS.prefab";
        GameObject _tutotoons = PrefabUtility.LoadPrefabContents(_path_tutotoons_prefab);

        for (int i = 0; i < _tutotoons.GetComponent<Transform>().childCount; i++)
        {
            if (_tutotoons.GetComponent<Transform>().GetChild(i).name.Equals(_name_prefab_to_remove))
            {
                _tutotoons.GetComponent<Transform>().GetChild(i).parent = null;
            }
        }

        PrefabUtility.SaveAsPrefabAsset(_tutotoons, _path_tutotoons_prefab);
        PrefabUtility.UnloadPrefabContents(_tutotoons);
    }

    public static void AddPrefab(string _path_to_prefab)
    {
        string _path_tutotoons_prefab = "Assets/Packages/TutoTOONS/TutoTOONS.prefab";
        GameObject _tutotoons = PrefabUtility.LoadPrefabContents(_path_tutotoons_prefab);
        GameObject _prefab_to_add = PrefabUtility.LoadPrefabContents(_path_to_prefab);
        _prefab_to_add.GetComponent<Transform>().parent = _tutotoons.GetComponent<Transform>();
        PrefabUtility.RecordPrefabInstancePropertyModifications(_prefab_to_add.GetComponent<Transform>());
        PrefabUtility.SaveAsPrefabAsset(_tutotoons, _path_tutotoons_prefab);

        PrefabUtility.UnloadPrefabContents(_tutotoons);
    }
        
    public static AdNetworkKeys GetAdNetworkKeys(string _ad_network)
    {
        string _url = "https://apps.tutotoons.com/";

        //TODO: may need to include UDP platforms later

        string _platform_name = TutoTOONS.AppConfig.PLATFORM_NAME_ANDROID;

#if UNITY_IOS
        _platform_name = TutoTOONS.AppConfig.PLATFORM_NAME_IOS;
#endif

#if GEN_PLATFORM_AMAZON
		    _platform_name = TutoTOONS.AppConfig.PLATFORM_NAME_AMAZON;
#endif

        UnityWebRequest _www = UnityWebRequest.Get(_url + TutoTOONS.SystemUtils.GetBundleID() + "." + _platform_name + ".json");
        _www.SendWebRequest();
        while (!_www.isDone)
        {
            //Wait
        }
        if (_www.isNetworkError || _www.isHttpError)
        {
            Debug.Log("Error: Failed to get " + _ad_network + " keys: " + _www.error);
            return null;
        }

        string _result = _www.downloadHandler.text;

        TutoTOONS.AppConfigData _data = JsonUtility.FromJson<TutoTOONS.AppConfigData>(_result);
        AdNetworkKeys _keys = new AdNetworkKeys();

        for (int i = 0; i < _data.ad_networks.Count; i++)
        {
            if (_data.ad_networks[i].keyword == _ad_network)
            {
                _keys.key1 = _data.ad_networks[i].settings.key1;
                _keys.key2 = _data.ad_networks[i].settings.key2;
                _keys.key3 = _data.ad_networks[i].settings.key3;

                _keys.key1_no_comp = _data.ad_networks[i].settings.key1_no_comp;
                _keys.key2_no_comp = _data.ad_networks[i].settings.key2_no_comp;
                _keys.key3_no_comp = _data.ad_networks[i].settings.key3_no_comp;

                break;
            }
        }

        return _keys;
    }
}
    

public class Attribute
{
    public string name { get; set; }
    public string value { get; set; }

    public Attribute(string _name, string _value)
    {
        name = _name;
        value = _value;
    }
}

public class AdNetworkKeys
{
    public string key1, key2, key3, key1_no_comp, key2_no_comp, key3_no_comp;
}