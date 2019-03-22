
## Description
jimu 是一个基于.Net Core 2.0 简单易用的微服务框架，使用了大量的开源库（如 DotNetty, consul.net, Flurl.Http, Json.net, Log4net, Quartz.net ... ）, 支持分布式、高并发和负载均衡， 实现了服务治理（如服务注册、发现、健康检测 ...）和 RPC 调用。  
jimu 在持续迭代开发中，很多功能还在排期（如可视化监控和管理工具，热更新，服务熔断、限流和降级 ...），如非高手，不建议上生产环境。  

jimu(积木)，正如其中文名，希望用她来开发项目像搭积木一样简单快速可控，使项目安全可靠稳定，整体架构可拓展、高并发、分布式。

更多详情，[查看 Wiki](https://github.com/grissomlau/jimu/wiki)

## Quick Start
### 1. 微服务项目
创建一个基于 .Net Core 2.0 的类库项目，并添加 jimu 依赖
```bash
Install-Package  Jimu
```
添加服务  
注意引用空间： using Jimu;
```csharp 
[JimuServiceRoute("api/{Service}")] // RPC 调用路径
 public class UserService : IJimuService
 {
     [JimuService(CreatedBy = "grissom")] // 指定服务的元数据, 该服务调用路径为 api/user/getname?id=
     public string GetName(string id)
     {
         return $"user id {id}, name enjoy!";
     }
 }

```
### 2. 微服务服务端项目
创建一个基于 .Net Core 2.0 的控制台项目， 并添加 jimu.server 依赖
```bash
Install-Package  Jimu.Server
```
在 Main 函数中添加服务器启动代码  
注意引用空间： using Jimu.Server;
```csharp
static void Main(string[] args)
{
    var hostBuilder = new ServiceHostServerBuilder(new Autofac.ContainerBuilder())
        .UseLog4netLogger()
        .LoadServices("QuickStart.Services")
        .UseDotNettyForTransfer("127.0.0.1", 8001)
        .UseInServerForDiscovery()
        ;
    using (var host = hostBuilder.Build())
    {
        host.Run();
        Console.ReadLine();
    }

}
```
### 3. 微服务客户端项目
创建一个基于 .Net Core 2.0 的 Asp.Net Core Web 应用程序（可选择 API 项目模版），并添加 jimu.client 依赖
```bash
Install-Package  Jimu.Client
```
修改 Startup.cs 类的代码， 以便添加对 jimu 的支持

```csharp
using Jimu.Client;
using Jimu.Client.ApiGateway;

 public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc();
            services.UseJimu();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMvc();
            var host = new ServiceHostClientBuilder(new Autofac.ContainerBuilder())
                .UseLog4netLogger()
                .UsePollingAddressSelector()
                .UseDotNettyForTransfer()
                .UseInServerForDiscovery(new Jimu.DotNettyAddress("127.0.0.1", 8001))
                .Build();
            app.UseJimu(host);
            host.Run();
        }
    }
```
### 4. 同时启动 服务端 和 客户端 
然后在浏览器访问： http://localhost:58156/api/user/getname?id=666

### 5. 更多 demo 
请下载 jimu 源码, 或者下载项目  [jimu.demo](https://github.com/grissomlau/jimu.demo)

## About Me 
项目暂时由我独自开发和管理，问题请提交 [issues](https://github.com/grissomlau/jimu/issues)  
项目的更多资料正在断断续续地整理， 可关注我的 [博客园](http://www.cnblogs.com/grissom007/)  
联系我请发邮件： grissomlau@qq.com



## 配置

### 服务器

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
           "UseInService": true
           
       }
   }
   ```

   

2. JimuNLogOptions: NLog 组件

#### 授权

1. JwtAuthorizationOptions: Jwt 授权

   ```json
   {
       "JwtAuthorizationOptions":{
           "ServerIp": "192.168.10.195",
           "ServerPort": 8001,
           "Protocol": "Http", //传输协议：Http,Netty
           "SecretKey": "123456",//生成token 的钥匙
           "ValidateLifetime": true,
           "ExpireTimeSpan": "1.1:1:0",//token 有效时长: day.hour:minute:second
           "ValidateIssuer": false,//
           "ValidIssuer": "",
           "ValidateAudience": false,
           "ValidAudience": "",
           "TokenEndpointPath": "/api/v1/token",//获取 token 的路径
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
              "ServerAddress": "192.168.10.195:8001",//服务宿主的地址
          }
      }
      ```

      

   

#### 服务调用协议

1. TransportOptions： 服务调用的传输组件

   ```json
   {
       "TransportOptions":{
           "Ip":"192.168.10.195",//服务宿主ip
           "Port": 8001, //服务宿主端口
           "Protocol":"Netty" //传输协议： Netty, Http
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
        "ServerIp": "192.168.10.195",
        "ServerPort": 8001,
        "Protocol": "Http", //传输协议：Http,Netty
        "SecretKey": "123456",//生成token 的钥匙
        "ValidateLifetime": true,
        "ExpireTimeSpan": "1.1:1:0",//token 有效时长: day.hour:minute:second
        "ValidateIssuer": false,//
        "ValidIssuer": "",
        "ValidateAudience": false,
        "ValidAudience": "",
        "TokenEndpointPath": "/api/v1/token",//获取 token 的路径
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
           "GetType":"HttpHeader" //从http header 获取， Request.Headers["Authorization"]
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

#### 