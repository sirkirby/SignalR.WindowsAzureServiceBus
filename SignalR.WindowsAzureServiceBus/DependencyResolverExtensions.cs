//---------------------------------------------------------------------------------
// Copyright (c) 2012, Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//---------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SignalR.WindowsAzureServiceBus
{
	public static class DependencyResolverExtensions
	{
		public static IDependencyResolver UseWindowsAzureServiceBus(this IDependencyResolver resolver,
																																string connectionStringName,
		                                                            string topicPathPrefix,
		                                                            int numberOfTopics)
		{
			var instanceId = Environment.MachineName + "_" + GetRoleInstanceNumber().ToString();

			return UseWindowsAzureServiceBus(resolver,
																			 connectionStringName,
			                                 topicPathPrefix,
			                                 numberOfTopics,
			                                 instanceId);
		}

		public static IDependencyResolver UseWindowsAzureServiceBus(this IDependencyResolver resolver,
		                                                            string connectionStringName,
		                                                            string topicPathPrefix,
		                                                            int numberOfTopics,
		                                                            string instanceId)
		{
			var bus = new Lazy<ServiceBusMessageBus>(() => new ServiceBusMessageBus(topicPathPrefix,
			                                                                        numberOfTopics,
			                                                                        connectionStringName,
			                                                                        instanceId));
			resolver.Register(typeof (IMessageBus), () => bus.Value);
			return resolver;
		}

		private static Int64 GetRoleInstanceNumber()
		{
			// if not in an Azure web role, fall back to the server's unique local ip4 address
			var host = Dns.GetHostEntry(Dns.GetHostName());
			var address = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			return Int64.Parse(address.ToString().Replace(".", ""));
		}
	}
}
