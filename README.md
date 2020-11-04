前言
===

本系列前三篇文章分别从ASP\.NET Core认证的三个重要概念，到如何实现最简单的登录、注销和认证，再到如何配置Cookie 选项，来介绍如何使用ASP\.NET Core认证。感兴趣的可以了解一下。

- [ASP.NET Core Authentication系列（一）理解Claim, ClaimsIdentity, ClaimsPrincipal](https://www.cnblogs.com/liang24/p/13910368.html)
- [ASP.NET Core Authentication系列（二）实现认证、登录和注销](https://www.cnblogs.com/liang24/p/13912695.html)
- [ASP.NET Core Authentication系列（三）Cookie选项](https://www.cnblogs.com/liang24/p/13919397.html)

这三篇文章都是从单应用角度来介绍如何使用ASP\.NET Core认证，但是在实际开发中，往往都是多应用、分布式部署的，仅通过上面的内容没办法直接应用到多应用上。例如有3个应用，分别对应PC端、移动端和服务端，假设它们的域名分别为www.91suke.com，m.91suke.com以及service.91suke.com，如何让这三个应用都共享认证。

本文将介绍如何通过共享授权Cookie来实现多应用间单点登录（SSO）。

源码下载地址：[https://github.com/liang24/SSO](https://github.com/liang24/SSO)

如何实现
===

前面我们已经解决了如何使用Cookie来实现认证功能，要实现共享授权Cookie还需要解决以下两个问题：

1. Cookie共享
2. Cookie的认证票据的解析

第一个问题比较简单，只要设置Cookie的域为根域，其他子域都能获得这个Cookie。

```csharp
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "TestCookie"; //设置统一的Cookie名称
        options.Cookie.Domain = ".91suke.com"; //设置Cookie的域为根域，这样所有子域都可以发现这个Cookie
    });
```

第二个问题主要是让多个应用共用解析算法，在ASP\.NET Core里是通过`services.AddDataProtection`配置数据加密保存方式。数据加密配置保存方式现阶段ASP\.NET  Core支持：

- 保存到文件：PersistKeysToFileSystem
- 保存到数据库：PersistKeysToDbContext<Context>
- 保存到Redis：PersistKeysToStackExchangeRedis
- 保存到Azure：PersistKeysToAzureBlobStorage

```csharp
services.AddDataProtection()
    //.PersistKeysToDbContext<SSOContext>()  //把加密数据保存在数据库
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\server\share\directory\"))  //把加密信息保存大文件夹
    //.PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
    .SetApplicationName("SSO"); //把所有子系统都设置为统一的应用名称
```

使用PersistKeysToFileSystem
---

这个方法最简单，就是把生成的票据保存到磁盘目录上，多个应用同时访问这个目录，达到共享效果。

- 优点：实现简单，只要应用有目录权限即可，不需要再配置其他东西。
- 缺点：必须部署在同一台服务器上，无法分布式部署。

使用PersistKeysToDbContext
---

这个方法是把票据持久化到数据库，应用只要有访问数据库的权限，就能达到共享效果。

- 优点：支持分布式部署。
- 缺点：在高并发场景下，数据库IO将会是瓶颈；三种方式里实现的代码量是最多的；

使用PersistKeysToStackExchangeRedis
---

这个方法是把票据保存到Redis缓存里，应用只要有访问Redis的权限，就能达到共享效果。

- 优点：支持分布部署，高并发场景。
- 缺点：需要配置额外的缓存服务器。

参考资料
===

- [Share authentication cookies among ASP.NET apps](https://docs.microsoft.com/en-us/aspnet/core/security/cookie-sharing?view=aspnetcore-3.1)
- [Sharing cookies between applications](https://jakeydocs.readthedocs.io/en/latest/security/data-protection/compatibility/cookie-sharing.html)
- [Asp.Net Core基于Cookie实现同域单点登录(SSO)](https://www.cnblogs.com/liuju150/p/10114778.html)
- [集群环境下，你不得不注意的ASP.NET Core Data Protection 机制](https://www.cnblogs.com/sheng-jie/p/11653196.html)
- [.NET跨平台之旅：ASP.NET Core从传统ASP.NET的Cookie中读取用户登录信息](https://www.cnblogs.com/cmt/p/5940796.html)