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
            "CgxDb21tb24ucHJvdG8SBnhzZl9wYipoChBEaXNjb25uZWN0UmVhc29uEgsK",
            "B0RSX05vbmUQABIICgRGdWxsEAESDQoJSFRUaW1lb3V0EAISDgoKU2VydmVy",
            "RG93bhADEg4KCk1zZ0ludmFsaWQQBBIOCgpMb2dpbkVycm9yEAUqKwoLTG9n",
            "aW5SZXN1bHQSCwoHU3VjY2VzcxAAEg8KC1N5c3RlbUVycm9yEAEqdAoIT3BS",
            "ZXN1bHQSBgoCT2sQABIVChFNeXNxbF9TY2hlbWFFcnJvchABEhYKEk15c3Fs",
            "X1Bvb2xOb3RFeGlzdBACEhkKFU15c3FsX1NxbEJhc2VOb3RFeGlzdBADEhYK",
            "Ek15c3FsX0V4ZWN1dGVFcnJvchAEYgZwcm90bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::XsfPb.DisconnectReason), typeof(global::XsfPb.LoginResult), typeof(global::XsfPb.OpResult), }, null));
    }
    #endregion

  }
  #region Enums
  public enum DisconnectReason {
    [pbr::OriginalName("DR_None")] DrNone = 0,
    /// <summary>
    /// 服务器已满
    /// </summary>
    [pbr::OriginalName("Full")] Full = 1,
    /// <summary>
    /// 心跳超时
    /// </summary>
    [pbr::OriginalName("HTTimeout")] Httimeout = 2,
    /// <summary>
    /// 服务器关闭
    /// </summary>
    [pbr::OriginalName("ServerDown")] ServerDown = 3,
    /// <summary>
    /// 消息非法
    /// </summary>
    [pbr::OriginalName("MsgInvalid")] MsgInvalid = 4,
    /// <summary>
    /// 登录错误
    /// </summary>
    [pbr::OriginalName("LoginError")] LoginError = 5,
  }

  public enum LoginResult {
    /// <summary>
    /// 登录成功
    /// </summary>
    [pbr::OriginalName("Success")] Success = 0,
    /// <summary>
    /// 系统错误
    /// </summary>
    [pbr::OriginalName("SystemError")] SystemError = 1,
  }

  public enum OpResult {
    [pbr::OriginalName("Ok")] Ok = 0,
    [pbr::OriginalName("Mysql_SchemaError")] MysqlSchemaError = 1,
    [pbr::OriginalName("Mysql_PoolNotExist")] MysqlPoolNotExist = 2,
    [pbr::OriginalName("Mysql_SqlBaseNotExist")] MysqlSqlBaseNotExist = 3,
    [pbr::OriginalName("Mysql_ExecuteError")] MysqlExecuteError = 4,
  }

  #endregion

}

#endregion Designer generated code
