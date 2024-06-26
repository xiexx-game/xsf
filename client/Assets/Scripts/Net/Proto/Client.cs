// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Client.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace XsfPb {

  /// <summary>Holder for reflection information generated from Client.proto</summary>
  public static partial class ClientReflection {

    #region Descriptor
    /// <summary>File descriptor for Client.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ClientReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "CgxDbGllbnQucHJvdG8SBnhzZl9wYiISChBDbHRfR3RfSGFuZHNoYWtlIhIK",
            "EEd0X0NsdF9IYW5kc2hha2UiIAoQQ2x0X0d0X0hlYXJ0YmVhdBIMCgR0aW1l",
            "GAEgASgEIjwKEEd0X0NsdF9IZWFydGJlYXQSEwoLY2xpZW50X3RpbWUYASAB",
            "KAQSEwoLc2VydmVyX3RpbWUYAiABKAQiMAoLQ2x0X0dfTG9naW4SDwoHYWNj",
            "b3VudBgBIAEoCRIQCghwYXNzd29yZBgCIAEoCSIjChFHdF9DbHRfRGlzY29u",
            "bmVjdBIOCgZyZWFzb24YASABKAUiIwoRR19DbHRfTG9naW5SZXN1bHQSDgoG",
            "cmVzdWx0GAEgASgNIiEKDkdfQ2x0X1Rlc3REYXRhEg8KB21lc3NhZ2UYASAB",
            "KAliBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Clt_Gt_Handshake), global::XsfPb.Clt_Gt_Handshake.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Gt_Clt_Handshake), global::XsfPb.Gt_Clt_Handshake.Parser, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Clt_Gt_Heartbeat), global::XsfPb.Clt_Gt_Heartbeat.Parser, new[]{ "Time" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Gt_Clt_Heartbeat), global::XsfPb.Gt_Clt_Heartbeat.Parser, new[]{ "ClientTime", "ServerTime" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Clt_G_Login), global::XsfPb.Clt_G_Login.Parser, new[]{ "Account", "Password" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.Gt_Clt_Disconnect), global::XsfPb.Gt_Clt_Disconnect.Parser, new[]{ "Reason" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.G_Clt_LoginResult), global::XsfPb.G_Clt_LoginResult.Parser, new[]{ "Result" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::XsfPb.G_Clt_TestData), global::XsfPb.G_Clt_TestData.Parser, new[]{ "Message" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// client --> gate 握手请求
  /// </summary>
  public sealed partial class Clt_Gt_Handshake : pb::IMessage<Clt_Gt_Handshake> {
    private static readonly pb::MessageParser<Clt_Gt_Handshake> _parser = new pb::MessageParser<Clt_Gt_Handshake>(() => new Clt_Gt_Handshake());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Clt_Gt_Handshake> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Handshake() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Handshake(Clt_Gt_Handshake other) : this() {
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Handshake Clone() {
      return new Clt_Gt_Handshake(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Clt_Gt_Handshake);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Clt_Gt_Handshake other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Clt_Gt_Handshake other) {
      if (other == null) {
        return;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
        }
      }
    }

  }

  /// <summary>
  /// gate --> client 握手反馈
  /// </summary>
  public sealed partial class Gt_Clt_Handshake : pb::IMessage<Gt_Clt_Handshake> {
    private static readonly pb::MessageParser<Gt_Clt_Handshake> _parser = new pb::MessageParser<Gt_Clt_Handshake>(() => new Gt_Clt_Handshake());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Gt_Clt_Handshake> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Handshake() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Handshake(Gt_Clt_Handshake other) : this() {
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Handshake Clone() {
      return new Gt_Clt_Handshake(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Gt_Clt_Handshake);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Gt_Clt_Handshake other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Gt_Clt_Handshake other) {
      if (other == null) {
        return;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
        }
      }
    }

  }

  /// <summary>
  /// client --> gate 心跳 请求
  /// </summary>
  public sealed partial class Clt_Gt_Heartbeat : pb::IMessage<Clt_Gt_Heartbeat> {
    private static readonly pb::MessageParser<Clt_Gt_Heartbeat> _parser = new pb::MessageParser<Clt_Gt_Heartbeat>(() => new Clt_Gt_Heartbeat());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Clt_Gt_Heartbeat> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Heartbeat() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Heartbeat(Clt_Gt_Heartbeat other) : this() {
      time_ = other.time_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_Gt_Heartbeat Clone() {
      return new Clt_Gt_Heartbeat(this);
    }

    /// <summary>Field number for the "time" field.</summary>
    public const int TimeFieldNumber = 1;
    private ulong time_;
    /// <summary>
    /// 客户端发起心跳当前时间毫秒数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong Time {
      get { return time_; }
      set {
        time_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Clt_Gt_Heartbeat);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Clt_Gt_Heartbeat other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Time != other.Time) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Time != 0UL) hash ^= Time.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Time != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(Time);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Time != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(Time);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Clt_Gt_Heartbeat other) {
      if (other == null) {
        return;
      }
      if (other.Time != 0UL) {
        Time = other.Time;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Time = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// gate --> client 心跳 反馈
  /// </summary>
  public sealed partial class Gt_Clt_Heartbeat : pb::IMessage<Gt_Clt_Heartbeat> {
    private static readonly pb::MessageParser<Gt_Clt_Heartbeat> _parser = new pb::MessageParser<Gt_Clt_Heartbeat>(() => new Gt_Clt_Heartbeat());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Gt_Clt_Heartbeat> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Heartbeat() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Heartbeat(Gt_Clt_Heartbeat other) : this() {
      clientTime_ = other.clientTime_;
      serverTime_ = other.serverTime_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Heartbeat Clone() {
      return new Gt_Clt_Heartbeat(this);
    }

    /// <summary>Field number for the "client_time" field.</summary>
    public const int ClientTimeFieldNumber = 1;
    private ulong clientTime_;
    /// <summary>
    /// 客户端发起心跳当前时间毫秒数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong ClientTime {
      get { return clientTime_; }
      set {
        clientTime_ = value;
      }
    }

    /// <summary>Field number for the "server_time" field.</summary>
    public const int ServerTimeFieldNumber = 2;
    private ulong serverTime_;
    /// <summary>
    /// 服务器回复心跳当前时间毫秒数
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public ulong ServerTime {
      get { return serverTime_; }
      set {
        serverTime_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Gt_Clt_Heartbeat);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Gt_Clt_Heartbeat other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ClientTime != other.ClientTime) return false;
      if (ServerTime != other.ServerTime) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ClientTime != 0UL) hash ^= ClientTime.GetHashCode();
      if (ServerTime != 0UL) hash ^= ServerTime.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (ClientTime != 0UL) {
        output.WriteRawTag(8);
        output.WriteUInt64(ClientTime);
      }
      if (ServerTime != 0UL) {
        output.WriteRawTag(16);
        output.WriteUInt64(ServerTime);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ClientTime != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ClientTime);
      }
      if (ServerTime != 0UL) {
        size += 1 + pb::CodedOutputStream.ComputeUInt64Size(ServerTime);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Gt_Clt_Heartbeat other) {
      if (other == null) {
        return;
      }
      if (other.ClientTime != 0UL) {
        ClientTime = other.ClientTime;
      }
      if (other.ServerTime != 0UL) {
        ServerTime = other.ServerTime;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            ClientTime = input.ReadUInt64();
            break;
          }
          case 16: {
            ServerTime = input.ReadUInt64();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// client --> login 登录
  /// </summary>
  public sealed partial class Clt_G_Login : pb::IMessage<Clt_G_Login> {
    private static readonly pb::MessageParser<Clt_G_Login> _parser = new pb::MessageParser<Clt_G_Login>(() => new Clt_G_Login());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Clt_G_Login> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[4]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_G_Login() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_G_Login(Clt_G_Login other) : this() {
      account_ = other.account_;
      password_ = other.password_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Clt_G_Login Clone() {
      return new Clt_G_Login(this);
    }

    /// <summary>Field number for the "account" field.</summary>
    public const int AccountFieldNumber = 1;
    private string account_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Account {
      get { return account_; }
      set {
        account_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "password" field.</summary>
    public const int PasswordFieldNumber = 2;
    private string password_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Password {
      get { return password_; }
      set {
        password_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Clt_G_Login);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Clt_G_Login other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Account != other.Account) return false;
      if (Password != other.Password) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Account.Length != 0) hash ^= Account.GetHashCode();
      if (Password.Length != 0) hash ^= Password.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Account.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Account);
      }
      if (Password.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Password);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Account.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Account);
      }
      if (Password.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Password);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Clt_G_Login other) {
      if (other == null) {
        return;
      }
      if (other.Account.Length != 0) {
        Account = other.Account;
      }
      if (other.Password.Length != 0) {
        Password = other.Password;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Account = input.ReadString();
            break;
          }
          case 18: {
            Password = input.ReadString();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// gate --> client 服务器断开连接
  /// </summary>
  public sealed partial class Gt_Clt_Disconnect : pb::IMessage<Gt_Clt_Disconnect> {
    private static readonly pb::MessageParser<Gt_Clt_Disconnect> _parser = new pb::MessageParser<Gt_Clt_Disconnect>(() => new Gt_Clt_Disconnect());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<Gt_Clt_Disconnect> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[5]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Disconnect() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Disconnect(Gt_Clt_Disconnect other) : this() {
      reason_ = other.reason_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public Gt_Clt_Disconnect Clone() {
      return new Gt_Clt_Disconnect(this);
    }

    /// <summary>Field number for the "reason" field.</summary>
    public const int ReasonFieldNumber = 1;
    private int reason_;
    /// <summary>
    /// 断开原因
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int Reason {
      get { return reason_; }
      set {
        reason_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as Gt_Clt_Disconnect);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(Gt_Clt_Disconnect other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Reason != other.Reason) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Reason != 0) hash ^= Reason.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Reason != 0) {
        output.WriteRawTag(8);
        output.WriteInt32(Reason);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Reason != 0) {
        size += 1 + pb::CodedOutputStream.ComputeInt32Size(Reason);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(Gt_Clt_Disconnect other) {
      if (other == null) {
        return;
      }
      if (other.Reason != 0) {
        Reason = other.Reason;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Reason = input.ReadInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// game --> client 服务器断开连接
  /// </summary>
  public sealed partial class G_Clt_LoginResult : pb::IMessage<G_Clt_LoginResult> {
    private static readonly pb::MessageParser<G_Clt_LoginResult> _parser = new pb::MessageParser<G_Clt_LoginResult>(() => new G_Clt_LoginResult());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<G_Clt_LoginResult> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[6]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_LoginResult() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_LoginResult(G_Clt_LoginResult other) : this() {
      result_ = other.result_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_LoginResult Clone() {
      return new G_Clt_LoginResult(this);
    }

    /// <summary>Field number for the "result" field.</summary>
    public const int ResultFieldNumber = 1;
    private uint result_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public uint Result {
      get { return result_; }
      set {
        result_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as G_Clt_LoginResult);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(G_Clt_LoginResult other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Result != other.Result) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Result != 0) hash ^= Result.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Result != 0) {
        output.WriteRawTag(8);
        output.WriteUInt32(Result);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Result != 0) {
        size += 1 + pb::CodedOutputStream.ComputeUInt32Size(Result);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(G_Clt_LoginResult other) {
      if (other == null) {
        return;
      }
      if (other.Result != 0) {
        Result = other.Result;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 8: {
            Result = input.ReadUInt32();
            break;
          }
        }
      }
    }

  }

  /// <summary>
  /// game --> client 测试协议
  /// </summary>
  public sealed partial class G_Clt_TestData : pb::IMessage<G_Clt_TestData> {
    private static readonly pb::MessageParser<G_Clt_TestData> _parser = new pb::MessageParser<G_Clt_TestData>(() => new G_Clt_TestData());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<G_Clt_TestData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::XsfPb.ClientReflection.Descriptor.MessageTypes[7]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_TestData() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_TestData(G_Clt_TestData other) : this() {
      message_ = other.message_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public G_Clt_TestData Clone() {
      return new G_Clt_TestData(this);
    }

    /// <summary>Field number for the "message" field.</summary>
    public const int MessageFieldNumber = 1;
    private string message_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Message {
      get { return message_; }
      set {
        message_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as G_Clt_TestData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(G_Clt_TestData other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Message != other.Message) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Message.Length != 0) hash ^= Message.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (Message.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Message);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Message.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Message);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(G_Clt_TestData other) {
      if (other == null) {
        return;
      }
      if (other.Message.Length != 0) {
        Message = other.Message;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Message = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
