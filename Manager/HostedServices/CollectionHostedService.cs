using AutoMapper;
using Manager.General;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Manager.HostedServices
{
    public class CollectionHostedService : IHostedService, IDisposable
    {
        protected readonly TrainSetManager _trainSetManager;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public static bool IS_IN_PROGRESS = false;
        public static bool IS_STOP_REQUIRED = false;

        const int DELAY_TIME = 60_000;

        public CollectionHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = new LoggerFactory().CreateLogger(GetType().Name);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (IS_IN_PROGRESS || IS_STOP_REQUIRED)
                {
                    break;
                }

                try
                {
                    IS_IN_PROGRESS = true;

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        IS_IN_PROGRESS = true;
                        var manager = scope.ServiceProvider.GetRequiredService<TrainSetManager>();
                        await manager.ScheduleUpdate();
                    }
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, "Can not schedule update train sets");
                }
                finally
                {
                    IS_IN_PROGRESS = false;
                }

                await Task.Delay(DELAY_TIME);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            IS_STOP_REQUIRED = true;
            if (!IS_IN_PROGRESS) return;

            int counter = 0;

            while (IS_IN_PROGRESS && counter < 1000)
            {
                await Task.Delay(200);
                counter++;
            }
        }

        public void Dispose()
        {

        }
    }
}
