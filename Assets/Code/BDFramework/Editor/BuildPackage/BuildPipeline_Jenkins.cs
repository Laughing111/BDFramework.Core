using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.Build.Content;
using BDFramework.Editor.Asset;
using BDFramework.Editor.BuildPackage;
using Code.BDFramework.Editor;

namespace BDFramework.Editor
{
    static public class BuildPipeline_Jenkins
    {

        [MenuItem("BDFrameWork工具箱/打包/导出APK")]
        public static void GenEmptyApk()
        {
            BuildPipeline_Jenkins.BuildPackage_APK_Editor();
        }
        [MenuItem("BDFrameWork工具箱/打包/导出APK(RebuildAsset)")]
        public static void GenApk_RebuildAsset()
        {
            BuildPipeline_Jenkins.BuildALLAssetsAndBuildInPackage_APK_Editor();
        }
        
        [MenuItem("BDFrameWork工具箱/打包/导出XCode工程(ipa暂未实现)")]
        public static void GenIpa()
        {
            BuildPipeline_Jenkins.BuildALLAssetsAndBuildInPackage_iOS_Editor();
        }
        
        

        #region 命令行相关操作
        private static string[] _args;
        static void _setPass()
        {
            var pass = _getArg(_args.Length - 2);
            Debug.Log("KeyStore Password:" + pass);
            PlayerSettings.Android.keystorePass = pass;
            PlayerSettings.Android.keyaliasPass = pass;
        }

        static string _getOutPath()
        {
            return _getArg(_args.Length - 1);
        }

        static void _initArgs()
        {
            _args = Environment.GetCommandLineArgs();
        }

        static string _getArg(int index)
        {
            return _args[index];
        }
        #endregion
        
        #region Android

        static void _androidCheck()
        {
            var dir = Path.GetDirectoryName(Application.dataPath);
            var path = Path.Combine(dir, "SDK");
            path = Path.Combine(path, "user.keystore");

            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            PlayerSettings.Android.keystoreName = path.Replace("\\", "/");
        }

        /// <summary>
        /// build apk成为一个空包.
        ///  需要cmd传参
        /// </summary>
        static public void BuildEmpty_APK()
        {
            _initArgs();
            _androidCheck();
            _setPass();
            var outputPath = _getOutPath();
            UnityEditor.BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.Android,
                BuildOptions.None);
        }

        /// <summary>
        /// 只生成资源,
        /// 需要cmd传参
        /// </summary>
        static public void BuildALLAssets_APK()
        {
            _initArgs();
            _setPass();
            var outputPath = _getOutPath();
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.Android, outputPath);
        }

        /// <summary>
        /// build apk,Assetbunld 位于Streaming下~
        /// </summary>
        static public void BuildALLAssetsAndBuildInPackage_APK()
        {
            _initArgs();

            _androidCheck();

            _setPass();

            var outputPath = _getOutPath();

            //清空StreamingAsset
            var ios = IPath.Combine(Application.streamingAssetsPath, "iOS");
            if (Directory.Exists(ios))
            {
                Directory.Delete(ios, true);
            }

            var win = IPath.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(win))
            {
                Directory.Delete(win, true);
            }

            //开始项目一键打包
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.Android,
                Application.streamingAssetsPath);
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.Android, BuildOptions.None);
        }

        /// <summary>
        /// build apk,Assetbunld 位于Streaming下~
        /// </summary>
        static public void BuildALLAssetsAndBuildInPackage_APK_Editor()
        {
            //开始项目一键打包
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.Android,Application.streamingAssetsPath);
            BuildPackage_APK_Editor();
        }
        /// <summary>
        /// build apk,Assetbunld 位于Streaming下~
        /// </summary>
        static public void BuildPackage_APK_Editor()
        {
            if (!BDEditorHelper.EditorConfig.IsSetConfig())
            {
                BDebug.LogError("请注意设置apk keystore账号密码");
                return;
            }

            var absroot = Application.dataPath.Replace("Assets", "");
            PlayerSettings.Android.keystoreName =absroot  + BDEditorHelper.EditorConfig.Android.keystoreName;
            PlayerSettings.keystorePass =  BDEditorHelper.EditorConfig.Android.keystorePass;
            PlayerSettings.Android.keyaliasName= BDEditorHelper.EditorConfig.Android.keyaliasName;
            PlayerSettings.keyaliasPass =  BDEditorHelper.EditorConfig.Android.keyaliasPass;

            var outpath =  EditorUtility.OpenFolderPanel("选择导出文件夹", Application.dataPath, "");
            if (string.IsNullOrEmpty(outpath))
            {
                return;
            }
            var outputPath = IPath.Combine( outpath ,  Application.productName+".apk");
            Debug.Log(outputPath);
            //清空StreamingAsset
            var ios = IPath.Combine(Application.streamingAssetsPath, "iOS");
            if (Directory.Exists(ios))
            {
                Directory.Delete(ios, true);
            }
            var win = IPath.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(win))
            {
                Directory.Delete(win, true);
            }
            //开始项目一键打包
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.Android,BuildOptions.None);
            //
            EditorUtility.DisplayDialog("提示", "打包完成", "OK");
        }
        #endregion
        
        #region iOS

        /// <summary>
        /// build apk成为一个空包.
        ///  需要cmd传参
        /// </summary>
        static public void BuildEmpty_iOS()
        {
            _initArgs();
            _setPass();
            var outputPath = _getOutPath();
            UnityEditor.BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.iOS,
                BuildOptions.None);
        }



        /// <summary>
        /// 只生成资源,
        /// 需要cmd传参
        /// </summary>
        static public void BuildALLAssets_iOS()
        {
            _initArgs();
            var outputPath = _getOutPath();
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.IPhonePlayer, outputPath);
        }

        /// <summary>
        /// build apk,Assetbunld 位于Streaming下~
        /// </summary>
        static public void BuildALLAssetsAndBuildInPackage_iOS()
        {
            _initArgs();
            var outputPath = _getOutPath();

            //清空StreamingAsset
            var ios = IPath.Combine(Application.streamingAssetsPath, "Android");
            if (Directory.Exists(ios))
            {
                Directory.Delete(ios, true);
            }

            var win = IPath.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(win))
            {
                Directory.Delete(win, true);
            }

            //开始项目一键打包
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.IPhonePlayer,
                Application.streamingAssetsPath);
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.iOS, BuildOptions.None);
        }

        /// <summary>
        /// build apk,Assetbunld 位于Streaming下~
        /// </summary>
        static public void BuildALLAssetsAndBuildInPackage_iOS_Editor()
        {
            var outputPath = Application.streamingAssetsPath + "/" + DateTime.Now.ToShortTimeString() + ".apk";
            //清空StreamingAsset
            var and = IPath.Combine(Application.streamingAssetsPath, "Android");
            if (Directory.Exists(and))
            {
                Directory.Delete(and, true);
            }

            var win = IPath.Combine(Application.streamingAssetsPath, "Windows");
            if (Directory.Exists(win))
            {
                Directory.Delete(win, true);
            }

            //开始项目一键打包
            EditorWindow_OnekeyBuildAsset.OneKeyBuildALLAssets_ForBuildPackage(RuntimePlatform.IPhonePlayer,
                Application.streamingAssetsPath);
            UnityEditor.BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.iOS,
                BuildOptions.None);

            //TODO  Build 完之后，需要调用shell 编译xcode
        }

        #endregion
    }
}