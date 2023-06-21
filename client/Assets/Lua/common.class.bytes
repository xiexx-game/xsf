--------------------------------------------------------------------------
--
-- 文件：Common\Class.lua
-- 作者：Xoen Xie
-- 时间：2023/06/21
-- 描述：类的简单封装
-- 说明：
--
--------------------------------------------------------------------------

function class(class, super)
    local new_class = class or {}
    new_class.super = super
    new_class.__index = new_class

    if super then
        setmetatable(new_class, { __index = function(t, k)
            local v = super[k]
            t[k] = v
            return v
        end })
    end

    function new_class.New()

        local obj = {}
        setmetatable(obj, new_class)
        return obj
    end

    return new_class
end
