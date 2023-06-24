-- client message id




-- 客户端消息ID
XSF.CMSGID = {
    None = 0,
    Clt_Gt_Handshake = 1,             -- client --> gate 握手请求
    Gt_Clt_Handshake = 2,             -- gate --> client 握手反馈
    Clt_Gt_Heartbeat = 3,             -- client --> gate 心跳 请求
    Gt_Clt_Heartbeat = 4,             -- gate --> client 心跳 反馈

    LuaStart = 256,                    -- Lua消息ID起始值

    Clt_L_Login = 257,                   -- client --> login 登录

    Max = 1024,
}
