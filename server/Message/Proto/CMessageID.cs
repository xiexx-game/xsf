// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: CMessageID.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace XsfPb {

  /// <summary>Holder for reflection information generated from CMessageID.proto</summary>
  public static partial class CMessageIDReflection {

    #region Descriptor
    /// <summary>File descriptor for CMessageID.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static CMessageIDReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChBDTWVzc2FnZUlELnByb3RvEgZ4c2ZfcGIq1AEKBkNNU0dJRBIPCgtDTVNH",
            "SURfTm9uZRAAEhQKEENsdF9HdF9IYW5kc2hha2UQARIUChBHdF9DbHRfSGFu",
            "ZHNoYWtlEAISFAoQQ2x0X0d0X0hlYXJ0YmVhdBADEhQKEEd0X0NsdF9IZWFy",
            "dGJlYXQQBBIVChFHdF9DbHRfRGlzY29ubmVjdBAFEg8KC0NsdF9HX0xvZ2lu",
            "EAYSFQoRR19DbHRfTG9naW5SZXN1bHQQBxISCg5HX0NsdF9UZXN0RGF0YRAI",
            "Eg4KCkNNU0dJRF9NYXgQZGIGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::XsfPb.CMSGID), }, null));
    }
    #endregion

  }
  #region Enums
  /// <summary>
  /// 客户端消息ID
  /// </summary>
  public enum CMSGID {
    /// <summary>
    ///MESSAGE_ID_START
    /// </summary>
    [pbr::OriginalName("CMSGID_None")] None = 0,
    /// <summary>
    /// client --> Gate 握手
    /// </summary>
    [pbr::OriginalName("Clt_Gt_Handshake")] CltGtHandshake = 1,
    /// <summary>
    /// gate --> client 握手反馈
    /// </summary>
    [pbr::OriginalName("Gt_Clt_Handshake")] GtCltHandshake = 2,
    /// <summary>
    /// client --> gate 心跳
    /// </summary>
    [pbr::OriginalName("Clt_Gt_Heartbeat")] CltGtHeartbeat = 3,
    /// <summary>
    /// gate --> client 心跳反馈
    /// </summary>
    [pbr::OriginalName("Gt_Clt_Heartbeat")] GtCltHeartbeat = 4,
    /// <summary>
    /// gate --> client 服务器断开连接
    /// </summary>
    [pbr::OriginalName("Gt_Clt_Disconnect")] GtCltDisconnect = 5,
    /// <summary>
    /// client --> login 登录
    /// </summary>
    [pbr::OriginalName("Clt_G_Login")] CltGLogin = 6,
    /// <summary>
    /// game --> client 登录结果
    /// </summary>
    [pbr::OriginalName("G_Clt_LoginResult")] GCltLoginResult = 7,
    /// <summary>
    /// game --> client 测试协议
    /// </summary>
    [pbr::OriginalName("G_Clt_TestData")] GCltTestData = 8,
    /// <summary>
    ///MESSAGE_ID_END
    /// </summary>
    [pbr::OriginalName("CMSGID_Max")] Max = 100,
  }

  #endregion

}

#endregion Designer generated code
