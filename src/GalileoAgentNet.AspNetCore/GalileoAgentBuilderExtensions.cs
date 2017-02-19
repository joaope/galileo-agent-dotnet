using System;
using GalileoAgentNet.Configuration;
using Microsoft.AspNetCore.Builder;

namespace GalileoAgentNet.AspNetCore
{
    public static class GalileoAgentBuilderExtensions
    {
        public static IApplicationBuilder UseGalileo(this IApplicationBuilder builder, AgentConfiguration configuration, IEntriesQueue queue)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }
            
            GalileoAgentAccessor.AgentInstance = new GalileoAgent(configuration, queue);
            return builder.UseMiddleware<GalileoAgentMiddleware>();
        }

        public static IApplicationBuilder UseGalileo(this IApplicationBuilder builder, AgentConfiguration configuration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            GalileoAgentAccessor.AgentInstance = new GalileoAgent(configuration);
            return builder.UseMiddleware<GalileoAgentMiddleware>();
        }

        public static IApplicationBuilder UseGalileo(this IApplicationBuilder builder, string galileoServiceToken)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            GalileoAgentAccessor.AgentInstance = new GalileoAgent(galileoServiceToken);
            return builder.UseMiddleware<GalileoAgentMiddleware>();
        }
    }
}
