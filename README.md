
## Description
jimu 是一个基于.Net Core 2.0 简单易用的微服务框架，使用了大量的开源库（如 DotNetty, consul.net, Flurl.Http, Json.net, Log4net, Quartz.net ... ）, 支持分布式、高并发和负载均衡， 实现了服务治理（如服务注册、发现、健康检测 ...）和 RPC 调用。  

jimu(积木)，正如其中文名，希望用她来开发项目像搭积木一样简单快速可控，使项目安全可靠稳定，整体架构可拓展、高并发、分布式。

更多详情，[查看 Wiki](https://github.com/grissomlau/jimu/wiki)

## Quick Start
请下载 jimu 源码, 或者下载项目  [jimu.demo](https://github.com/grissomlau/jimu.demo)

## About Me 
项目暂时由我独自开发和管理，问题请提交 [issues](https://github.com/grissomlau/jimu/issues)  
项目的更多资料正在断断续续地整理， 可关注我的 [博客园](http://www.cnblogs.com/grissom007/)  
联系我请发邮件： grissomlau@qq.com

## 核心

jimu 最核心的思想是 IOC 和 DI, 通过配置文件使用了 autofac 将组件注入到框架中，用组件来驱动框架，使框架更具弹性。



## 配置

### 服务端

#### 日志

1. JimuLog4netOptions: Log4net 日志组件

   ```json
   {
       "JimuLog4netOptions":{
           "EnableConsoleLog":true,
           "EnableFileLog":true,
           "FileLogPath":"log",
           "FileLogLevel":"Debug,Info,Warn,Error",
           "ConsoleLogLevel":"Debug,Info,Warn,Error",
           "UseInService": true // ILogger 是否注入到 service 
           
       }
   }
   ```

   

2. JimuNLogOptions: NLog 组件

#### 授权

1. JwtAuthorizationOptions: Jwt 授权

   ```json
   {
       "JwtAuthorizationOptions":{
            "ServiceInvokeIp": "${SERVICE_INVOKE_IP}", //服务宿主的地址
           "ServiceInvokePort": "${SERVICE_INVOKE_PORT}",
           "Protocol": "Netty", //传输协议：Http,Netty
           "SecretKey": "123456", //生成token 的钥匙
           "ValidateLifetime": true,
           "ExpireTimeSpan": "0.16:0:0", //token 有效时长: day.hour:minute:second
           "ValidateIssuer": false, //
           "ValidIssuer": "",
           "ValidateAudience": false,
           "ValidAudience": "",
           "TokenEndpointPath": "/v2/token", //获取 token 的路径
           "CheckCredentialServiceId": "Auth.IServices.IAuthService.Check(context)" //验证用户名密码是否正确的 service id, context 是 JwtAuthorizationContext，包含 UserName，Password等调用上下文信息         
       }
   }
   ```

   

   #### 服务发现

   1. ConsulOptions： 使用 Consul 作为服务发现组件

      ```json
      {
          "ConsulOptions":{
       		 "Ip": "127.0.0.1", //consul ip
         		 "Port": "8500", // consul port
         		 "ServiceGroups": "ctauto.test.store", //服务注册所属的组别
         		 "ServiceInvokeIp": "127.0.0.1", //服务宿主的地址
         		 "ServiceInvokePort": "8004 //服务宿主的端口
          }
      }
```
      
   
   
   

#### 服务调用协议

1. TransportOptions： 服务调用的传输组件

   ```json
   {
       "TransportOptions":{
           "Ip": "127.0.0.1", //服务宿主ip
           "Port": 8001, //服务宿主端口
           "Protocol": "Netty", //传输协议： Netty, Http
           "ServiceInvokeIp": "127.0.0.1", //服务宿主的地址
           "ServiceInvokePort": "8001"
       }
   }
   ```

#### 服务

1. ServiceOptions： 服务配置

   ```json
   {
       "ServiceOptions":{
           "Path": "",//服务dll所在路径，默认当前目录
           "LoadFilePattern": "IService.dll,Service.dll",//需要加载的服务dll，支持统配符:*.dll,*.txt
           "WatchFilePattern": "",//监视文件修改，实现热加载,e.g.:IService.dll,Service.dll,支持通配符：*.dll,*.json
           "EnableWatch": false//是否启动热加载
       }
   }
   ```

##### ORM

数据库接入

1. Dapper

   ```json
   {
      "DapperOptions": {
        "ConnectionString": "server=localhost;database=grissom_dev;user=root;pwd=root;",
        "DbType": "MySql" //数据库类型，支持： MySQL,SQLServer,Oracle,SQLite, PostgreSQL
     }
   }
   ```

   

##### MiniDDD Repository

MiniDDD 是一个轻量级的 DDD 框架， MiniDDD Repository 就是基于该框架的数据仓储，支持以下仓储

1. Dapper

   ```json
   {
     "MiniDDDDapperOptions": {
       "ConnectionString": "server=localhost;database=grissom_dev;user=root;pwd=root;",
       "DbType": "MySql" //数据库类型，支持： MySQL,SQLServer,Oracle,SQLite, PostgreSQL
         // 没有 sql 日志可输出
     }
   }
   ```

   

2. EF

   ```json
   {
      "MiniDDDEFOptions": {
        "ConnectionString": "server=localhost;database=grissom_dev;user=root;pwd=root;",
        "DbType": "MySql", //数据库类型，支持： MySQL,SQLServer,Oracle,SQLite, PostgreSQL
        "TableModelAssemblyName": "",//EF对应的表的实体类dll, Server 项目引用了则不需要设置
        "OpenLogTrace":false, //是否开启 sql 日志,一般 debug 时开启方面查看生成的 sql
        "LogLevel":"Debug" //日志级别： Debug,Information,Warning,Error
      }
   }
   ```

   

3. SqlSugar

   ```json
   {
       "MiniDDDSqlSugarOptions": {
         "ConnectionString": "server=localhost;database=grissom_dev;user=root;pwd=root;",
         "DbType": "MySql", //数据库类型，支持： MySQL,SQLServer,Oracle,SQLite, PostgreSQL
         "OpenLogTrace":false, //是否开启 sql 日志,一般 debug 时开启方面查看生成的 sql，没有日志级别可选
      }
   }
   ```

   



### 客户端

#### 日志

1. JimuLog4netOptions: Log4net 日志组件

   ```json
   {
       "JimuLog4netOptions":{
           "EnableConsoleLog":true,
           "EnableFileLog":true,
           "FileLogPath":"log",
           "FileLogLevel":"Debug,Info,Warn,Error",
           "ConsoleLogLevel":"Debug,Info,Warn,Error"
       }
   }
   ```

   

2. JimuNLogOptions: NLog 组件

#### 授权

1. JwtAuthorizationOptions: Jwt 授权，支持在客户端实现 jwt 授权，服务端不需要配置

```json
{
    "JwtAuthorizationOptions":{
        "Protocol": "Http", //传输协议：Http,Netty
        "SecretKey": "123456", //生成token 的钥匙
        "ValidateLifetime": true,
        "ExpireTimeSpan": "0.16:1:0", //token 有效时长: day.hour:minute:second
        "ValidateIssuer": false, //
        "ValidIssuer": "",
        "ValidateAudience": false,
        "ValidAudience": ""
    }
}
```

#### 服务发现

1. ConsulOptions： 使用 Consul 作为服务发现组件

```json
{
    "ConsulOptions":{
        "Ip": "127.0.0.1",//consul ip
        "Port": 8500,// consul port
        "ServiceGroups": "MyService",//服务注册所属的组别
    }
}
```

#### 服务发现刷新频率

1. DiscoveryOptions：客户端会缓存已发现的服务，设定刷新频率

   ```json
   {
       "DiscoveryOptions":{
           "UpdateJobIntervalMinute":1//单位分钟，1分钟刷新一次
       }
   }
   ```

#### 容错机制

1. FaultTolerantOptions：服务调用时的容错机制

   ```json
   {
       "FaultTolerantOptions":{
           "RetryTimes":0 //服务调用失败重试次数
       }
   }
   ```

   

#### 服务健康监测

1. HealthCheckOptions： 根据已发现服务的ip,port 定时进行服务器心跳监测（客户端主动连接）

   ```json
   {
       "HealthCheckOptions":{
           "IntervalMinute":1 //心跳监测时间间隔，单位分钟
       }
   }
   ```

   

#### 负载均衡

1. LoadBalanceOptions

   ```json
   {
       "LoadBalanceOptions":{
           "LoadBalance":"Polling" //负载均衡算法: Polling - 轮询
       }
   }
   ```

   

#### 远程代理

1. ServiceProxyOptions

   ```json
   {
       "ServiceProxyOptions":{
           "AssemblyNames":["IOrderServices.dll","IUserServices.dll"]//代理接口dll配置
       }
   }
   ```

#### 客户端获取请求 token 的方式

1. TokenGetterOptions

   ```json
   {
       "TokenGetterOptions":{
           "GetFrom":"HttpHeader" //从http header 获取， Request.Headers["Authorization"]
       }
   }
   ```

#### 服务调用

1. TransportOptions： 服务调用的传输组件

   ```json
   {
       "TransportOptions":{
           "Protocol":"Netty,Http" //传输协议： Netty, Http
       }
   }
   ```

