// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: Common.proto

#include "Common.pb.h"

#include <algorithm>

#include <google/protobuf/io/coded_stream.h>
#include <google/protobuf/extension_set.h>
#include <google/protobuf/wire_format_lite.h>
#include <google/protobuf/descriptor.h>
#include <google/protobuf/generated_message_reflection.h>
#include <google/protobuf/reflection_ops.h>
#include <google/protobuf/wire_format.h>
// @@protoc_insertion_point(includes)
#include <google/protobuf/port_def.inc>
namespace xsf_pb {
}  // namespace xsf_pb
static constexpr ::PROTOBUF_NAMESPACE_ID::Metadata* file_level_metadata_Common_2eproto = nullptr;
static const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* file_level_enum_descriptors_Common_2eproto[2];
static constexpr ::PROTOBUF_NAMESPACE_ID::ServiceDescriptor const** file_level_service_descriptors_Common_2eproto = nullptr;
const ::PROTOBUF_NAMESPACE_ID::uint32 TableStruct_Common_2eproto::offsets[1] = {};
static constexpr ::PROTOBUF_NAMESPACE_ID::internal::MigrationSchema* schemas = nullptr;
static constexpr ::PROTOBUF_NAMESPACE_ID::Message* const* file_default_instances = nullptr;

const char descriptor_table_protodef_Common_2eproto[] PROTOBUF_SECTION_VARIABLE(protodesc_cold) =
  "\n\014Common.proto\022\006xsf_pb*h\n\020DisconnectReas"
  "on\022\013\n\007DR_None\020\000\022\010\n\004Full\020\001\022\r\n\tHTTimeout\020\002"
  "\022\016\n\nServerDown\020\003\022\016\n\nMsgInvalid\020\004\022\016\n\nLogi"
  "nError\020\005*+\n\013LoginResult\022\013\n\007Success\020\000\022\017\n\013"
  "SystemError\020\001b\006proto3"
  ;
static const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable*const descriptor_table_Common_2eproto_deps[1] = {
};
static ::PROTOBUF_NAMESPACE_ID::internal::SCCInfoBase*const descriptor_table_Common_2eproto_sccs[1] = {
};
static ::PROTOBUF_NAMESPACE_ID::internal::once_flag descriptor_table_Common_2eproto_once;
static bool descriptor_table_Common_2eproto_initialized = false;
const ::PROTOBUF_NAMESPACE_ID::internal::DescriptorTable descriptor_table_Common_2eproto = {
  &descriptor_table_Common_2eproto_initialized, descriptor_table_protodef_Common_2eproto, "Common.proto", 181,
  &descriptor_table_Common_2eproto_once, descriptor_table_Common_2eproto_sccs, descriptor_table_Common_2eproto_deps, 0, 0,
  schemas, file_default_instances, TableStruct_Common_2eproto::offsets,
  file_level_metadata_Common_2eproto, 0, file_level_enum_descriptors_Common_2eproto, file_level_service_descriptors_Common_2eproto,
};

// Force running AddDescriptors() at dynamic initialization time.
static bool dynamic_init_dummy_Common_2eproto = (  ::PROTOBUF_NAMESPACE_ID::internal::AddDescriptors(&descriptor_table_Common_2eproto), true);
namespace xsf_pb {
const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* DisconnectReason_descriptor() {
  ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&descriptor_table_Common_2eproto);
  return file_level_enum_descriptors_Common_2eproto[0];
}
bool DisconnectReason_IsValid(int value) {
  switch (value) {
    case 0:
    case 1:
    case 2:
    case 3:
    case 4:
    case 5:
      return true;
    default:
      return false;
  }
}

const ::PROTOBUF_NAMESPACE_ID::EnumDescriptor* LoginResult_descriptor() {
  ::PROTOBUF_NAMESPACE_ID::internal::AssignDescriptors(&descriptor_table_Common_2eproto);
  return file_level_enum_descriptors_Common_2eproto[1];
}
bool LoginResult_IsValid(int value) {
  switch (value) {
    case 0:
    case 1:
      return true;
    default:
      return false;
  }
}


// @@protoc_insertion_point(namespace_scope)
}  // namespace xsf_pb
PROTOBUF_NAMESPACE_OPEN
PROTOBUF_NAMESPACE_CLOSE

// @@protoc_insertion_point(global_scope)
#include <google/protobuf/port_undef.inc>
