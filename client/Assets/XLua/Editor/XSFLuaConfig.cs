/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using XLua;
using System.Reflection;
using System.Linq;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class ExampleConfig
{
    /***************如果你全lua编程，可以参考这份自动化配置***************/
    //--------------begin 纯lua编程配置参考----------------------------
    static List<string> exclude = new List<string> {
       "HideInInspector", "ExecuteInEditMode",
       "AddComponentMenu", "ContextMenu",
       "RequireComponent", "DisallowMultipleComponent",
       "SerializeField", "AssemblyIsEditorAssembly",
       "Attribute", "Types",
       "UnitySurrogateSelector", "TrackedReference",
       "TypeInferenceRules", "FFTWindow",
       "RPC", "Network", "MasterServer",
       "BitStream", "HostData",
       "ConnectionTesterStatus", "GUI", "EventType",
       "EventModifiers", "FontStyle", "TextAlignment",
       "TextEditor", "TextEditorDblClickSnapping",
       "TextGenerator", "TextClipping", "Gizmos",
       "ADBannerView", "ADInterstitialAd",
       "Android", "Tizen", "jvalue",
       "iPhone", "iOS", "Windows", "CalendarIdentifier",
       "CalendarUnit", "CalendarUnit",
       "ClusterInput", "FullScreenMovieControlMode",
       "FullScreenMovieScalingMode", "Handheld",
       "LocalNotification", "NotificationServices",
       "RemoteNotificationType", "RemoteNotification",
       "SamsungTV", "TextureCompressionQuality",
       "TouchScreenKeyboardType", "TouchScreenKeyboard",
       "MovieTexture", "UnityEngineInternal",
       "Terrain", "Tree", "SplatPrototype",
       "DetailPrototype", "DetailRenderMode",
       "MeshSubsetCombineUtility", "AOT", "Social", "Enumerator",
       "SendMouseEvents", "Cursor", "Flash", "ActionScript",
       "OnRequestRebuild", "Ping",
       "ShaderVariantCollection", "SimpleJson.Reflection",
       "CoroutineTween", "GraphicRebuildTracker",
       "Advertisements", "UnityEditor", "WSA",
       "EventProvider", "Apple",
       "ClusterInput", "Motion",
       "UnityEngine.UI.ReflectionMethodsCache", "NativeLeakDetection",
       "NativeLeakDetectionMode", "WWWAudioExtensions", "UnityEngine.Experimental",
    };

    static bool isExcluded(Type type)
    {
       var fullName = type.FullName;
       for (int i = 0; i < exclude.Count; i++)
       {
           if (fullName.Contains(exclude[i]))
           {
               return true;
           }
       }
       return false;
    }

    [LuaCallCSharp]
    public static IEnumerable<Type> LuaCallCSharp
    {
       get
       {
            List<Type> typeList = new List<Type>() {
                typeof(System.Object),
                typeof(UnityEngine.Object),
                typeof(Array),
                typeof(UnityEngine.Vector2),
                typeof(UnityEngine.Vector3),
                typeof(UnityEngine.Vector4),
                typeof(UnityEngine.Quaternion),
                typeof(UnityEngine.Time),
                typeof(UnityEngine.GameObject),
                typeof(UnityEngine.Component),
                typeof(UnityEngine.Behaviour),
                typeof(UnityEngine.Transform),
                typeof(UnityEngine.TextAsset),
                typeof(UnityEngine.MonoBehaviour),
                typeof(UnityEngine.Mathf),
                typeof(UnityEngine.Canvas),
                typeof(TMPro.TMP_Text),
                typeof(TMPro.TextMeshProUGUI),
                typeof(UnityEngine.UI.Slider),
                typeof(UnityEngine.UI.Image),
                typeof(UnityEngine.Sprite),
                typeof(UnityEngine.RectTransform),
                typeof(TMPro.TMP_InputField),
                typeof(TMPro.TMP_Dropdown),
                typeof(UnityEngine.UI.Toggle),
                typeof(UnityEngine.Camera),
            };

            var unityTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                where !(assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder)
                                from type in assembly.GetExportedTypes()
                                where typeList.Contains(type) && !isExcluded(type)
                                        && type.BaseType != typeof(MulticastDelegate) && !type.IsInterface && !type.IsEnum
                                select type);


            return unityTypes;
       }
    }

    //--------------end 纯lua编程配置参考----------------------------

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
            };

#if UNITY_2018_1_OR_NEWER
    [BlackList]
    public static Func<MemberInfo, bool> MethodFilter = (memberInfo) =>
    {
        if (memberInfo.DeclaringType.IsGenericType && memberInfo.DeclaringType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            if (memberInfo.MemberType == MemberTypes.Constructor)
            {
                ConstructorInfo constructorInfo = memberInfo as ConstructorInfo;
                var parameterInfos = constructorInfo.GetParameters();
                if (parameterInfos.Length > 0)
                {
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(parameterInfos[0].ParameterType))
                    {
                        return true;
                    }
                }
            }
            else if (memberInfo.MemberType == MemberTypes.Method)
            {
                var methodInfo = memberInfo as MethodInfo;
                if (methodInfo.Name == "TryAdd" || methodInfo.Name == "Remove" && methodInfo.GetParameters().Length == 2)
                {
                    return true;
                }
            }
        }
        return false;
    };
#endif
}
