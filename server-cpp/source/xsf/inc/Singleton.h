//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/Singleton.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：单例模板
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _SINGLETON_H_
#define _SINGLETON_H_

namespace xsf
{
    template <typename T>
    class Singleton
    {
    public:
        static T * Instance()
        {
            if(m_Instance == nullptr)
                m_Instance = new T();

            return m_Instance;
        }

        // 删除拷贝构造和赋值操作，确保单例的唯一性
        Singleton(const Singleton &) = delete;
        Singleton &operator=(const Singleton &) = delete;

    protected:
        Singleton() {} // 防止外部实例化
    private:
        static T * m_Instance;
    };

    template <typename T>
    T* Singleton<T>::m_Instance = nullptr;
}

#endif
