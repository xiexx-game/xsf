// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Common.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace XsfPb {

  /// <summary>Holder for reflection information generated from Common.proto</summary>
  public static partial class CommonReflection {

    #region Descriptor
    /// <summary>File descriptor for Common.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CommonReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxDb21tb24ucHJvdG8SBnhzZl9wYipNCglBY3RvclByb3ASDAoITmlja25h",
            "bWUQABILCgdBY3RvcklEEAESDAoISGVhZGljb24QAhIOCgpDcmVhdGVUaW1l",
            "EAMSBwoDTWF4EARiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::XsfPb.ActorProp), }, null));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  /// 角色属性
  /// </summary>
  public enum ActorProp {
    /// <summary>
    /// 昵称
    /// </summary>
    [pbr::OriginalName("Nickname")] Nickname = 0,
    /// <summary>
    /// actor_id
    /// </summary>
    [pbr::OriginalName("ActorID")] ActorId = 1,
    /// <summary>
    /// 头像
    /// </summary>
    [pbr::OriginalName("Headicon")] Headicon = 2,
    /// <summary>
    /// 出生时间
    /// </summary>
    [pbr::OriginalName("CreateTime")] CreateTime = 3,
    [pbr::OriginalName("Max")] Max = 4,
  }

  #endregion

}

#endregion Designer generated code