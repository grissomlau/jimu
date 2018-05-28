using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jimu.Server.Proxy
{
    public interface IServiceProxy
    {
        /// <summary>
        /// 生成服务代理。
        /// </summary>
        /// <param name="interfacTypes">需要被代理的接口类型。</param>
        /// <returns>服务代理实现。</returns>
        IEnumerable<Type> GenerateProxys(IEnumerable<Type> interfacTypes);

        /// <summary>
        /// 生成服务代理代码树。
        /// </summary>
        /// <param name="interfaceType">需要被代理的接口类型。</param>
        /// <returns>代码树。</returns>
        SyntaxTree GenerateProxyTree(Type interfaceType);
    }
}
