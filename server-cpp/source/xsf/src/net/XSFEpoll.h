//////////////////////////////////////////////////////////////////////////
// 
// 文件：source/xsf/src/net/XSFEpoll.h
// 作者：Xoen Xie
// 时间：2020/03/22
// 描述：epoll的一些封装
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_EPOLL_H_
#define _XSF_EPOLL_H_

#include <netdb.h>
#include <unistd.h>
#include <sys/epoll.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <fcntl.h>

namespace xsf
{
	#define MAX_EPOLL_EVENT		64

	struct EpollEvent
	{
		void * ud;
		bool read;
		bool write;
	};


	// 判断epoll描述符是否有效
	inline bool FDInvalid(int efd) 
	{
		return efd == -1;
	}

	// 创建一个epoll描述符
	inline int EpollCreate() 
	{
		return epoll_create1(EPOLL_CLOEXEC);
	}

	// 释放一个epoll描述符
	inline void EpollRelease(int efd) 
	{
		close(efd);
	}

	inline bool EpollAdd(int efd, int fd, void *ud) 
	{
		epoll_event ev;
		ev.events = EPOLLIN;
		ev.data.ptr = ud;
		if (epoll_ctl(efd, EPOLL_CTL_ADD, fd, &ev) == -1) 
		{
			return false;
		}

		return true;
	}

	inline void EpollDel(int efd, int fd)
	{
		epoll_ctl(efd, EPOLL_CTL_DEL, fd , nullptr);
	}


	inline void EpollModify(int efd, int sock, void *ud, bool enable) 
	{
		epoll_event ev;
		ev.events = EPOLLIN | (enable ? EPOLLOUT : 0);
		ev.data.ptr = ud;

		epoll_ctl(efd, EPOLL_CTL_MOD, sock, &ev);
	}


	inline int EpollWait(int efd, EpollEvent *e, int time)
	{
		epoll_event ev[MAX_EPOLL_EVENT];
		int n = epoll_wait(efd, ev, MAX_EPOLL_EVENT, time);

		for (int i = 0; i < n; i++ ) 
		{
			e[i].ud = ev[i].data.ptr;

			unsigned int flag = ev[i].events;
			e[i].write = (flag & EPOLLOUT) != 0;
			e[i].read = (flag & EPOLLIN) != 0;
		}

		return n;
	}

	inline bool FDNonblocking(int fd) 
	{
		int flag = fcntl(fd, F_GETFL, 0);
		if ( -1 == flag ) {
			return false;
		}

		fcntl(fd, F_SETFL, flag | O_NONBLOCK);

		return true;
	}
}


#endif
